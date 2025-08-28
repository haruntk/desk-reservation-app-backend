using DeskReservationApp.Application.Mappings;
using DeskReservationApp.Domain.Interfaces;
using DeskReservationApp.Application.Interfaces;
using DeskReservationApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DeskReservationApp.Infrastructure.Persistance;
using DeskReservationApp.Infrastructure.Persistance.Repositories;
using DeskReservationApp.Domain.Configuration;
using DeskReservationApp.Infrastructure.Mappings;
using DeskReservationApp.Application.Services;
using DeskReservationApp.API.Middleware;
using DeskReservationApp.Infrastructure.Persistance.Configurations;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + Windows Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Desk Reservation App", Version = "v1" });
    options.AddSecurityDefinition("windows", new OpenApiSecurityScheme
    {
        Description = "Windows Authentication",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "negotiate"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "windows"
                }
            },
            new List<string>()
        }
    });
});

// Database contexts
builder.Services.AddDbContext<DeskReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeskReservationConnectionString")));

builder.Services.AddDbContext<DeskReservationAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeskReservationAuthConnectionString")));

// Configuration
builder.Services.Configure<ReservationStatusOptions>(builder.Configuration.GetSection(ReservationStatusOptions.SectionName));
builder.Services.Configure<WindowsAuthOptions>(builder.Configuration.GetSection(WindowsAuthOptions.SectionName));

// Register repositories
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IDeskRepository, DeskRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();

// Register application services
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Add Email Service
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Infrastructure services
builder.Services.AddScoped<IWindowsAuthService, WindowsAuthService>();
builder.Services.AddScoped<RoleSeedService>();
builder.Services.AddScoped<IReservationStatusService, ReservationStatusService>();
builder.Services.AddScoped<IClaimsTransformation, WindowsClaimsTransformation>();

// Background services
builder.Services.AddHostedService<ReservationStatusBackgroundService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles), typeof(InfrastructureMappingProfile));

// CORS - Updated for Windows Authentication
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173") // Frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // Required for Windows Authentication
});

// Windows Authentication
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate(options =>
    {
        // Configure for IIS and HTTP.sys
        options.PersistKerberosCredentials = true;
        options.PersistNtlmCredentials = true;
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

// Apply migrations and seed data at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // Apply database migrations
    var businessDb = services.GetRequiredService<DeskReservationDbContext>();
    businessDb.Database.Migrate();

    var authDb = services.GetRequiredService<DeskReservationAuthDbContext>();
    authDb.Database.Migrate();

    var roleSeedService = services.GetRequiredService<RoleSeedService>();
    await roleSeedService.SeedRolesAsync();
}

app.Run();