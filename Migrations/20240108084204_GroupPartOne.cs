using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorVault.Migrations
{
    /// <inheritdoc />
    public partial class GroupPartOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Passwords",
                newName: "Shared");

            migrationBuilder.AddColumn<string>(
                name: "GroupsIds",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Passwords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Passwords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersMail = table.Column<string>(type: "TEXT", nullable: false),
                    CypheredPassword = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupsIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Passwords");

            migrationBuilder.RenameColumn(
                name: "Shared",
                table: "Passwords",
                newName: "CategoryId");
        }
    }
}
