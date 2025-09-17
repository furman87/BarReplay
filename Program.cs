using BarReplay;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var loggingEnabled = builder.Configuration.GetSection("Logging:Enabled").Get<bool>();
if (loggingEnabled)
{
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.WriteTo.File("/Logs/api.log", rollingInterval: RollingInterval.Day, shared: true)
		.CreateLogger();
	builder.Host.UseSerilog();
}
builder.Services.AddSingleton<BarService>();
builder.Services.AddControllers();
builder.Services.AddCors();


var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseDefaultFiles();  // Serve index.html by default
app.UseStaticFiles();   // Enable wwwroot

// Remove custom logging middleware (Serilog will handle all logging)

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

