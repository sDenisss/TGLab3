using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TG.Attractions;
using TG.DataBase;
using TG.Users;

namespace TG.API
{
    public static class Actions
    {
        public static void Acts(WebApplication app)
        {
       
            app.MapGet("/attractions", (AttractionDbContext db, string? region) =>
            {
                IQueryable<Attraction>? attractions = db.Attraction;

                // Фильтрация по региону, если параметр указан
                if (!string.IsNullOrEmpty(region))
                {
                    attractions = attractions.Where(a => a.Region.Contains(region));
                }

                return Results.Ok(attractions);
            });

            app.MapGet("/attractions/{id}", (int id, AttractionDbContext db) =>
            {
            var attractions = db.Attraction;
            var attraction = attractions.SingleOrDefault(p => p.Id == id);

            if (attraction == null)
                return Results.NotFound();

            return Results.Ok(attraction);
            });

            app.MapGet("/users", (UserDbContext db, string? name) =>
            {
                IQueryable<User>? users = db.User;

                // Фильтрация по региону, если параметр указан
                if (!string.IsNullOrEmpty(name))
                {
                    users = users.Where(a => a.Username.Contains(name));
                }

                return Results.Ok(users);
            });

            app.MapGet("/users/{id}", (int id, UserDbContext db) =>
            {
                var users = db.User;
                var user = users.SingleOrDefault(p => p.Id == id);

                if (user == null)
                    return Results.NotFound();

                return Results.Ok(user);
            });

            // app.MapPost("/users", (User user, UserDbContext db) =>
            // {
            //     var pets = db.User;
            //     db.User.Add(user);
            //     db.SaveChanges();
            //     return Results.Created($"/users/{user.Id}", user);
            // });

            app.MapPost("/users", async (HttpContext context, UserDbContext db) =>
            {

                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();

                // Парсим входящие данные
                var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                if (formData is null || !formData.ContainsKey("name") || !formData.ContainsKey("mail") || !formData.ContainsKey("password"))
                {
                    return Results.BadRequest("Некорректные данные");
                }

                var user = new User
                {
                    Username = formData["name"],
                    Email = formData["mail"],
                    Password = formData["password"]
                };

                try
                {
                    db.User.Add(user);
                    await db.SaveChangesAsync();
                    return Results.Ok(user);
                }
                catch (DbUpdateException ex)
                {
                    return Results.BadRequest("Ошибка при сохранении пользователя: " + ex.Message);
                }
            });

            app.MapPost("/login", async (UserDbContext db, User loginUser) =>
            {
                // Проверяем, существует ли пользователь с таким именем
                var user = await db.User.FirstOrDefaultAsync(u => u.Username == loginUser.Username);

                if (user == null)
                {
                    // Если пользователь не найден
                    return Results.BadRequest(new { message = "Неверное имя или пароль" });
                }

                // Создаем экземпляр класса Auth и проверяем пароль
                var auth = new Auth();
                bool isValidPassword = await auth.VerifyPasswordAsync(loginUser.Password, user.Salt, user.Password);

                if (!isValidPassword)
                {
                    // Если пароль неверный
                    return Results.BadRequest(new { message = "Неверное имя или пароль" });
                }

                // Успешный вход
                return Results.Ok(new { message = "Успешный вход", userId = user.Id });
            });

            app.MapPost("/registry", async (UserDbContext db, User regUser) =>
            {
                // Проверяем, есть ли пользователь с таким именем или почтой
                var existingUser = await db.User
                    .FirstOrDefaultAsync(u => u.Username == regUser.Username || u.Email == regUser.Email);

                if (existingUser != null)
                {
                    // Если пользователь уже существует
                    return Results.BadRequest(new { message = "Пользователь с таким именем или почтой уже существует." });
                }

                // Создаем экземпляр класса Registry и хэшируем пароль
                var registry = new Users.Registry();
                var passwordResult = await registry.HashPasswordAsync(regUser.Password);

                // Устанавливаем хэш и соль для пользователя
                regUser.Password = passwordResult.Hash;
                regUser.Salt = passwordResult.Salt;

                // Добавляем нового пользователя в базу данных
                db.User.Add(regUser);
                await db.SaveChangesAsync();

                // Успешная регистрация
                return Results.Ok(new { message = "Успешная регистрация", userId = regUser.Id });
            });

            app.MapPost("/getEmail", (UserDbContext db, User loginUser) => 
            {
                var user = db.User.FirstOrDefault(u => u.Username == loginUser.Username);

                if (user == null)
                {
                    return Results.BadRequest(new { message = "Неверный логин или пароль" });
                }

                return Results.Ok(new { email = user.Email });
            });

            // app.MapDelete("/users/{id}", (int id, UserDbContext db) =>
            // {
            //     DbSet<User>? users = db.User;
            //     var userEntity = db.User.SingleOrDefault(p => p.Id == id);
            //     if (userEntity == null)
            //         return Results.NotFound();

            //     users.Remove(userEntity);
            //     db.SaveChanges();
            //     return Results.NoContent();
            // });

            // app.MapPut("/users/{id}", (int id, User user, UserDbContext db) =>
            // {
            //     db.Entry(user).State = EntityState.Modified;
            //     db.SaveChanges();
            //     return Results.Ok(user);
            // });


            app.MapGet("/getHistory", async (HttpContext context, UserDbContext db) =>
            {
                var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
                var historyPath = Path.Combine(dataFolder, "history.json");
                // var history = 

                try
                {
                    if (!Directory.Exists(dataFolder))
                    {
                        Directory.CreateDirectory(dataFolder);
                    }

                    if (!File.Exists(historyPath))
                    {
                        await File.WriteAllTextAsync(historyPath, "[]");
                    }

                    var jsonData = await File.ReadAllTextAsync(historyPath);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonData);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500; // Internal Server Error
                    await context.Response.WriteAsync($"Error reading history: {ex.Message}");
                }
            });

            app.MapPost("/saveHistory", async (HttpContext context, UserDbContext db, User curUser) =>
            {
                var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
                var historyPath = Path.Combine(dataFolder, "history.json");

                // Убедимся, что папка data существует
                if (!Directory.Exists(dataFolder))
                {
                    Directory.CreateDirectory(dataFolder);
                }

                using var reader = new StreamReader(context.Request.Body);
                var content = await reader.ReadToEndAsync();

                // Сохранение данных в файл
                await File.WriteAllTextAsync(historyPath, content);
                // curUser.History = 
                context.Response.StatusCode = 200; // Успешный ответ
            });

            app.MapPost("/clearHistory", async context =>
            {
                var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
                var historyPath = Path.Combine(dataFolder, "history.json");

                if (File.Exists(historyPath))
                {
                    File.Delete(historyPath);
                }

                context.Response.StatusCode = 200; // Успешный ответ
            });
        }
    }
}