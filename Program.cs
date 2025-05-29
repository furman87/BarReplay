using BarReplay;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<BarService>();
builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseDefaultFiles();  // Serve index.html by default
app.UseStaticFiles();   // Enable wwwroot

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

