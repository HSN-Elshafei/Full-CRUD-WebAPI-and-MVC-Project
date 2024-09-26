
using Microsoft.EntityFrameworkCore;
using webAPIDay_2.Models;

namespace webAPIDay_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddControllers().AddJsonOptions(
            //        options =>
            //        {
            //            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            //        }
            //    );
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ITIContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddCors(co =>
            {
                co.AddPolicy("hsn", pb =>
                {
                    pb.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseCors("hsn");


            app.MapControllers();

            app.Run();
        }
    }
}
