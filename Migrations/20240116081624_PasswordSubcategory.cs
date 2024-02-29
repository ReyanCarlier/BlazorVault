using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorVault.Migrations
{
    /// <inheritdoc />
    public partial class PasswordSubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubcategoryName",
                table: "Passwords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubcategoryName",
                table: "Passwords");
        }
    }
}
