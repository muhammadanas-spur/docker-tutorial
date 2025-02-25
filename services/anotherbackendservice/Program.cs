using Microsoft.EntityFrameworkCore;
using WorkHelpers.Context;

namespace anotherbackendservice;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContext<WorkDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
        });
        builder.Services.AddControllers();
            builder.Services.AddCors(options =>
         {
             options.AddPolicy("AllowAll", builder =>
             {
                 builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
             });
         });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
