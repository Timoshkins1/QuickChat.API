using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickChat.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Users",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "AvatarUrl");
        }
    }
}
