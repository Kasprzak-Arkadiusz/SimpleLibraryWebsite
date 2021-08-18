using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleLibraryWebsite.Migrations
{
    public partial class RemoveNumberOfUpvotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number of upvotes",
                schema: "Identity",
                table: "Requests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Number of upvotes",
                schema: "Identity",
                table: "Requests",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);
        }
    }
}
