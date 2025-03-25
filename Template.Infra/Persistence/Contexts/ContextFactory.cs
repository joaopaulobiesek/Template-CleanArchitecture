using Microsoft.EntityFrameworkCore.Design;
using System.Runtime.InteropServices;

namespace Template.Infra.Persistence.Contexts.Core
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory(); // Diretório atual do projeto

            Console.WriteLine($"Procurando appsettings.json em: {basePath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            string connectionStringKey = isMac ? "Context_MAC" : "Context";

            var connectionString = configuration.GetConnectionString(connectionStringKey);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"ConnectionString '{connectionStringKey}' não encontrada no appsettings.json!");
            }

            Console.WriteLine($"Rodando no {(isMac ? "Mac" : "Windows")} - Usando ConnectionString: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlServer(connectionString);

            return new Context(optionsBuilder.Options);
        }
    }
}