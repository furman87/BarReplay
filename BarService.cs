namespace BarReplay
{
    using Dapper;
    using System.Data;

    public class BarService
    {
        private readonly IDbConnection _db;
        private string _tableName;

        public BarService(IConfiguration config)
        {
            _db = new Npgsql.NpgsqlConnection(config.GetConnectionString("Default"));
            _tableName = GetTableName(1);
        }

        public async Task<IEnumerable<Bar>> GetInitialBars(DateTime start, int interval, int count)
        {
            _tableName = GetTableName(interval);
            var sql = @$"
            SELECT EXTRACT(EPOCH FROM bar_date)::bigint AS Time, open_price AS Open, high_price AS High, low_price AS Low, COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date)) AS Close

            FROM {_tableName}
            WHERE bar_date <= :Start
            ORDER BY bar_date DESC
            LIMIT :Count";

            var bars = await _db.QueryAsync<Bar>(sql, new { Start = start, Count = count });
            return bars.Reverse(); // display oldest first
        }

        public async Task<Bar?> GetNextBar(DateTime after, int interval)
        {
            _tableName = GetTableName(interval);
            var sql = @$"
            SELECT EXTRACT(EPOCH FROM bar_date)::bigint AS Time, open_price AS Open, high_price AS High, low_price AS Low, COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date)) AS Close
            FROM {_tableName}
            WHERE bar_date > :After
            ORDER BY bar_date
            LIMIT 1";

            return await _db.QueryFirstOrDefaultAsync<Bar>(sql, new { After = after });
        }

        public async Task<IEnumerable<Bar>> GetBarsBefore(DateTime time, int interval, int count)
        {
            _tableName = GetTableName(interval);
            var sql = @$"
            SELECT EXTRACT(EPOCH FROM bar_date)::bigint AS Time,
                   open_price AS Open, high_price AS High, low_price AS Low, COALESCE(close_price, LEAD(open_price) OVER (ORDER BY bar_date)) AS Close
            FROM {_tableName}
            WHERE bar_date < :Time
            ORDER BY bar_date DESC
            LIMIT :Count";

            var bars = await _db.QueryAsync<Bar>(sql, new { Time = time, Count = count });
            return bars.Reverse(); // return oldest to newest
        }
        
        private string GetTableName(int interval)
        {
            return interval == 1 ? "nq_1m" : $"vm_nq_{interval}m";
        }
    }
}
