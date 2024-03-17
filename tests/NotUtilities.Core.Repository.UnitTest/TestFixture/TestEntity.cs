using NotUtilities.Core.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
