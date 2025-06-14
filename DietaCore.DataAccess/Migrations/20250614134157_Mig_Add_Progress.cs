using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DietaCore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Mig_Add_Progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    BodyFatPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    MuscleMass = table.Column<decimal>(type: "numeric", nullable: true),
                    WaistCircumference = table.Column<decimal>(type: "numeric", nullable: true),
                    ChestCircumference = table.Column<decimal>(type: "numeric", nullable: true),
                    HipCircumference = table.Column<decimal>(type: "numeric", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordedByDietitianId = table.Column<int>(type: "integer", nullable: true),
                    IsClientEntry = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientProgresses_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientProgresses_Dietitians_RecordedByDietitianId",
                        column: x => x.RecordedByDietitianId,
                        principalTable: "Dietitians",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProgresses_ClientId",
                table: "ClientProgresses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProgresses_RecordedByDietitianId",
                table: "ClientProgresses",
                column: "RecordedByDietitianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientProgresses");
        }
    }
}
