using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApp.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class RemoverTabelaUsuarioDesnecessaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remover foreign keys que referenciam a tabela Usuario
            migrationBuilder.DropForeignKey(
                name: "FK_HorariosDisponiveis_Usuario_UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Usuario_UsuarioId",
                schema: "public",
                table: "Servicos");

            // Remover índices relacionados
            migrationBuilder.DropIndex(
                name: "IX_HorariosDisponiveis_UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_UsuarioId",
                schema: "public",
                table: "Servicos");

            // Remover colunas UsuarioId das tabelas
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                schema: "public",
                table: "Servicos");

            // Remover a tabela Usuario completamente
            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recriar tabela Usuario (caso precise fazer rollback)
            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    LojaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaAtuacao = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UltimoAcesso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Lojas_LojaId",
                        column: x => x.LojaId,
                        principalSchema: "public",
                        principalTable: "Lojas",
                        principalColumn: "Id");
                });

            // Recriar colunas UsuarioId
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                schema: "public",
                table: "Servicos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis",
                type: "uuid",
                nullable: true);

            // Recriar índices
            migrationBuilder.CreateIndex(
                name: "IX_Servicos_UsuarioId",
                schema: "public",
                table: "Servicos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDisponiveis_UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_LojaId",
                schema: "public",
                table: "Usuario",
                column: "LojaId");

            // Recriar foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_HorariosDisponiveis_Usuario_UsuarioId",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "UsuarioId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Usuario_UsuarioId",
                schema: "public",
                table: "Servicos",
                column: "UsuarioId",
                principalSchema: "public",
                principalTable: "Usuario",
                principalColumn: "Id");
        }
    }
} 