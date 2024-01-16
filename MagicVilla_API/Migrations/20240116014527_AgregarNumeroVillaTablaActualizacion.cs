using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNumeroVillaTablaActualizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "NumeroVillas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 15, 20, 45, 26, 859, DateTimeKind.Local).AddTicks(2381), new DateTime(2024, 1, 15, 20, 45, 26, 859, DateTimeKind.Local).AddTicks(2371) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 15, 20, 45, 26, 859, DateTimeKind.Local).AddTicks(2385), new DateTime(2024, 1, 15, 20, 45, 26, 859, DateTimeKind.Local).AddTicks(2384) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "NumeroVillas");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 15, 20, 32, 9, 870, DateTimeKind.Local).AddTicks(791), new DateTime(2024, 1, 15, 20, 32, 9, 870, DateTimeKind.Local).AddTicks(781) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 15, 20, 32, 9, 870, DateTimeKind.Local).AddTicks(795), new DateTime(2024, 1, 15, 20, 32, 9, 870, DateTimeKind.Local).AddTicks(794) });
        }
    }
}
