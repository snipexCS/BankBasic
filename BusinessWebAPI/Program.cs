var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Optional: configure HTTP logging, CORS for GUI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapGet("/", () => "BusinessWebAPI running");
app.MapControllers();

app.Run();
