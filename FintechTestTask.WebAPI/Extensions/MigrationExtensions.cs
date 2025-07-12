using FintechTestTask.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FintechTestTask.WebAPI.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
        
        dbContext.Database.Migrate();
    }
}