using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using HumanResourceApplication.Models;
using FluentValidation;
using HumanResourceApplication.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register the DbContext with a connection string
builder.Services.AddDbContext<HrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Create AutoMapper configuration and register it
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Register repositories
builder.Services.AddScoped<IEmployeeRepo, EmployeeService>();
builder.Services.AddScoped<ICountryRepository, CountryService>();
builder.Services.AddScoped<IJobRepository, JobServices>();
builder.Services.AddScoped<IJobHistoryRepository, JobHistoryServices>();

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CountryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<JobDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<JobHistoryDTO>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
