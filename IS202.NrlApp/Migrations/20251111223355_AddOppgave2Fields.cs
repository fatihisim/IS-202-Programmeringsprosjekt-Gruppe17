using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IS202.NrlApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOppgave2Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeoJsonData",
                table: "Obstacles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GeometryType",
                table: "Obstacles",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Obstacles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedBy",
                table: "Obstacles",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProcessorComment",
                table: "Obstacles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Obstacles",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Obstacles",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeoJsonData",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "GeometryType",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "ProcessedBy",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "ProcessorComment",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Obstacles");
        }
    }
}
