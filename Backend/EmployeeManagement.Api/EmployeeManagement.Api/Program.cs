// Program.cs
// Cấu hình chính của ứng dụng Employee Management System
// Bao gồm: EF Core + MySQL, JWT Auth, Hangfire, CORS, Swagger

using System.Text;
using EmployeeManagement.Api.Configurations;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Endpoints;
using EmployeeManagement.Api.Infrastructure.Swagger;
using EmployeeManagement.Api.Jobs;
using EmployeeManagement.Api.Services;
using EmployeeManagement.Api.Services.Interfaces;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CẤU HÌNH SERILOG LOGGING
// =====================================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// =====================================================
// CẤU HÌNH OPTIONS TỪ APPSETTINGS
// =====================================================
builder.Services.Configure<EmailConfiguration>(
    builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<SystemDefaults>(
    builder.Configuration.GetSection("SystemDefaults"));

// Đăng ký EmailConfiguration như một service singleton
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<IOptions<EmailConfiguration>>().Value);

// =====================================================
// CẤU HÌNH ENTITY FRAMEWORK CORE + MYSQL
// =====================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, 
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            // Cấu hình retry on failure
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        }));

// =====================================================
// CẤU HÌNH JWT AUTHENTICATION
// =====================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// =====================================================
// CẤU HÌNH HANGFIRE + MYSQL STORAGE
// =====================================================
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(new MySqlStorage(
        hangfireConnectionString,
        new MySqlStorageOptions
        {
            TablesPrefix = "Hangfire_",
            PrepareSchemaIfNecessary = true,
            QueuePollInterval = TimeSpan.FromSeconds(15),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            DashboardJobListLimit = 50000,
            TransactionTimeout = TimeSpan.FromMinutes(1)
        })));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
});

// =====================================================
// ĐĂNG KÝ SERVICES (DI)
// =====================================================
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeeContractService, EmployeeContractService>();
builder.Services.AddScoped<IEmployeeHistoryService, EmployeeHistoryService>();
builder.Services.AddScoped<IContractTypeService, ContractTypeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<IEmployeeImportService, EmployeeImportService>();
builder.Services.AddScoped<ReminderJobs>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// =====================================================
// CẤU HÌNH CORS
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",           // Angular dev server
                "http://localhost:5000",           // Local
                "https://localhost:5001",          // Local HTTPS
                "https://atlink.asia",             // Production
                "http://atlink.asia"               // Production HTTP
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =====================================================
// CẤU HÌNH SWAGGER
// =====================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee Management API",
        Version = "v1",
        Description = "Hệ thống quản lý nhân sự nội bộ - An Tưởng Technology",
        Contact = new OpenApiContact
        {
            Name = "An Tưởng Technology",
            Email = "soft@atpro.com.vn"
        }
    });

    // Cấu hình file upload cho Swagger
    options.OperationFilter<SwaggerFileOperationFilter>();

    // Cấu hình JWT Bearer Authentication cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token. Ví dụ: 'eyJhbGciOiJIUzI1NiIsInR5cCI...'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =====================================================
// TỰ ĐỘNG MIGRATE DATABASE
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Log.Information("Đang kiểm tra và migrate database...");
        await dbContext.Database.MigrateAsync();
        Log.Information("Database migration hoàn thành.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Lỗi khi migrate database. Vui lòng tạo migration trước.");
    }
}

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================

// Serilog request logging
app.UseSerilogRequestLogging();

// Khai báo sub-path khi deploy IIS dưới /workping
app.UsePathBase("/workping");
app.UseRouting();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        options.RoutePrefix = "swagger";
    });
}

// Serve Angular SPA - UseDefaultFiles PHẢI trước UseStaticFiles
app.UseDefaultFiles();
app.UseStaticFiles();

// CORS
app.UseCors("AllowAngular");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
// Truy cập tại /hangfire - Cho môi trường internal nên không cần auth phức tạp
app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    // Trong môi trường internal, có thể bỏ qua auth
    // Nếu cần bảo mật, thêm Authorization filter
    Authorization = Array.Empty<IDashboardAuthorizationFilter>(),
    DashboardTitle = "Quản lý Nhân sự - Background Jobs"
});

// =====================================================
// MAP API ENDPOINTS
// =====================================================
app.MapAuthEndpoints();
app.MapEmployeeEndpoints();
app.MapEmployeeContractEndpoints();
app.MapContractTypeEndpoints();
app.MapDashboardEndpoints();
app.MapSystemSettingEndpoints();
app.MapImportEndpoints();
app.MapEmailTemplateEndpoints();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .AllowAnonymous();

// Fallback: trả về index.html cho Angular SPA routing (mọi route không phải /api)
app.MapFallbackToFile("index.html");

// =====================================================
// CẤU HÌNH HANGFIRE RECURRING JOBS
// =====================================================
// Chạy sau khi app đã start
app.Lifetime.ApplicationStarted.Register(() =>
{
    Log.Information("Đang cấu hình Hangfire recurring jobs...");

    // [1] Chúc mừng sinh nhật → nhân viên - Hàng ngày lúc 8:00 AM
    RecurringJob.AddOrUpdate<ReminderJobs>(
        "daily-birthday-emails",
        job => job.SendDailyBirthdayEmailsAsync(),
        "0 8 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") });

    // [2] Danh sách sinh nhật tháng → HR - Ngày 1 hàng tháng lúc 8:00 AM
    RecurringJob.AddOrUpdate<ReminderJobs>(
        "monthly-birthday-list",
        job => job.SendMonthlyBirthdayListAsync(),
        "0 8 1 * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") });

    // [3] Thử việc sắp kết thúc → HR - Hàng ngày lúc 8:30 AM
    RecurringJob.AddOrUpdate<ReminderJobs>(
        "probation-reminder",
        job => job.CheckProbationRemindersAsync(),
        "30 8 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") });

    // [4] Hợp đồng sắp hết hạn → HR - Hàng ngày lúc 9:00 AM
    RecurringJob.AddOrUpdate<ReminderJobs>(
        "contract-reminder",
        job => job.CheckContractRemindersAsync(),
        "0 9 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") });

    Log.Information("Hangfire recurring jobs đã được cấu hình.");
});

// =====================================================
// CHẠY ỨNG DỤNG
// =====================================================
Log.Information("Employee Management API đang khởi động...");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Ứng dụng bị lỗi khi khởi động!");
}
finally
{
    Log.CloseAndFlush();
}
