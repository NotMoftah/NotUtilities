using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotUtilities.Core.Repository.UnitTest.TestFixture
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
