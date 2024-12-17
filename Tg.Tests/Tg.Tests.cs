using Xunit;
using System.Net;
using System.Text;
using System.Text.Json;
using TG.DataBase;
using TG.Attractions;
using TG.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;


namespace TG.Tests
{
    public class ActionsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public ActionsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;

            // Создаем HTTP-клиент на основе тестового хоста
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Используем SQLite In-Memory для тестов
                    services.AddDbContext<UserDbContext>(options =>
                        options.UseSqlite("DataSource=:memory:"));
                    services.AddDbContext<AttractionDbContext>(options =>
                        options.UseSqlite("DataSource=:memory:"));
                });
            }).CreateClient();

            // Инициализация баз данных
            InitializeDatabasesAsync().Wait();
        }

        private async Task InitializeDatabasesAsync()
        {
            // Инициализация UserDbContext
            using var userScope = CreateInMemoryContext<UserDbContext>("UserDb");
            await userScope.Database.OpenConnectionAsync();
            await userScope.Database.EnsureCreatedAsync();
            userScope.User.Add(new User
            {
                Id = 1,
                Username = "testUser",
                Email = "test@mail.com",
                Password = "hashedPwd",
                Salt = "salt"
            });
            await userScope.SaveChangesAsync();

            // Инициализация AttractionDbContext
            using var attractionScope = CreateInMemoryContext<AttractionDbContext>("AttractionDb");
            await attractionScope.Database.OpenConnectionAsync();
            await attractionScope.Database.EnsureCreatedAsync();
            attractionScope.Attraction.Add(new Attraction
            {
                Id = 1,
                Name = "Park",
                Region = "North"
            });
            await attractionScope.SaveChangesAsync();
        }

        private TDbContext CreateInMemoryContext<TDbContext>(string name) where TDbContext : DbContext
        {
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite($"DataSource={name};Mode=Memory;Cache=Shared")
                .Options;

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), options);
        }
    }
}
