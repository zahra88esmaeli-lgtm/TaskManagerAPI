using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerAPI.Data;

namespace TaskManagerAPI.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                  
                    ["Jwt:Key"] = "cdy456sdj876ijy345mnb123okdtoi88"
                });
            });
            builder.ConfigureServices(services =>
            {

                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDBContext>) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDBContext>))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<AppDBContext>(options =>
                    options.UseInMemoryDatabase(_dbName));
            });
        }
    }
}
