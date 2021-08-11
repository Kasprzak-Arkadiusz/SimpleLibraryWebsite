using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Migrations
{
    public partial class FixTypoInLoanFirstName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date of adding",
                schema: "Identity",
                table: "Loans",
                newName: "Lent from");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lent from",
                schema: "Identity",
                table: "Loans",
                newName: "Date of adding");
        }
    }
}
