using Cut_Cut.DAL.DBContext;
using Cut_Cut.DAL.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.OpenApi.Models;
using Cut_Cut.API.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Cut_Cut.BLL.Interfaces;
using Cut_Cut.BLL.Services;
using Cut_Cut.DAL.IRepositories;
using Cut_Cut.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

//GET the configurations from appsettings.JSON
ConfigurationManager configuration = builder.Configuration;

//GET the current Enviroment
var env = builder.Environment;

// Configure the right appsettings.json for the actual enviroment
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

// GET connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DBConnection");

Console.WriteLine(connectionString + " " + env.EnvironmentName);

// Add services to the container.

//Add DB Conmection
builder.Services.AddDbContext<CutCutDbContext>(options =>
    options.UseSqlite(connectionString));

//Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.ApiKey,
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
var services = new ServiceCollection();
builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperConfiguration());
});
IMapper mapper = mapperConfig.CreateMapper();
services.AddSingleton(mapper);

//Add Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CutCutDbContext>()
    .AddDefaultTokenProviders();


//Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
//Add JWT Bearer
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.MapIdentityApi<User>();

app.MapControllers();

app.Run();
