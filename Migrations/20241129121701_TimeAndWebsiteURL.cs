using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMTestFlow.Migrations
{
    /// <inheritdoc />
    public partial class TimeAndWebsiteURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteURL",
                table: "Templates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaitInterval",
                table: "BuildingBlocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteURL",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "WaitInterval",
                table: "BuildingBlocks");
        }
    }
}
