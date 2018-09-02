using Microsoft.EntityFrameworkCore.Migrations;

namespace robstagram.Migrations
{
    public partial class ImagesChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Images");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Images",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Images",
                nullable: false,
                defaultValue: 0);
        }
    }
}
