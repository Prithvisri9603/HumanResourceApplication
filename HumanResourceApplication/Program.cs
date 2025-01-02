using AutoMapper;
using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();


// Adding Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

builder.Services.AddAuthorization();


builder.Services.AddAuthorization();



// Register the DbContext with a connection string
builder.Services.AddDbContext<HrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//create mapper configuration and passing it to the profile
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
//create IMapper instance and pass the mapperconfig to it 
IMapper mapper = mapperConfig.CreateMapper();

//register the mapper instance to the service container
builder.Services.AddSingleton(mapper);

// Register repositories
builder.Services.AddScoped<IEmployeeRepo, EmployeeService>();
builder.Services.AddScoped<ICountryRepository, CountryService>();
builder.Services.AddScoped<IJobRepository, JobServices>();
builder.Services.AddScoped<IJobHistoryRepository, JobHistoryServices>();
builder.Services.AddScoped<ILocationRepository, LocationServices>();
builder.Services.AddScoped<IDepartmentRepository, DeptServices>();
builder.Services.AddScoped<IRegionRepository, RegionServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();

//builder.Services.AddScoped<ICountryRepository, CountryService>();
//builder.Services.AddScoped<IRegionRepository, RegionServices>();

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CountryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<JobDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<JobHistoryDTO>();
builder.Services.AddValidatorsFromAssemblyContaining<LocationDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DepartmentDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegionDTOValidator>();


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FoodServicePortal",
        Version = "v1",
        Description = "PFood Delivery Service API Documentation",
        Contact = new OpenApiContact
        {
            Name = "Admin",
            Email = "admin1@example.com"
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodServicePortal v1");
    c.RoutePrefix = string.Empty; // This will serve Swagger UI at the root URL
});

//Setting up AJAX
app.UseStaticFiles();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            var message = $"Global Exception :{exception.Error.Message} ";
            await context.Response.WriteAsync(message).ConfigureAwait(false);
        }
    });
});

app.MapControllers();

app.Run();
