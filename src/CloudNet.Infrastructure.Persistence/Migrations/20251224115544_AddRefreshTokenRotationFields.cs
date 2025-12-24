using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudNet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenRotationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Device",
                schema: "identity",
                table: "RefreshTokens",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                schema: "identity",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedByTokenId",
                schema: "identity",
                table: "RefreshTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RevokedAt",
                schema: "identity",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "RefreshTokens",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FamilyId",
                schema: "identity",
                table: "RefreshTokens",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                schema: "identity",
                table: "RefreshTokens",
                column: "ReplacedByTokenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_FamilyId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "Device",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ReplacedByTokenId",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                schema: "identity",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "RefreshTokens");
        }
    }
}
