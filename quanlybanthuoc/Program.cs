using Microsoft.EntityFrameworkCore;
using Serilog;
using quanlybanthuoc.Data;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Data.Repositories.Impl;
using quanlybanthuoc.Mappings;
using quanlybanthuoc.Middleware.Exceptions;
using quanlybanthuoc.Services;
using quanlybanthuoc.Services.Impl;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1️⃣ Configure Serilog (read from appsettings.json)
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // read Serilog section from appsettings.json
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // replace default logging

// ============================================
// 2️⃣ Add services to the container
// ============================================

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// DbContext and Repositories
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
builder.Services.AddScoped<IVaiTroRepository, VaiTroRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IThuocRepository, ThuocRepository>(); 

// Services
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IThuocService, ThuocService>();

// authentication & authorization
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // validate the token expiration
        ValidateIssuerSigningKey = true, // validate the signing key
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey), // symmetricSecurityKey use the HMACSHA256 algorithm
        ClockSkew = TimeSpan.Zero // eliminate default clock skew
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "https://127.0.0.1:5500",
                "https://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// seed data initializer
builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

app.UseCors("AllowFrontend");

// ============================================
// 3️⃣ Configure HTTP request pipeline
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quản lý bán thuốc API v1");
    });
}

app.UseHttpsRedirection();

// Custom exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// Serilog request logging (logs every HTTP request)
app.UseSerilogRequestLogging();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create data sample
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await DataInitializer.SeedData(dbContext);
}

// ============================================
// 4️⃣ Run application with safe Serilog shutdown
// ============================================

try
{
    Log.Information("Starting up the application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed!");
}
finally
{
    Log.CloseAndFlush();
}
