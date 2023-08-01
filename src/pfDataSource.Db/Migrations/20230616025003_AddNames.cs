using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pfDataSource.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PureFarmingFullSourceName",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PureFarmingSourceName",
                table: "SourceConfigurations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PureFarmingFullSourceName",
                table: "SourceConfigurations");

            migrationBuilder.DropColumn(
                name: "PureFarmingSourceName",
                table: "SourceConfigurations");
        }
    }
}
