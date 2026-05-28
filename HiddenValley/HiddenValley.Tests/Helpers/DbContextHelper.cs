using HiddenValley.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HiddenValley.Tests.Helpers;

public static class DbContextHelper
{
    // Cada llamada genera un nombre único garantizando aislamiento total entre tests
    public static ApplicationDbContext CreateInMemory(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"{dbName}_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new ApplicationDbContext(options);
    }
}
