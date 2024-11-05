using ArticleProject.Application;
using ArticleProject.Presentation.Extensions;
using ArticleProject.Presentation.Middlewares;
using ArticleProject.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJWTAuth(builder.Configuration);
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(AppAssembly).Assembly);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Serilog as the default logging provider
app.UseSerilogRequestLogging();

// Use the custom global exception handler middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
