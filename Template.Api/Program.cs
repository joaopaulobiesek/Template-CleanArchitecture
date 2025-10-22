using Hangfire;
using QuestPDF.Infrastructure;
using Template.Api.Middlewares;
using Template.Infra.BackgroundJobs;
using Template.Infra.Persistence.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AdicionarSwagger(builder.Configuration, typeof(Program).Assembly.GetName().Version.ToString());

builder.Services.AddSwaggerGen();

builder.Services.AdicionarInfra(builder.Configuration);

builder.Services.AddApplication();

var app = builder.Build();

QuestPDF.Settings.License = LicenseType.Community;

// CORS configurado corretamente para permitir cookies e frontend específico
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:4200";

app.UseCors(options => options
    .WithOrigins(frontendUrl)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());  // ← IMPORTANTE para cookies HTTP-Only funcionarem

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Tenant.Api.v1/swagger.json", "Tenant API v1");
    });
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapControllers();
await app.InitializeDatabaseAsync();

app.Services.ConfigureRecurringJobs();

await app.RunAsync();