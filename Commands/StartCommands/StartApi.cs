using Microsoft.Extensions.FileProviders;
using TG.DataBase;
using Microsoft.EntityFrameworkCore;
using TG.Users;
using System.Text.Json;
using TG.Attractions;
using TG.API;

namespace TG.Commands.StartCommands
{
    public class StartApi
    {
        public static void Start(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<AttractionDbContext>();
            builder.Services.AddTransient<UserDbContext>();
            var app = builder.Build();
            

            HtmlPages.Html(app);
            Actions.Acts(app);
            Swagger.UseSwaggerUI(app);
            DatabaseHelper? d = new DatabaseHelper();
            d.InitializeDatabase();
            DataBaseManager dbm = new DataBaseManager();
            dbm.DataBaseM();
            // DatabaseUsers du = new DatabaseUsers();
            // du.InitializeUsersDatabase();

            app.UseStaticFiles();
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assets")),
            //     RequestPath = "/Assets"
            // });

            var rootPath = Path.Combine(AppContext.BaseDirectory, "Assets");
            var fileProvider = new PhysicalFileProvider(rootPath);

            var frontendPath = Path.Combine(AppContext.BaseDirectory, "Frontend");
            var fileProvider2 = new PhysicalFileProvider(frontendPath);




            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Frontend")),
            //     RequestPath = "/Frontend"
            // });

            app.UseHttpsRedirection();
            app.Run();

        }
    }
}

public partial class Program { }