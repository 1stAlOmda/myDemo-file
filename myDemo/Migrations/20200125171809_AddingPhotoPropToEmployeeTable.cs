using Microsoft.EntityFrameworkCore.Migrations;

namespace myDemo.Migrations
{
    public partial class AddingPhotoPropToEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoName",
                table: "Employee",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoName",
                table: "Employee");
        }
    }
}
