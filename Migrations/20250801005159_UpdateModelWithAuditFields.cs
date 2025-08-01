using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrilhaApiDesafio.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelWithAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Tarefas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Tarefas",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Data",
                table: "Tarefas",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_IsDeleted",
                table: "Tarefas",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_IsDeleted_DataCriacao",
                table: "Tarefas",
                columns: new[] { "IsDeleted", "DataCriacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Status",
                table: "Tarefas",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Data",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_IsDeleted",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_IsDeleted_DataCriacao",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Status",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tarefas");
        }
    }
}
