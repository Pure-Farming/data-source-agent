
using FluentAssertions;
using System.Text;
using System.Reflection;
using pfDataSource.Services.Models;
using System.ComponentModel.DataAnnotations;
using Castle.Core.Internal;
using System.Text.RegularExpressions;

namespace pfDataSource.Test
{
    public class FileConfigurationModelRegexTests
    {

        [Fact]
        public void FileConfigurationModelRegexTests_Validate_Path_Regex()
        {
            var pathRegex = typeof(FileConfiguration).GetProperty("Path").GetAttribute<RegularExpressionAttribute>();

            var regexPattern = pathRegex.Pattern;

            Regex.Match("C:\\test\\dir", regexPattern).Success.Should().BeTrue();
            Regex.Match("/test/dir", regexPattern).Success.Should().BeTrue();

        }

        [Fact]
        public void FileConfigurationModelRegexTests_Validate_Correct_CronExpressions()
        {
            var pathRegex = typeof(FileConfiguration).GetProperty("CronExpression").GetAttribute<RegularExpressionAttribute>();

            var regexPattern = pathRegex.Pattern;

            var expressions = new List<string>
            {
                "* * * * *",
                "0 0 12 * *",
                "0 15 10 ? *",
                "0 * 14 * * ",
                "0 0/5 14 * *",
                "0 0-5 14 * *",
                "0 0 12 1/5 *",
                "0 0-5 14 * *"
            };

            expressions.ForEach(expression => { Regex.Match(expression, regexPattern).Success.Should().BeTrue(); });

        }

        [Fact]
        public void FileConfigurationModelRegexTests_Validate_InCorrect_CronExpressions()
        {
            var pathRegex = typeof(FileConfiguration).GetProperty("CronExpression").GetAttribute<RegularExpressionAttribute>();

            Regex.Match("* * * * * *", pathRegex.Pattern).Success.Should().BeFalse(); //to many params, 6
            Regex.Match("* * * *", pathRegex.Pattern).Success.Should().BeFalse(); //to few params, 4
            Regex.Match("some-junk", pathRegex.Pattern).Success.Should().BeFalse();

        }
    }
}