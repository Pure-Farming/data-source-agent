
using FluentAssertions;
using System.Text;

namespace pfDataSource.Test
{
    public class CsvBuilderTests
    {

        private readonly List<object> exampleData = null;

        public CsvBuilderTests() {

            exampleData = new List<object>
            {
                new { Id = 1, FirstName = "James", LastName = "Bond" },
                new { Id = 2, FirstName = "Jason", LastName = "Bourne" }
            };
        }


        [Fact]
        public void CsvBuilder_Produces_Valid_String()
        {

            var builder = new Common.CsvBuilder<object>();
            builder.Build(exampleData);
            var result = builder.GetString().ToString().Replace("\"", "");

            var strings = result.Split(Environment.NewLine);

            strings.Should().NotBeNull();

            var header = strings[0].Split(",");

            header[0].Should().Be("Id");
            header[1].Should().Be("FirstName");
            header[2].Should().Be("LastName");

            var data1 = strings[1].Split(",");
            var data2 = strings[2].Split(",");

            data1[0].Should().Be("1");
            data1[1].Should().Be("James");
            data1[2].Should().Be("Bond");

            data2[0].Should().Be("2");
            data2[1].Should().Be("Jason");
            data2[2].Should().Be("Bourne");

        }
    }
}