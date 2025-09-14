using Microsoft.EntityFrameworkCore;
using WebApiJobs.Data;
using WebApiJobs.Infrastructure;

namespace WebApiJobs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddLogging(x => x.AddSimpleConsole());
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors(true);
                    options.EnableSensitiveDataLogging(true);
                    options.LogTo(Console.WriteLine);
                }
                options.UseSqlServer(builder.Configuration.GetConnectionString("connectionstring"));
            });

            builder.Services.AddInfrastructure();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
