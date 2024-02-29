using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorVault.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainToPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Passwords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Passwords");
        }
    }
}
