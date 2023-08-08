
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

namespace pfDataSource.Test
{
    public class DataSourceConfigurationServiceTests
    {
        private SourceConfiguration _sourceConfiguration;    

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
                TempFilesPath = "/local/dir",
                Aws = new Services.Models.DataSourceConfiguration.AwsConfiguration
                {
                    S3BucketArn = "aws::arn",
                    SecretId = "123456789",
                    SecretKey = "some-secret-key"
                }
            };

        }


        [Fact]
        public void DataSourceConfigurationServiceTests_BuildConfigurationObject_Returns_Valid_SourceConfiguration()
        {
           
            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildConfigurationObject(_sourceConfiguration, _sourceConfiguration.AwsSecrectId, _sourceConfiguration.AwsSecretKey);

            result.Should().NotBeNull();

            result.SourceType.Should().Be("test-type");
            result.DisplayType.Should().Be("test");
            result.DisplayName.Should().Be("Test");
            result.TempFilesPath.Should().Be("/local/dir");
            result.Name.Should().Be("Test Source");
            result.FullName.Should().Be("Full Test Source");
            result.Aws.S3BucketArn.Should().Be("aws::arn");
            result.Aws.SecretId.Should().Be("123456789");
            result.Aws.SecretKey.Should().Be("some-secret-key");
            result.Configuration.Should().BeNull();

        }

        [Fact]
        public void DataSourceConfigurationServiceTests_BuildConfigurationObject_Returns_Valid_SourceConfiguration_With_Configuration()
        {

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            // Test with configuration object
            _sourceConfiguration.Configuration = JsonConvert.SerializeObject(new { prop1 = "value1", prop2 = "value2" });

            var result = dataSourceConfigurationServiceMock.BuildConfigurationObject(_sourceConfiguration, _sourceConfiguration.AwsSecrectId, _sourceConfiguration.AwsSecretKey, typeof(object));

            result.Should().NotBeNull();

            result.Configuration.Should().NotBeNull();

            JsonConvert.SerializeObject(result.Configuration).Should().Be(_sourceConfiguration.Configuration);

        }

            [Fact]
        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Update()
        {
    
            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration, _sourceConfiguration.AwsSecrectId, _sourceConfiguration.AwsSecretKey);

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

            _dataSourceConfiguration.Configuration = new { prop1 = "value1", prop2 = "value2" };

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration, _sourceConfiguration.AwsSecrectId, _sourceConfiguration.AwsSecretKey);

            result.Should().NotBeNull();

            result.Configuration.Should().Be(_sourceConfiguration.Configuration);


        }

        [Fact]

        public void DataSourceConfigurationServiceTests_BuildSourceObject_Returns_Valid_SourceConfiguration_For_Create()
        {

            // Reset SourceConfiguration for create testing
            _sourceConfiguration = new SourceConfiguration();

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration, "123456789", "some-secret-key");

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

            _dataSourceConfiguration.Configuration = new { prop1 = "value1", prop2 = "value2" };

            var dataSourceConfigurationServiceMock = new DataSourceConfigurationService();

            var result = dataSourceConfigurationServiceMock.BuildSourceObject(_sourceConfiguration, _dataSourceConfiguration, "123456789", "some-secret-key");

            result.Should().NotBeNull();

            result.Configuration.Should().Be(_sourceConfiguration.Configuration);

        }

    }
}