using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApp.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaPrestadorServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alterar PrestadorId para ser nullable na tabela Servicos
            migrationBuilder.AlterColumn<Guid>(
                name: "PrestadorId",
                schema: "public",
                table: "Servicos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // Criar tabela PrestadorServicos
            migrationBuilder.CreateTable(
                name: "PrestadorServicos",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorPersonalizado = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    DuracaoPersonalizadaEmMinutos = table.Column<int>(type: "integer", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestadorServicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestadorServicos_Prestadores_PrestadorId",
                        column: x => x.PrestadorId,
                        principalSchema: "public",
                        principalTable: "Prestadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrestadorServicos_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalSchema: "public",
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Criar índices
            migrationBuilder.CreateIndex(
                name: "IX_PrestadorServico_PrestadorId",
                schema: "public",
                table: "PrestadorServicos",
                column: "PrestadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorServico_ServicoId",
                schema: "public",
                table: "PrestadorServicos",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestadorServico_PrestadorId_ServicoId",
                schema: "public",
                table: "PrestadorServicos",
                columns: new[] { "PrestadorId", "ServicoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remover tabela PrestadorServicos
            migrationBuilder.DropTable(
                name: "PrestadorServicos",
                schema: "public");

            // Reverter PrestadorId para não nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "PrestadorId",
                schema: "public",
                table: "Servicos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
