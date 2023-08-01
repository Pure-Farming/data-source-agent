using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pfDataSource.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddAwsDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwsS3BucketArn",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AwsSecrectId",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AwsSecretKey",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsS3BucketArn",
                table: "SourceConfigurations");

            migrationBuilder.DropColumn(
                name: "AwsSecrectId",
                table: "SourceConfigurations");

            migrationBuilder.DropColumn(
                name: "AwsSecretKey",
                table: "SourceConfigurations");
        }
    }
}
