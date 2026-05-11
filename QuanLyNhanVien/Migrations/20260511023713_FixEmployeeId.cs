using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuanLyNhanVien.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeId : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql(
        @"ALTER TABLE ""Employees""
          ALTER COLUMN ""Ngay""
          TYPE timestamp with time zone
          USING ""Ngay""::timestamp with time zone;"
    );

    migrationBuilder.AlterColumn<string>(
        name: "MaNhanVien",
        table: "Employees",
        type: "text",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "TEXT");

    migrationBuilder.AlterColumn<double>(
        name: "Longitude",
        table: "Employees",
        type: "double precision",
        nullable: true,
        oldClrType: typeof(float),
        oldType: "REAL",
        oldNullable: true);

    migrationBuilder.AlterColumn<double>(
        name: "Latitude",
        table: "Employees",
        type: "double precision",
        nullable: true,
        oldClrType: typeof(float),
        oldType: "REAL",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "KhuVuc",
        table: "Employees",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "ImagePath",
        table: "Employees",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "HoTen",
        table: "Employees",
        type: "text",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "TEXT");

    migrationBuilder.AlterColumn<string>(
        name: "GhiChu",
        table: "Employees",
        type: "text",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "TEXT",
        oldNullable: true);

    migrationBuilder.AlterColumn<int>(
        name: "Id",
        table: "Employees",
        type: "integer",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "INTEGER")
        .Annotation("Npgsql:ValueGenerationStrategy",
            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ngay",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "MaNhanVien",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Employees",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Employees",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KhuVuc",
                table: "Employees",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Employees",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HoTen",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "Employees",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
