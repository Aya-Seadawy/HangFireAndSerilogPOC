using Hangfire;
using HangfirProject.Middleware;
using HangfirProject.SerilogSinks;
using HangfirProject.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .MinimumLevel.Debug()
        .WriteTo.CustomSink()
        .Enrich.FromLogContext()
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(x =>
{
    x.UseSqlServerStorage(configuration.GetConnectionString("DBConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IJobTestService, JobTestService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
app.MapControllers();
app.UseHangfireDashboard();

app.Run();
