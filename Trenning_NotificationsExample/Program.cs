using Microsoft.OpenApi.Models;
using Trenning_NotificationsExample.MongoDB;

using Trenning_NotificationsExample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.Configure<PassportsDatabaseSettings>(
    builder.Configuration.GetSection("PassportsDatabase"));

builder.Services.AddSingleton<PassportChangesService>();
builder.Services.AddHostedService<PassportUpdateHostedService>();
builder.Services.AddHttpClient<FileUpdateService>();



var app = builder.Build();

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
