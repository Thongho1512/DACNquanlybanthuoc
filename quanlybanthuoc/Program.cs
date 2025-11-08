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
// 1️⃣ Configure Serilog
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();
builder.Services.AddScoped<IChiNhanhRepository, ChiNhanhRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<IChiTietDonHangRepository, ChiTietDonHangRepository>();
builder.Services.AddScoped<IPhuongThucThanhToanRepository, PhuongThucThanhToanRepository>();
builder.Services.AddScoped<ILichSuDiemRepository, LichSuDiemRepository>();
builder.Services.AddScoped<ILoHangRepository, LoHangRepository>();
builder.Services.AddScoped<IKhoHangRepository, KhoHangRepository>();
builder.Services.AddScoped<IDonNhapHangRepository, DonNhapHangRepository>();

// Services
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IThuocService, ThuocService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<INhaCungCapService, NhaCungCapService>();
builder.Services.AddScoped<IChiNhanhService, ChiNhanhService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IPhuongThucThanhToanService, PhuongThucThanhToanService>();
builder.Services.AddScoped<ILoHangService, LoHangService>();
builder.Services.AddScoped<IKhoHangService, KhoHangService>();
builder.Services.AddScoped<IDonNhapHangService, DonNhapHangService>();
builder.Services.AddScoped<IVaiTroService, VaiTroService>();


// ============================================
// 3️⃣ Authentication & Authorization
// ============================================
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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

// ============================================
// 4️⃣ AUTHORIZATION POLICIES THEO TÀI LIỆU
// ============================================
builder.Services.AddAuthorization(options =>
{
    // =========================================
    // POLICY: AdminOnly - Chỉ ADMIN
    // =========================================
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("ADMIN"));

    // =========================================
    // POLICY: AdminOrManager - ADMIN hoặc MANAGER
    // =========================================
    options.AddPolicy("AdminOrManager", policy =>
        policy.RequireRole("ADMIN", "MANAGER"));

    // =========================================
    // POLICY: AllStaff - Tất cả nhân viên (trừ ADMIN)
    // =========================================
    options.AddPolicy("AllStaff", policy =>
        policy.RequireRole("MANAGER", "STAFF", "WAREHOUSE_STAFF"));

    // =========================================
    // POLICY: SalesStaff - Nhân viên bán hàng
    // =========================================
    options.AddPolicy("SalesStaff", policy =>
        policy.RequireRole("ADMIN", "MANAGER", "STAFF"));

    // =========================================
    // POLICY: WarehouseStaff - Nhân viên kho
    // =========================================
    options.AddPolicy("WarehouseStaff", policy =>
        policy.RequireRole("ADMIN", "MANAGER", "WAREHOUSE_STAFF"));

    // =========================================
    // POLICY: AllUsers - Tất cả người dùng đã đăng nhập
    // =========================================
    options.AddPolicy("AllUsers", policy =>
        policy.RequireAuthenticatedUser());
});

// ============================================
// 5️⃣ CORS Configuration
// ============================================
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

// Data initializer
builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

app.UseCors("AllowFrontend");

// ============================================
// 6️⃣ Configure HTTP request pipeline
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

// Serilog request logging
app.UseSerilogRequestLogging();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============================================
// 7️⃣ Seed Data (4 Roles + Sample Users)
// ============================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await DataInitializer.SeedData(dbContext);
}

// ============================================
// 8️⃣ Run application
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