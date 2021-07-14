using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Data.Migrations
{
    public partial class TestMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookID", "AddingDate", "Author", "Genre", "IsBorrowed", "ReaderID", "Title" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adam Mickiewicz", 8, false, null, "Master Thaddeus" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookID", "AddingDate", "Author", "Genre", "IsBorrowed", "ReaderID", "Title" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adam Mickiewicz", 8, false, null, "Ode to Youth" });
        }
    }
}
