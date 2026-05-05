using KreatxProject.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace KreatxProject.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Konfigurimi i lidhjes me SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            // 2. SHTIMI I CORS (Leja për React)
            builder.Services.AddCors(options => {
                options.AddPolicy("LejoReact", policy => {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 3. Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // 4. Renditja e Middleware (RËNDËSISHME!)
            app.UseDefaultFiles();
            app.MapStaticAssets();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // AKTIVIZO CORS KËTU (Para Authorization dhe Controllers)
            app.UseCors("LejoReact");

            app.UseAuthorization();
            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}