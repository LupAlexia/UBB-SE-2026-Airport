using AirportApp.ClassLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;

namespace AirportApp.Tests.IntegrationTests;

public abstract class RepositoryTestBase
{
    protected AppDbContext DbContext { get; private set; } = null!;

    [SetUp]
    public virtual void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryAirportRepoTestDb_" + Guid.NewGuid().ToString())
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    [TearDown]
    public virtual void TearDown()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}
