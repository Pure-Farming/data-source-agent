
using FluentAssertions;
using System.Text;
using pfDataSource.Common;
using pfDataSource.Common.Configuration;
using pfDataSource.Services;
using Moq;
using pfDataSource.Db;
using pfDataSource.Db.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.DataProtection;
using pfDataSource.Services.Models;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace pfDataSource.Test
{
    public class DataSourceConfigurationServiceTests
    {
        private SourceConfiguration _sourceConfiguration;
        private FileConfiguration _fileConfiguration;

        private Services.Models.DataSourceConfiguration _dataSourceConfiguration;

        public DataSourceConfigurationServiceTests() {

            _sourceConfiguration = new SourceConfiguration
            {
                Id = 1,
                SourceType = "test-type",
                DisplayType = "test",
                DisplayName = "Test",
                TempFilesPath = "/local/dir",
                PureFarmingSourceName = "Test Source",
                PureFarmingFullSourceName = "Full Test Source",
                AwsSecrectId = "123456789",
                AwsSecretKey = "some-secret-key",
                AwsS3BucketArn = "aws::arn"
            };

            _dataSourceConfiguration = new Services.Models.DataSourceConfiguration
            {
                DisplayName = "Test",
                Name = "Test Source",
                FullName = "Full Test Source",
                SourceType = "test-type",
                DisplayType = "test",
                TempFilesPath = "/local/dir"
            };


            _fileConfiguration = new FileConfiguration
            {
                Path = "C:\\test\\dir",
                FileType = "CSV",
                SubmissionDelay = 0,
                WatchDirectory = false,
                CronExpression = "* * * * *"
            };

        }


        [Fact]
        public void DataSourceConfigurationServiceTests_BuildConfigurationObject_Returns_Valid_SourceConfiguration()
        {
           
            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildConfigurationObject(_sourceConfiguration);

            result.Should().NotBeNull();

            result.SourceType.Should().Be("test-type");
            result.DisplayType.Should().Be("test");
            result.DisplayName.Should().Be("Test");
            result.TempFilesPath.Should().Be("/local/dir");
            result.Name.Should().Be("Test Source");
            result.FullName.Should().Be("Full Test Source");
            result.Configuration.Should().BeNull();

        }

        [Fact]
        public void DataSourceConfigurationServiceTests_BuildConfigurationObject_Returns_Valid_SourceConfiguration_With_Configuration()
        {

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            // Test with configuration object
            _sourceConfiguration.Configuration = JsonConvert.SerializeObject(_fileConfiguration);

            var result = dataSourceConfigurationServiceMock.BuildConfigurationObject(_sourceConfiguration);

            result.Should().NotBeNull();

            result.Configuration.Should().NotBeNull();

            JsonConvert.SerializeObject(result.Configuration).Should().Be(_sourceConfiguration.Configuration);

        }

            [Fact]
        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Update()
        {
    
            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration);

            result.Should().NotBeNull();

            result.Id.Should().Be(1);
            result.SourceType.Should().Be("test-type");
            result.DisplayType.Should().Be("test");
            result.DisplayName.Should().Be("Test");
            result.TempFilesPath.Should().Be("/local/dir");
            result.PureFarmingSourceName.Should().Be("Test Source");
            result.PureFarmingFullSourceName.Should().Be($"com.purefarming.data-source.Full Test Source");
            result.AwsSecrectId.Should().Be("123456789");
            result.AwsSecretKey.Should().Be("some-secret-key");
            result.AwsS3BucketArn.Should().Be("aws::arn");
            result.Configuration.Should().BeNull();

        }

        [Fact]
        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Update_With_Configuration()
        { 

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            _dataSourceConfiguration.Configuration = _fileConfiguration;

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration);

            result.Should().NotBeNull();

            result.Configuration.Should().Be(_sourceConfiguration.Configuration);

        }

        [Fact]

        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Create()
        {

            // Reset SourceConfiguration for create testing
            _sourceConfiguration = new SourceConfiguration();

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration);

            result.Should().NotBeNull();

            result.SourceType.Should().Be("test-type");
            result.DisplayType.Should().Be("test");
            result.DisplayName.Should().Be("Test");
            result.TempFilesPath.Should().Be("/local/dir");
            result.PureFarmingSourceName.Should().Be("Test Source");
            result.PureFarmingFullSourceName.Should().Be($"com.purefarming.data-source.Full Test Source");
            result.AwsSecrectId.Should().Be("123456789");
            result.AwsSecretKey.Should().Be("some-secret-key");
            result.AwsS3BucketArn.Should().Be("aws::arn");
            result.Configuration.Should().BeNull();

        }

        [Fact]

        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Create_with_Configuration()
        {

            // Reset SourceConfiguration for create testing
            _sourceConfiguration = new SourceConfiguration();

            _dataSourceConfiguration.Configuration = _fileConfiguration;

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration);

            result.Should().NotBeNull();

            result.Configuration.Should().Be(_sourceConfiguration.Configuration);

        }

    }
}