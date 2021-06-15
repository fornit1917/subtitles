using Microsoft.EntityFrameworkCore.Migrations;

namespace Subtitles.Migrations
{
    public partial class AddTimeColumnToPhrase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Phrases",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Phrases");
        }
    }
}
