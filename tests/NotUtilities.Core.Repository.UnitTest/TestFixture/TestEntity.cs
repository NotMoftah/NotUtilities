using NotUtilities.Core.Repository.Interface;

namespace NotUtilities.Core.Repository.UnitTest.TestFixture
{
    public class TestEntity : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public TestEntity()
        {
            Name = $"name-{Guid.NewGuid()}";
        }
    }
}
