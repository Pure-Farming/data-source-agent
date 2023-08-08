
using FluentAssertions;
using System.Text;
using pfDataSource.Common;

namespace pfDataSource.Test
{
    public class ObjectExtenstionsTest
    {
        [Fact]
        public void ObjectExtenstionsTest_GetProperties_Returns_Valid_Properties()
        {

            var obj = new { Id = 1, FirstName = "James", LastName = "Bond" };

            var fullProps = obj.GetProperties().ToList();

            fullProps.Should().NotBeNull();

            fullProps[0].Should().Be("Id");
            fullProps[1].Should().Be("FirstName");
            fullProps[2].Should().Be("LastName");
        }

        [Fact]
        public void ObjectExtenstionsTest_GetPropertyValue_Returns_Valid_Value()
        {

            var obj = new { Id = 1, FirstName = "James", LastName = "Bond" };

            var firstName = obj.GetPropertyValue("Id");
            firstName.Should().NotBeNull();
            firstName.Should().Be(1);
        }
    }
}