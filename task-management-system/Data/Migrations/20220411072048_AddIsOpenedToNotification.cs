using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace task_management_system.Data.Migrations
{
    public partial class AddIsOpenedToNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOpened",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpened",
                table: "Notifications");
        }
    }
}
