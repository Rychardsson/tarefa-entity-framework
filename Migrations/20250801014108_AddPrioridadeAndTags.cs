using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrilhaApiDesafio.Migrations
{
    /// <inheritdoc />
    public partial class AddPrioridadeAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Prioridade",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Tarefas",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tarefas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Data_Status",
                table: "Tarefas",
                columns: new[] { "Data", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Prioridade",
                table: "Tarefas",
                column: "Prioridade");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_Status_Prioridade",
                table: "Tarefas",
                columns: new[] { "Status", "Prioridade" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_UserId",
                table: "Tarefas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Data_Status",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Prioridade",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_Status_Prioridade",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_UserId",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tarefas");
        }
    }
}
