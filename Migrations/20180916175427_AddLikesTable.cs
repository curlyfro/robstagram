using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace robstagram.Migrations
{
    public partial class AddLikesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Entries_EntryId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EntryId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EntryId",
                table: "Customers");

            migrationBuilder.CreateTable(
                name: "Like",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    EntryId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Like", x => new { x.EntryId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_Like_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Like_Entries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "Entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Like_CustomerId",
                table: "Like",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Like");

            migrationBuilder.AddColumn<int>(
                name: "EntryId",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EntryId",
                table: "Customers",
                column: "EntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Entries_EntryId",
                table: "Customers",
                column: "EntryId",
                principalTable: "Entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
