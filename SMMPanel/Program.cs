using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SMMPanel.Data;
using SMMPanel.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Swagger və kontrollerlər
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(); // MapControllers üçün vacib

// 🔹 DB bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 EmailService üçün konfiqurasiya
builder.Services.Configure<SmtpSetting  >(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<EmailService>();

// 🔹 AuthService və interface
builder.Services.AddScoped<IAuthService, AuthService>();

// 🔹 Authentication & Authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// ✅ BURADA YENİ app OBYEKTI YARADILIR
var app = builder.Build();

// 🔹 Swagger və Dev mühit üçün UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Middleware zənciri
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Controller-ləri map et
app.MapControllers();

app.Run();
