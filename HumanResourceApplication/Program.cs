using AutoMapper;
using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;

using Microsoft.Data.SqlClient;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

//create Imapper instance and pass the mapperconfig to it
IMapper mapper = mapperConfig.CreateMapper();

//register the mapper instance to the service container
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IEmployeeRepo, EmployeeService>();


builder.Services.AddValidatorsFromAssemblyContaining<EmployeeDTO>();

builder.Services.AddValidatorsFromAssemblyContaining<JobHistoryDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<JobDTO>();

builder.Services.AddScoped<IJobRepository, JobServices>();
builder.Services.AddScoped<IJobHistoryRepository, JobHistoryServices>();


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
