using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickChat.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPrivateChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Chats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
