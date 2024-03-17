using Microsoft.Extensions.DependencyInjection;
using NotUtilities.Core.Repository.Interface;
using NotUtilities.Core.Repository.UnitTest.TestFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotUtilities.Core.Repository.UnitTest
{
    public class UnitOfWorkIntegrationTest : TestFixtureBase
    {
        [Test]
        public async Task FindByIdAsync_WhenKeyExist_ReturnsCorrectData()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<TestEntity, int>();

            // Act
            var entity = await repository.FindByIdAsync(1);

            // Assert
            Assert.That(entity, Is.Not.Null, "Entity should not be null.");
            Assert.That(entity.Id, Is.EqualTo(1), "Entity ID should match the requested ID.");
            Assert.That(entity.Name, Is.Not.Null.And.Not.Empty.And.StartsWith("name-"), "Entity name should not be null or empty and should start with 'name-'.");
        }

    }
}
