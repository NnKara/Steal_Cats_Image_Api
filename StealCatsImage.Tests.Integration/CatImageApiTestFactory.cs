using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StealCatsImage.Domain.Entities;
using StealCatsImage.Infrastructure.Data;

namespace StealCatsImage.Tests.Integration;

public class CatImageApiTestFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    private static string IntegrationDbName { get; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var existingDbContext = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (existingDbContext != null) services.Remove(existingDbContext);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(IntegrationDbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TheCatApi:BaseUrl"] = "https://api.thecatapi.com/",
                ["TheCatApi:ApiKey"] = "test-key"
            });
        });
    }

    public async Task<int> SeedCatAsync(CatEntity cat)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Cats.Add(cat);
        await db.SaveChangesAsync();
        return cat.Id;
    }
}
