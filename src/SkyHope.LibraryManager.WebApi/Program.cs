using Microsoft.EntityFrameworkCore;
using LibraryManager.DataAccess;
using LibraryManager.DataAccess.Repositories;
using LibraryManager.DataAccess.Models;
using NLog;
using NLog.Web;


namespace LibraryManager.LibraryManagerApi;
public class Program
{
    public static void Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddUserSecrets<Program>();
            var config = configBuilder.Build();

            builder.Services.AddDbContext<LibraryContext>(options =>
            options.UseSqlServer(builder.Configuration["Database:ConnectionString"]));
            builder.Services.Configure<DatabaseOptions>(config.GetSection("Database"));
            builder.Services.AddSingleton<ILibraryManagerRepository<User>, UserRepository>();
            builder.Services.AddSingleton<ILibraryManagerRepository<Book>, BookRepository>();
            builder.Services.AddSingleton<ILibraryManagerRepository<Author>, AuthorRepository>();
            builder.Services.AddControllers();

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


            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}