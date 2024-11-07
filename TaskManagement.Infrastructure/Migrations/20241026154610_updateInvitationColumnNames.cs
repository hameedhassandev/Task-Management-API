using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateInvitationColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Invitation",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "Invitation",
                newName: "InvitationStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvitationStatus",
                table: "Invitation",
                newName: "MyProperty");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Invitation",
                newName: "Name");
        }
    }
}
