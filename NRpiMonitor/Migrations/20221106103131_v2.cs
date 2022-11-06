using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRpiMonitor.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeedtestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadBandwidth = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadBandwidth = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeedtestResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeedtestResults_Timestamp",
                table: "SpeedtestResults",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeedtestResults");
        }
    }
}
