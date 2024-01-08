using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVillametroscuadrados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MetroCuadrados",
                table: "Villas",
                newName: "MetrosCuadrados");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 8, 18, 6, 56, 308, DateTimeKind.Local).AddTicks(5585), new DateTime(2024, 1, 8, 18, 6, 56, 308, DateTimeKind.Local).AddTicks(5572) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 8, 18, 6, 56, 308, DateTimeKind.Local).AddTicks(5589), new DateTime(2024, 1, 8, 18, 6, 56, 308, DateTimeKind.Local).AddTicks(5588) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MetrosCuadrados",
                table: "Villas",
                newName: "MetroCuadrados");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 8, 17, 28, 5, 770, DateTimeKind.Local).AddTicks(7603), new DateTime(2024, 1, 8, 17, 28, 5, 770, DateTimeKind.Local).AddTicks(7590) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2024, 1, 8, 17, 28, 5, 770, DateTimeKind.Local).AddTicks(7607), new DateTime(2024, 1, 8, 17, 28, 5, 770, DateTimeKind.Local).AddTicks(7606) });
        }
    }
}
