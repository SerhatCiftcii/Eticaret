using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationCodeToAppUsersNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "AppUsers",
                type: "varchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid", "VerificationCode" },
                values: new object[] { new DateTime(2025, 3, 9, 21, 25, 57, 6, DateTimeKind.Local).AddTicks(2299), new Guid("f301da6b-13a2-43bc-9f95-7cbf23fa7c44"), null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 3, 9, 21, 5, 15, 150, DateTimeKind.Local).AddTicks(4116), new Guid("94cc995b-c933-4ecf-96b2-862ddc210b5d") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 3, 9, 21, 5, 15, 150, DateTimeKind.Local).AddTicks(6721));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 3, 9, 21, 5, 15, 150, DateTimeKind.Local).AddTicks(6729));
        }
    }
}
