using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Win32;
using HumanResourceApplication.Models;
using FluentValidation;
using HumanResourceApplication.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register the DbContext with a connection string
builder.Services.AddDbContext<HrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//create mapper configuration and passing it to the mapper profile
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

//create Imapper instance and pass the mapperconfig to it
IMapper mapper = mapperConfig.CreateMapper();

//register the mapper instance to the service container
builder.Services.AddSingleton(mapper);

//Register the repository
builder.Services.AddScoped<ICountryRepository,CountryService>();

//Configure the fluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CountryValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining<CountryPutValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
