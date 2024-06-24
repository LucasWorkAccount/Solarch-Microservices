﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Medical_Record_System.Migrations
{
    /// <inheritdoc />
    public partial class addedQuestionnaires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "questionnaires",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    question = table.Column<string>(type: "text", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false),
                    inserted_at = table.Column<DateTime>(type: "timestamp(6) without time zone", nullable: false, defaultValueSql: "statement_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("questionnaire_pkey", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questionnaires");
        }
    }
}
