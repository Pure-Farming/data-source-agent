using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pfDataSource.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddTempFilesPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempFilesPath",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempFilesPath",
                table: "SourceConfigurations");
        }
    }
}
