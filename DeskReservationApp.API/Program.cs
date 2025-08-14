using DeskReservationApp.Application.Mappings;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DeskReservationApp.Infrastructure.Persistance;
using DeskReservationApp.Infrastructure.Persistance.Repositories;
using DeskReservationApp.Infrastructure.Persistance.Configurations;
using DeskReservationApp.Infrastructure.Mappings;
using DeskReservationApp.Application.Services;
using DeskReservationApp.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT bearer
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Desk Reservation App", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: '{jwt-token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "bearer",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Database contexts
builder.Services.AddDbContext<DeskReservationAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeskReservationAuthConnectionString")));

builder.Services.AddDbContext<DeskReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeskReservationConnectionString")));

// Configuration
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Register repositories
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IDeskRepository, DeskRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register application services
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Infrastructure services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<RoleSeedService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles), typeof(InfrastructureMappingProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Identity Core
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("DeskReservationApp")
    .AddEntityFrameworkStores<DeskReservationAuthDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
});

// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeamLeadOrAdmin", policy => policy.RequireRole("TeamLead", "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed roles at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Apply Auth database migrations
    var authDb = services.GetRequiredService<DeskReservationAuthDbContext>();
    authDb.Database.Migrate();

    // Apply Business database migrations
    var businessDb = services.GetRequiredService<DeskReservationDbContext>();
    businessDb.Database.Migrate();

    var roleSeedService = services.GetRequiredService<RoleSeedService>();
    await roleSeedService.SeedRolesAsync();
}

app.Run();