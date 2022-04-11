using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace task_management_system.Data.Migrations
{
    public partial class AddMoneySpentToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MoneySpent",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneySpent",
                table: "Projects");
        }
    }
}
