using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskReservationApp.Infrastructure.Persistance.Migrations.Business
{
    /// <inheritdoc />
    public partial class AddWindowsUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WindowsUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindowsUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WindowsUsers_Email",
                table: "WindowsUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_WindowsUsers_UserName",
                table: "WindowsUsers",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WindowsUsers");
        }
    }
}
