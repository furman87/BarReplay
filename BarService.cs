namespace BarReplay
{
    using Dapper;
    using System.Data;

    /// <summary>
    /// Service for managing and retrieving bar data from the database.
    /// </summary>
    public class BarService
    {
        private readonly IDbConnection _db;
        private string tableName;
        private readonly ILogger<BarService> _logger;

        public BarService(IConfiguration config, ILogger<BarService> logger)
        {
            _db = new Npgsql.NpgsqlConnection(config.GetConnectionString("Default"));
            tableName = GetTableName(1);
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the initial set of bars starting from a specified date, with a given interval and count.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="interval"></param>
        /// <param name="count"></param>
        /// <returns>Enumerable of bar objects.</returns>
        public async Task<IEnumerable<Bar>> GetInitialBars(DateTime start, int interval, int count)
        {
            if (interval > 1)
            {
                await CreateOrRefreshView(interval);
            }

            tableName = GetTableName(interval);

            var sql = @$"
            SELECT
                EXTRACT(EPOCH FROM bar_date)::bigint AS Time,
                open_price AS Open,
                high_price AS High,
                low_price AS Low,
                COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date), open_price) AS Close
            FROM {tableName}
            WHERE bar_date <= :Start
            ORDER BY bar_date DESC
            LIMIT :Count";

            var bars = await _db.QueryAsync<Bar>(sql, new { Start = start, Count = count });
            return bars.Reverse(); // display oldest first
        }

        /// <summary>
        /// Retrieves the next bar after a specified date, for a given interval.
        /// </summary>
        /// <param name="after"></param>
        /// <param name="interval"></param>
        /// <returns>The next bar object</returns>
        public async Task<Bar?> GetNextBar(DateTime after, int interval)
        {
            _logger.LogInformation("Entered GetNextBar with after={after}, interval={interval}", after, interval);
            tableName = GetTableName(interval);
            var sql = @$"
            SELECT
                EXTRACT(EPOCH FROM bar_date)::bigint AS Time,
                open_price AS Open,
                high_price AS High,
                low_price AS Low,
                COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date), open_price) AS Close
            FROM {tableName}
            WHERE bar_date > :After
            ORDER BY bar_date
            LIMIT 1";

            return await _db.QueryFirstOrDefaultAsync<Bar>(sql, new { After = after });
        }

        /// <summary>
        /// Retrieves a specified number of bars before a given time, for a specified interval.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="interval"></param>
        /// <param name="count"></param>
        /// <returns>Enumerable of bar objects.</returns>
        public async Task<IEnumerable<Bar>> GetBarsBefore(DateTime time, int interval, int count)
        {
            _logger.LogInformation("Entered GetBarsBefore with time={time}, interval={interval}, count={count}", time, interval, count);
            tableName = GetTableName(interval);
            var sql = @$"
            SELECT
                EXTRACT(EPOCH FROM bar_date)::bigint AS Time,
                open_price AS Open,
                high_price AS High,
                low_price AS Low,
                COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date), open_price) AS Close
            FROM {tableName}
            WHERE bar_date < :Time
            ORDER BY bar_date DESC
            LIMIT :Count";

            var bars = await _db.QueryAsync<Bar>(sql, new { Time = time, Count = count });
            return bars.Reverse(); // return oldest to newest
        }

        public async Task<bool> ViewExists(int interval)
        {
            _logger.LogInformation("Entered ViewExists with interval={interval}", interval);
            var tableName = GetTableName(interval);
            var sql = @$"SELECT EXISTS (SELECT 1 FROM pg_matviews WHERE matviewname = :tableName)";
            return await _db.ExecuteScalarAsync<bool>(sql, new { tableName });
        }

        /// <summary>
        /// Triggers a refresh of the bars for a specified interval.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public async Task CreateOrRefreshView(int interval)
        {
            _logger.LogInformation("Entered CreateOrRefreshView with interval={interval}", interval);
            var sql = @$"SELECT create_or_refresh_nq_xm_view(:interval)";

            await _db.ExecuteAsync(sql, new { interval });
            return;
        }

        private string GetTableName(int interval)
        {
            return interval == 1 ? "nq_1m" : $"vm_nq_{interval}m";
        }
    }
}
