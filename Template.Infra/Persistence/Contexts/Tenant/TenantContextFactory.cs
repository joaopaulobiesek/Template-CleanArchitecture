using Microsoft.EntityFrameworkCore.Design;
using System.Runtime.InteropServices;

namespace Template.Infra.Persistence.Contexts.Tenant
{
    public class TenantContextFactory : IDesignTimeDbContextFactory<TenantContext>
    {
        public TenantContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory(); // Diretório atual do projeto

            Console.WriteLine($"Procurando appsettings.json em: {basePath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            string connectionStringKey = isMac ? "TenantContext_MAC" : "TenantContext";

            var connectionString = configuration.GetConnectionString(connectionStringKey);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"ConnectionString '{connectionStringKey}' não encontrada no appsettings.json!");
            }

            Console.WriteLine($"Rodando no {(isMac ? "Mac" : "Windows")} - Usando ConnectionString: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<TenantContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TenantContext(optionsBuilder.Options);
        }
    }
}