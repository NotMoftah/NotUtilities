using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NotUtilities.Core.Repository.Interface;
using System;

namespace NotUtilities.Core.Repository.UnitTest.TestFixture
{
    [TestFixture]
    public class TestFixtureBase
    {
        public ServiceProvider ServiceProvider { get; init; }

        public TestFixtureBase()
        {
            var serviceCollection = new ServiceCollection();

            // Configure your DbContext with InMemory provider
            serviceCollection.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
            serviceCollection.AddScoped<DbContext>(serviceCollection => serviceCollection.GetRequiredService<TestDbContext>());

            // Register other services
            serviceCollection.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            serviceCollection.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            // Configure TestFixture
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Initialize database or seed data
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                // Seed database or perform migrations
                context.Database.EnsureCreated();
                
                // Seed some inital random data
                for (int i = 1; i <= 1000; i++)
                {
                    var entity = new TestEntity { Id = i };
                    context.TestEntities.Add(entity);
                }

                context.SaveChanges();
            }
        }
    }
}