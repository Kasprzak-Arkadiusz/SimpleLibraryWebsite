using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Data.Migrations
{
    public partial class TestMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1,
                columns: new[] { "AddingDate", "IsBorrowed" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookID", "AddingDate", "Author", "Genre", "IsBorrowed", "ReaderID", "Title" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Adam Mickiewicz", 8, false, null, "Ode to Youth" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1,
                columns: new[] { "AddingDate", "IsBorrowed" },
                values: new object[] { new DateTime(2021, 7, 14, 16, 43, 5, 727, DateTimeKind.Local).AddTicks(7249), true });
        }
    }
}
