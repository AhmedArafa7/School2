
using FluentValidation;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Behaviors;
using Hospital_VerticalSlice.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace Hospital_VerticalSlice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            const string corsPolicy = "AllowAngularApp";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, policy =>
                {
                    policy.WithOrigins("http://localhost:4200","https://hospital-client2.pages.dev")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // مهم لدعم Cookies والـ SignalR مستقبلاً
                });
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddMediatR(opt => opt.RegisterServicesFromAssembly(typeof(Program).Assembly));

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSettings:Key"]!)),

            // تأكد من مطابقة الـ Issuer والـ Audience إذا كنت تستخدمهما في الـ TokenService، 
            // أو اجعلهما false إذا كنت لا تفحصهما:
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // إلغاء الـ 5 دقائق الإضافية الافتراضية
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // إذا كان سبب الفشل هو انتهاء صلاحية التوكن فقط
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                // التأكد من عدم وجود مسجّل بهذا البريد مسبقاً
                if (!context.Users.Any(u => u.Email == "admin@school.com"))
                {
                    var adminUser = new Hospital_VerticalSlice.Entities.User // تأكد من مسار الـ Namespace الصحيح للـ User
                    {
                        Name = "System Admin",
                        Email = "admin@school.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), // كلمة المرور الافتراضية
                        Role = Role.Admin // أو الرقم المقابل لصلاحية الأدمن لديك (مثل 2 أو Admin)
                    };

                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(); // السطر الجديد
            }

            app.UseHttpsRedirection();

            app.UseCors(corsPolicy);

            app.UseAuthentication(); 
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
