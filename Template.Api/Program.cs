using Template.Api.Middlewares;
using Template.Infra.Persistence.Contexts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AdicionarSwagger(builder.Configuration, typeof(Program).Assembly.GetName().Version.ToString());

builder.Services.AddSwaggerGen();

builder.Services.AdicionarInfra(builder.Configuration);

builder.Services.AddApplication();

var app = builder.Build();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
await app.InitializeDatabaseAsync();
await app.RunAsync();
