using Microsoft.EntityFrameworkCore;
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
    public class TransactionalRepositoryIntegrationTest : TestFixtureBase
    {
        [Test]
        public async Task Entities_WhenAccessed_ReturnsQueryableCollection()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var queryable = repository.Entities;

            // Assert
            Assert.That(queryable, Is.Not.Null, "Entities collection should not be null.");
            Assert.That(queryable, Is.InstanceOf<IQueryable<TestEntity>>(), "Entities should be an instance of IQueryable<TestEntity>.");

            // Additional checks to ensure the collection behaves as expected
            try
            {
                var count = queryable.Count(); // This is to trigger any potential exception that might arise from querying the database.
                Assert.That(count, Is.GreaterThanOrEqualTo(0), "Entities collection should be able to execute a query against the database without errors.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Accessing the entities collection should not throw an exception. Exception: {ex.Message}");
            }
        }

        [Test]
        public async Task FindByIdAsync_WhenKeyExist_ReturnsCorrectData()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var entity = await repository.FindByIdAsync(1);

            // Assert
            Assert.That(entity, Is.Not.Null, "Entity should not be null.");
            Assert.That(entity.Id, Is.EqualTo(1), "Entity ID should match the requested ID.");
            Assert.That(entity.Name, Is.Not.Null.And.Not.Empty.And.StartsWith("name-"), "Entity name should not be null or empty and should start with 'name-'.");
        }

        [Test]
        public async Task FindByIdAsync_WhenNoKeyExist_ReturnsNull()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var entity = await repository.FindByIdAsync(-1);

            // Assert
            Assert.That(entity, Is.Null, "Entity should be null when a non-existing ID is queried.");
        }

        [Test]
        public async Task FindAsync_WhenPredicateMatches_ReturnsCorrectEntity()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var expectedName = "name-";

            // Act
            var entity = await repository.FindAsync(x => x.Name.StartsWith(expectedName));

            // Assert
            Assert.That(entity, Is.Not.Null, "Expected non-null entity when the predicate matches an entity.");
        }

        [Test]
        public async Task FindAsync_WhenPredicateDoesNotMatch_ReturnsNull()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var entity = await repository.FindAsync(x => x.Name == "NonExistentName");

            // Assert
            Assert.That(entity, Is.Null, "Expected null entity when the predicate does not match any entity.");
        }

        [Test]
        public async Task FindByIdsAsync_WhenKeysExist_ReturnsCorrectData()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();
            var expectedIds = new List<int> { 1, 2, 3, 4, 5, 6, 7 };

            // Act
            var resultEntities = await repository.FindByIdsAsync(expectedIds);

            // Assert
            Assert.IsNotNull(resultEntities, "Result entities should not be null.");
            Assert.That(resultEntities.Count(), Is.EqualTo(expectedIds.Count), "Number of entities should match number of IDs.");
            Assert.IsTrue(resultEntities.Select(e => e.Id).Distinct().Count() == expectedIds.Count, "Entity IDs should be unique.");
            Assert.IsTrue(resultEntities.All(e => e.Name.StartsWith("name-")), "All entity names should start with 'name-'.");
        }

        [Test]
        public async Task FindByIdsAsync_WhenSomeKeysExist_ReturnsCorrectData()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();
            var queryIds = new List<int> { 1, -2, 3, -4, 234535, 6, -73245345 };
            var expectedIds = new List<int> { 1, 3, 6 };

            // Act
            var resultEntities = await repository.FindByIdsAsync(queryIds);

            // Assert
            Assert.IsNotNull(resultEntities, "Result entities should not be null.");
            Assert.That(resultEntities.Count(), Is.EqualTo(expectedIds.Count), "Number of entities should match number of IDs.");
            Assert.IsTrue(resultEntities.Select(e => e.Id).Distinct().Count() == expectedIds.Count, "Entity IDs should be unique.");
            Assert.IsTrue(resultEntities.All(e => e.Name.StartsWith("name-")), "All entity names should start with 'name-'.");
        }

        [Test]
        public async Task FindByIdsAsync_WhenNoKeysExist_ReturnsEmptyIEnumerable()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();
            var queryIds = new List<int> { -1, 32423425, -45645645 };

            // Act
            var resultEntities = await repository.FindByIdsAsync(queryIds);

            // Assert
            Assert.IsNotNull(resultEntities, "Result entities should not be null.");
            Assert.That(resultEntities.Count(), Is.EqualTo(0), "Result should be empty.");
        }

        [Test]
        public async Task FindAllAsync_WhenPredicateMatches_ReturnsCorrectEntities()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var expectedName = "name-";

            // Act
            var entities = await repository.FindAllAsync(x => x.Name.StartsWith(expectedName));

            // Assert
            Assert.That(entities, Is.Not.Empty, "Expected non-empty collection when the predicate matches multiple entities.");
            Assert.That(entities.All(e => e.Name.StartsWith(expectedName)), Is.True, "All returned entities should match the expected name.");
        }

        [Test]
        public async Task FindAllAsync_WhenPredicateDoesNotMatch_ReturnsEmptyCollection()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var entities = await repository.FindAllAsync(x => x.Name == "NonExistentName");

            // Assert
            Assert.That(entities, Is.Empty, "Expected empty collection when the predicate does not match any entities.");
        }

        [Test]
        public async Task SelectAsync_WhenSelectorIsValid_ReturnsProjectedCollection()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            var projections = await repository.SelectAsync(entities => entities.Select(e => new { e.Id, e.Name }));

            // Assert
            Assert.That(projections, Is.Not.Null, "Projection should not be null.");
            Assert.That(projections, Is.All.InstanceOf<object>(), "All projections should be of anonymous object type with Id and Name properties.");
            Assert.That(projections, Is.Not.Empty, "Expected non-empty collection for valid selector.");
        }

        [Test]
        public async Task InsertAsync_WhenEntityIsValid_ReturnsTrue()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntity = new TestEntity { Name = "TestEntity" };

            // Act
            var result = await repository.InsertAsync(newEntity);

            // Assert
            Assert.That(result, Is.True, "Insert should return true for a valid entity.");
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync);
        }

        [Test]
        public async Task InsertAsync_WhenEntityExist_ThrowsArgumentException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntity = new TestEntity {Id = 5, Name = "TestEntity" };

            // Act
            await repository.InsertAsync(newEntity);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(repository.SaveChangesAsync);
        }

        [Test]
        public async Task InsertAsync_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            TestEntity? nullEntity = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.InsertAsync(nullEntity));
        }

        [Test]
        public async Task InsertAsync_MultipleEntities_WhenAllAreValid_ReturnsTrue()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntities = new List<TestEntity>
            {
                new TestEntity { Name = "TestEntity1" },
                new TestEntity { Name = "TestEntity2" }
            };

            // Act
            var result = await repository.InsertAsync(newEntities);

            // Assert
            Assert.That(result, Is.True, "Insert should return true when all entities are valid.");
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync);
        }

        [Test]
        public async Task InsertAsync_MultipleEntities_WhenSomeAreInvalid_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntities = new List<TestEntity>
            {
                new TestEntity { Name = "TestEntity1" },
                new TestEntity { Id = 3, Name = "TestEntity2" },
                new TestEntity { Id = -45, Name = "TestEntity3" }
            };

            // Act
            var result = await repository.InsertAsync(newEntities);

            // Assert
            Assert.That(result, Is.True, "Insert should return true when some entities are invalid.");
            Assert.ThrowsAsync<ArgumentException>(repository.SaveChangesAsync, "Insert should throw ArgumentException when some entities are invalid.");
        }

        [Test]
        public async Task InsertAsync_MultipleEntities_WhenAnyIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntities = new List<TestEntity?>
            {
                new TestEntity { Name = "ValidEntity" },
                null // This null entity should cause the operation to fail.
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.InsertAsync(newEntities), "Insert should throw ArgumentNullException when any entity in the collection is null.");
        }

        [Test]
        public async Task UpdateAsync_WhenEntityExists_ReturnsTrue()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var entityToUpdate = new TestEntity { Id = 1, Name = "UpdatedName" };

            // Act
            var result = await repository.UpdateAsync(entityToUpdate);

            // Assert
            Assert.That(result, Is.True, "Update should return true when the entity exists.");
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync);
        }

        [Test]
        public async Task UpdateAsync_WhenEntityDoesNotExist_ThrowsDbUpdateConcurrencyException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var nonExistingEntity = new TestEntity { Id = 349999, Name = "NonExisting" };

            // Act
            var result = await repository.UpdateAsync(nonExistingEntity);

            // Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(repository.SaveChangesAsync, "Update should return throw when the entity does not exist.");
        }

        [Test]
        public async Task UpdateAsync_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            TestEntity? nullEntity = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.UpdateAsync(nullEntity), "Update should throw an ArgumentNullException when the entity is null.");
        }

        [Test]
        public async Task UpdateAsync_MultipleEntities_WhenAllExist_ReturnsTrue()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var existingEntities = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Entity1Updated" },
                new TestEntity { Id = 2, Name = "Entity2Updated" }
            };

            // Act
            var result = await repository.UpdateAsync(existingEntities);

            // Assert
            Assert.That(result, Is.True, "Update should return true when all entities exist and are updated successfully.");
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync);
        }

        [Test]
        public async Task UpdateAsync_MultipleEntities_WhenAnyDoesNotExist_ThrowsDbUpdateConcurrencyException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var mixedEntities = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "ValidEntity" },
                new TestEntity { Id = 99999, Name = "InvalidEntity" }
            };

            // Act
            var result = await repository.UpdateAsync(mixedEntities);

            // Assert
            Assert.That(result, Is.True, "Update should return true when any entity in the collection does not exist.");
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(repository.SaveChangesAsync, "Update should return throw DbUpdateConcurrencyException when any entity in the collection does not exist.");
        }

        [Test]
        public async Task DeleteAsync_WhenEntityIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            TestEntity? nullEntity = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.DeleteAsync(nullEntity), "Delete should throw an ArgumentNullException when the entity is null.");
        }

        [Test]
        public async Task DeleteAsync_MultipleEntities_WhenAllExist_ReturnsTrue()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var entitiesToDelete = new List<TestEntity>
            {
                new TestEntity { Id = 100 },
                new TestEntity { Id = 200 }
            };

            // Act
            var result = await repository.DeleteAsync(entitiesToDelete);

            // Assert
            Assert.That(result, Is.True, "Delete should return true when all entities exist and are deleted successfully.");
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync);
        }

        [Test]
        public async Task DeleteAsync_MultipleEntities_WhenAnyDoesNotExist_ThrowDbUpdateConcurrencyException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var mixedEntities = new List<TestEntity>
            {
                new TestEntity { Id = 100 },
                new TestEntity { Id = 995999 }
            };

            // Act
            var result = await repository.DeleteAsync(mixedEntities);

            // Assert
            Assert.That(result, Is.True, "Delete should return true when any entity in the collection does not exist.");
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(repository.SaveChangesAsync, "Delete should throw DbUpdateConcurrencyException when any entity in the collection does not exist.");
        }

        [Test]
        public async Task DeleteAsync_MultipleEntities_WhenAnyIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var entities = new List<TestEntity?>
            {
                new TestEntity { Id = 100 },
                null
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.DeleteAsync(entities.Cast<TestEntity>()), "Delete should throw an ArgumentNullException when any entity in the collection is null.");
        }

        [Test]
        public async Task SaveChangesAsync_WhenChangesExist_CompletesSuccessfully()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            var newEntity = new TestEntity { Name = "TempEntity" };
            await repository.InsertAsync(newEntity);

            // Act & Assert
            Assert.DoesNotThrowAsync(repository.SaveChangesAsync, "Saving changes should complete successfully without throwing exceptions.");
        }

        [Test]
        public async Task SaveChangesAsync_WhenNoChangesExist_CompletesSuccessfully()
        {
            // Arrange
            await using var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await repository.SaveChangesAsync(), "Saving without changes should complete successfully without throwing exceptions.");
        }

        [Test]
        public async Task AsyncDispose_WhenCalled_ReleasesResources()
        {
            // Arrange
            var scope = ServiceProvider.CreateAsyncScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITransactionalRepository<TestEntity, int>>();

            // Act
            await repository.DisposeAsync();

            // This test requires custom verification to ensure resources are released. This could involve checking the state of the repository,
            // but specifics depend on the implementation details of ITransactionalRepository and might not be directly testable.
            Assert.Pass("Resource release upon AsyncDispose call should be verified according to the specific logic and resource management strategy of ITransactionalRepository.");
        }


    }
}
