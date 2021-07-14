using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Data.Migrations
{
    public partial class ChangeBookToBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Readers_ReaderID",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Book_BookID",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book",
                table: "Book");

            migrationBuilder.RenameTable(
                name: "Book",
                newName: "Books");

            migrationBuilder.RenameIndex(
                name: "IX_Book_ReaderID",
                table: "Books",
                newName: "IX_Books_ReaderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "BookID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Readers_ReaderID",
                table: "Books",
                column: "ReaderID",
                principalTable: "Readers",
                principalColumn: "ReaderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Books_BookID",
                table: "Requests",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Readers_ReaderID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Books_BookID",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "Book");

            migrationBuilder.RenameIndex(
                name: "IX_Books_ReaderID",
                table: "Book",
                newName: "IX_Book_ReaderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book",
                table: "Book",
                column: "BookID");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Readers_ReaderID",
                table: "Book",
                column: "ReaderID",
                principalTable: "Readers",
                principalColumn: "ReaderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Book_BookID",
                table: "Requests",
                column: "BookID",
                principalTable: "Book",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
