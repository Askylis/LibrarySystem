using Microsoft.EntityFrameworkCore;
using LibraryManager.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LibraryManager.DataAccess.Repositories;
using LibraryManager.DataAccess.Models;


namespace LibraryManager.LibraryManagerApi;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration["Database:ConnectionString"]));
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddUserSecrets<Program>();
        var config = configBuilder.Build();
        builder.Services.Configure<DatabaseOptions>(config.GetSection("Database"));
        builder.Services.AddScoped<ILibraryManagerRepository<User>, UserRepository>();
        builder.Services.AddScoped<ILibraryManagerRepository<Book>, BookRepository>();
        builder.Services.AddScoped<ILibraryManagerRepository<Author>, AuthorRepository>();
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
}