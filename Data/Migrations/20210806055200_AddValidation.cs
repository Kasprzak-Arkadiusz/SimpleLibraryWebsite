using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Migrations
{
    public partial class AddValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Readers_ReaderID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Readers_ReaderID",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Readers_ReaderID",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Books_ReaderID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReaderID",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "ReaderID",
                table: "Requests",
                newName: "ReaderId");

            migrationBuilder.RenameColumn(
                name: "RequestID",
                table: "Requests",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_ReaderID",
                table: "Requests",
                newName: "IX_Requests_ReaderId");

            migrationBuilder.RenameColumn(
                name: "ReaderID",
                table: "Readers",
                newName: "ReaderId");

            migrationBuilder.RenameColumn(
                name: "ReaderID",
                table: "Loans",
                newName: "ReaderId");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "Loans",
                newName: "BookId");

            migrationBuilder.RenameColumn(
                name: "LoanID",
                table: "Loans",
                newName: "LoanId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_ReaderID",
                table: "Loans",
                newName: "IX_Loans_ReaderId");

            migrationBuilder.RenameColumn(
                name: "BookID",
                table: "Books",
                newName: "BookId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Requests",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Requests",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Readers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Readers",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Genre",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Readers_ReaderId",
                table: "Loans",
                column: "ReaderId",
                principalTable: "Readers",
                principalColumn: "ReaderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Readers_ReaderId",
                table: "Requests",
                column: "ReaderId",
                principalTable: "Readers",
                principalColumn: "ReaderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Readers_ReaderId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Readers_ReaderId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookId",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "ReaderId",
                table: "Requests",
                newName: "ReaderID");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Requests",
                newName: "RequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_ReaderId",
                table: "Requests",
                newName: "IX_Requests_ReaderID");

            migrationBuilder.RenameColumn(
                name: "ReaderId",
                table: "Readers",
                newName: "ReaderID");

            migrationBuilder.RenameColumn(
                name: "ReaderId",
                table: "Loans",
                newName: "ReaderID");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Loans",
                newName: "BookID");

            migrationBuilder.RenameColumn(
                name: "LoanId",
                table: "Loans",
                newName: "LoanID");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_ReaderId",
                table: "Loans",
                newName: "IX_Loans_ReaderID");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Books",
                newName: "BookID");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "Readers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Readers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<int>(
                name: "Genre",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AddColumn<int>(
                name: "ReaderID",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_ReaderID",
                table: "Books",
                column: "ReaderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Readers_ReaderID",
                table: "Books",
                column: "ReaderID",
                principalTable: "Readers",
                principalColumn: "ReaderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Readers_ReaderID",
                table: "Loans",
                column: "ReaderID",
                principalTable: "Readers",
                principalColumn: "ReaderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Readers_ReaderID",
                table: "Requests",
                column: "ReaderID",
                principalTable: "Readers",
                principalColumn: "ReaderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
