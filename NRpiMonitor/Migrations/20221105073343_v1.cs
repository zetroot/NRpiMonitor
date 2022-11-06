using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRpiMonitor.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PingResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Host = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentPackets = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceivedPackets = table.Column<int>(type: "INTEGER", nullable: false),
                    MinRtt = table.Column<double>(type: "REAL", nullable: false),
                    AvgRtt = table.Column<double>(type: "REAL", nullable: false),
                    MaxRtt = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PingResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PingResults_Host",
                table: "PingResults",
                column: "Host");

            migrationBuilder.CreateIndex(
                name: "IX_PingResults_Timestamp",
                table: "PingResults",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PingResults");
        }
    }
}
