using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Migrations
{
    public partial class AddedRowVersionField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Identity",
                table: "Requests",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Identity",
                table: "Loans",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Identity",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Identity",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Identity",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Identity",
                table: "Books");
        }
    }
}
