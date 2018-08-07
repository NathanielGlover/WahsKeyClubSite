using Microsoft.EntityFrameworkCore.Migrations;

namespace WahsKeyClubSite.Migrations.ServiceHoursDb
{
    public partial class FixHourLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ServiceHours",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ServiceHours");
        }
    }
}
