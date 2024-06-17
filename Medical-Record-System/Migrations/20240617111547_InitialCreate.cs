using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Medical_Record_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "jsonb", nullable: false),
                    inserted_at = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false, defaultValueSql: "statement_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("events_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "medical_record",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    sex = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    bsn = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    record = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("medical_record_pk", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "medical_record");
        }
    }
}
