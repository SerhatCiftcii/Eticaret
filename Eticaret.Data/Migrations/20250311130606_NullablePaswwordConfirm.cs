using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class NullablePaswwordConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 3, 11, 16, 6, 6, 88, DateTimeKind.Local).AddTicks(8827), new Guid("b1215482-c8d0-4fa3-8760-e351051bc5bf") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 3, 11, 16, 6, 6, 89, DateTimeKind.Local).AddTicks(1646));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 3, 11, 16, 6, 6, 89, DateTimeKind.Local).AddTicks(1653));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 3, 9, 21, 25, 57, 6, DateTimeKind.Local).AddTicks(2299), new Guid("f301da6b-13a2-43bc-9f95-7cbf23fa7c44") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 3, 9, 21, 25, 57, 6, DateTimeKind.Local).AddTicks(4452));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 3, 9, 21, 25, 57, 6, DateTimeKind.Local).AddTicks(4469));
        }
    }
}
