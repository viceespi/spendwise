using System.Data;
using Npgsql;
using SpendWise.Domain.Factories;
using SpendWise.Domain.Factories.Contracts;
using SpendWise.Domain.Repositories;
using SpendWise.Domain.Repositories.Contracts;
using SpendWise.Domain.Services;
using SpendWise.Domain.Services.Contracts;
using SpendWise.Domain.Validators;
using SpendWise.Domain.Validators.Contracts;

var builder = WebApplication.CreateBuilder(args);
// Add scoped configures the service locator with the scoped lifetime
// Service locator is responsible for creating instances of classes that are requested to it, like in dependency injection
builder.Services.AddScoped<IExpenseManagementServices, ExpenseManagementServices>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseFactory, ExpenseFactory>();
builder.Services.AddScoped<IExpenseValidator, ExpenseValidator>();
builder.Services.AddScoped<IDbConnection>(serviceLocator =>
{
    IConfiguration configuration = serviceLocator.GetRequiredService<IConfiguration>();
    string? connectionString = configuration.GetConnectionString("SpendWiseDB");
    return new NpgsqlConnection(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("AllowAnyOrigin");
app.MapControllers();

app.Run();
