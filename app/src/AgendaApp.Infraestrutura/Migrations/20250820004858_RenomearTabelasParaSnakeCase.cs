using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApp.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class RenomearTabelasParaSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Lojas_LojaId",
                schema: "public",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Prestadores_PrestadorId",
                schema: "public",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                schema: "public",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Lojas_LojaId",
                schema: "public",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HorariosDisponiveis_Lojas_LojaId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_HorariosDisponiveis_Prestadores_PrestadorId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestadores_Lojas_LojaId",
                schema: "public",
                table: "Prestadores");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestadorServicos_Prestadores_PrestadorId",
                schema: "public",
                table: "PrestadorServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_PrestadorServicos_Servicos_ServicoId",
                schema: "public",
                table: "PrestadorServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_AspNetRoles_RoleId",
                schema: "public",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Prestadores_PrestadorId",
                schema: "public",
                table: "Servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioClaims_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioLogins_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRoles_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRoles_AspNetRoles_RoleId",
                schema: "public",
                table: "UsuarioRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioTokens_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servicos",
                schema: "public",
                table: "Servicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "public",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prestadores",
                schema: "public",
                table: "Prestadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lojas",
                schema: "public",
                table: "Lojas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agendamentos",
                schema: "public",
                table: "Agendamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioTokens",
                schema: "public",
                table: "UsuarioTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRoles",
                schema: "public",
                table: "UsuarioRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioLogins",
                schema: "public",
                table: "UsuarioLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioClaims",
                schema: "public",
                table: "UsuarioClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaims",
                schema: "public",
                table: "RoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrestadorServicos",
                schema: "public",
                table: "PrestadorServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HorariosDisponiveis",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUsers",
                schema: "public",
                table: "ApplicationUsers");

            migrationBuilder.RenameTable(
                name: "Servicos",
                schema: "public",
                newName: "servicos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "public",
                newName: "roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Prestadores",
                schema: "public",
                newName: "prestadores",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Lojas",
                schema: "public",
                newName: "lojas",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Agendamentos",
                schema: "public",
                newName: "agendamentos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UsuarioTokens",
                schema: "public",
                newName: "usuario_tokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UsuarioRoles",
                schema: "public",
                newName: "usuario_roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UsuarioLogins",
                schema: "public",
                newName: "usuario_logins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UsuarioClaims",
                schema: "public",
                newName: "usuario_claims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "public",
                newName: "role_claims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "PrestadorServicos",
                schema: "public",
                newName: "prestador_servicos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "HorariosDisponiveis",
                schema: "public",
                newName: "horarios_disponiveis",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ApplicationUsers",
                schema: "public",
                newName: "application_users",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Servicos_PrestadorId",
                schema: "public",
                table: "servicos",
                newName: "IX_servicos_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Servicos_LojaId",
                schema: "public",
                table: "servicos",
                newName: "IX_servicos_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_Prestadores_LojaId",
                schema: "public",
                table: "prestadores",
                newName: "IX_prestadores_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_Prestadores_Email",
                schema: "public",
                table: "prestadores",
                newName: "IX_prestadores_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Lojas_Slug",
                schema: "public",
                table: "lojas",
                newName: "IX_lojas_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_Status",
                schema: "public",
                table: "agendamentos",
                newName: "IX_agendamentos_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_ServicoId_Data",
                schema: "public",
                table: "agendamentos",
                newName: "IX_agendamentos_ServicoId_Data");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_PrestadorId",
                schema: "public",
                table: "agendamentos",
                newName: "IX_agendamentos_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_LojaId_Data_Hora",
                schema: "public",
                table: "agendamentos",
                newName: "IX_agendamentos_LojaId_Data_Hora");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioRoles_RoleId",
                schema: "public",
                table: "usuario_roles",
                newName: "IX_usuario_roles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioLogins_UserId",
                schema: "public",
                table: "usuario_logins",
                newName: "IX_usuario_logins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioClaims_UserId",
                schema: "public",
                table: "usuario_claims",
                newName: "IX_usuario_claims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "public",
                table: "role_claims",
                newName: "IX_role_claims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_HorariosDisponiveis_PrestadorId",
                schema: "public",
                table: "horarios_disponiveis",
                newName: "IX_horarios_disponiveis_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_HorariosDisponiveis_LojaId",
                schema: "public",
                table: "horarios_disponiveis",
                newName: "IX_horarios_disponiveis_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_LojaId",
                schema: "public",
                table: "application_users",
                newName: "IX_application_users_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_CNPJ",
                schema: "public",
                table: "application_users",
                newName: "IX_application_users_CNPJ");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrestadorId",
                schema: "public",
                table: "agendamentos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_servicos",
                schema: "public",
                table: "servicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                schema: "public",
                table: "roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prestadores",
                schema: "public",
                table: "prestadores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lojas",
                schema: "public",
                table: "lojas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_agendamentos",
                schema: "public",
                table: "agendamentos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario_tokens",
                schema: "public",
                table: "usuario_tokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario_roles",
                schema: "public",
                table: "usuario_roles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario_logins",
                schema: "public",
                table: "usuario_logins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario_claims",
                schema: "public",
                table: "usuario_claims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_claims",
                schema: "public",
                table: "role_claims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prestador_servicos",
                schema: "public",
                table: "prestador_servicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_horarios_disponiveis",
                schema: "public",
                table: "horarios_disponiveis",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_application_users",
                schema: "public",
                table: "application_users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_agendamentos_lojas_LojaId",
                schema: "public",
                table: "agendamentos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_agendamentos_prestadores_PrestadorId",
                schema: "public",
                table: "agendamentos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_agendamentos_servicos_ServicoId",
                schema: "public",
                table: "agendamentos",
                column: "ServicoId",
                principalSchema: "public",
                principalTable: "servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_application_users_lojas_LojaId",
                schema: "public",
                table: "application_users",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_horarios_disponiveis_lojas_LojaId",
                schema: "public",
                table: "horarios_disponiveis",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_horarios_disponiveis_prestadores_PrestadorId",
                schema: "public",
                table: "horarios_disponiveis",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prestador_servicos_prestadores_PrestadorId",
                schema: "public",
                table: "prestador_servicos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prestador_servicos_servicos_ServicoId",
                schema: "public",
                table: "prestador_servicos",
                column: "ServicoId",
                principalSchema: "public",
                principalTable: "servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_prestadores_lojas_LojaId",
                schema: "public",
                table: "prestadores",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_role_claims_AspNetRoles_RoleId",
                schema: "public",
                table: "role_claims",
                column: "RoleId",
                principalSchema: "public",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_servicos_lojas_LojaId",
                schema: "public",
                table: "servicos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_servicos_prestadores_PrestadorId",
                schema: "public",
                table: "servicos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "prestadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_claims_application_users_UserId",
                schema: "public",
                table: "usuario_claims",
                column: "UserId",
                principalSchema: "public",
                principalTable: "application_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_logins_application_users_UserId",
                schema: "public",
                table: "usuario_logins",
                column: "UserId",
                principalSchema: "public",
                principalTable: "application_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_roles_AspNetRoles_RoleId",
                schema: "public",
                table: "usuario_roles",
                column: "RoleId",
                principalSchema: "public",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_roles_application_users_UserId",
                schema: "public",
                table: "usuario_roles",
                column: "UserId",
                principalSchema: "public",
                principalTable: "application_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_tokens_application_users_UserId",
                schema: "public",
                table: "usuario_tokens",
                column: "UserId",
                principalSchema: "public",
                principalTable: "application_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agendamentos_lojas_LojaId",
                schema: "public",
                table: "agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_agendamentos_prestadores_PrestadorId",
                schema: "public",
                table: "agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_agendamentos_servicos_ServicoId",
                schema: "public",
                table: "agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_application_users_lojas_LojaId",
                schema: "public",
                table: "application_users");

            migrationBuilder.DropForeignKey(
                name: "FK_horarios_disponiveis_lojas_LojaId",
                schema: "public",
                table: "horarios_disponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_horarios_disponiveis_prestadores_PrestadorId",
                schema: "public",
                table: "horarios_disponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_prestador_servicos_prestadores_PrestadorId",
                schema: "public",
                table: "prestador_servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_prestador_servicos_servicos_ServicoId",
                schema: "public",
                table: "prestador_servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_prestadores_lojas_LojaId",
                schema: "public",
                table: "prestadores");

            migrationBuilder.DropForeignKey(
                name: "FK_role_claims_AspNetRoles_RoleId",
                schema: "public",
                table: "role_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_servicos_lojas_LojaId",
                schema: "public",
                table: "servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_servicos_prestadores_PrestadorId",
                schema: "public",
                table: "servicos");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_claims_application_users_UserId",
                schema: "public",
                table: "usuario_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_logins_application_users_UserId",
                schema: "public",
                table: "usuario_logins");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_roles_AspNetRoles_RoleId",
                schema: "public",
                table: "usuario_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_roles_application_users_UserId",
                schema: "public",
                table: "usuario_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_tokens_application_users_UserId",
                schema: "public",
                table: "usuario_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_servicos",
                schema: "public",
                table: "servicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                schema: "public",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prestadores",
                schema: "public",
                table: "prestadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lojas",
                schema: "public",
                table: "lojas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_agendamentos",
                schema: "public",
                table: "agendamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario_tokens",
                schema: "public",
                table: "usuario_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario_roles",
                schema: "public",
                table: "usuario_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario_logins",
                schema: "public",
                table: "usuario_logins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario_claims",
                schema: "public",
                table: "usuario_claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_claims",
                schema: "public",
                table: "role_claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prestador_servicos",
                schema: "public",
                table: "prestador_servicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_horarios_disponiveis",
                schema: "public",
                table: "horarios_disponiveis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_application_users",
                schema: "public",
                table: "application_users");

            migrationBuilder.RenameTable(
                name: "servicos",
                schema: "public",
                newName: "Servicos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "public",
                newName: "Roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "prestadores",
                schema: "public",
                newName: "Prestadores",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "lojas",
                schema: "public",
                newName: "Lojas",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "agendamentos",
                schema: "public",
                newName: "Agendamentos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "usuario_tokens",
                schema: "public",
                newName: "UsuarioTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "usuario_roles",
                schema: "public",
                newName: "UsuarioRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "usuario_logins",
                schema: "public",
                newName: "UsuarioLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "usuario_claims",
                schema: "public",
                newName: "UsuarioClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "role_claims",
                schema: "public",
                newName: "RoleClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "prestador_servicos",
                schema: "public",
                newName: "PrestadorServicos",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "horarios_disponiveis",
                schema: "public",
                newName: "HorariosDisponiveis",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "application_users",
                schema: "public",
                newName: "ApplicationUsers",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_servicos_PrestadorId",
                schema: "public",
                table: "Servicos",
                newName: "IX_Servicos_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_servicos_LojaId",
                schema: "public",
                table: "Servicos",
                newName: "IX_Servicos_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_prestadores_LojaId",
                schema: "public",
                table: "Prestadores",
                newName: "IX_Prestadores_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_prestadores_Email",
                schema: "public",
                table: "Prestadores",
                newName: "IX_Prestadores_Email");

            migrationBuilder.RenameIndex(
                name: "IX_lojas_Slug",
                schema: "public",
                table: "Lojas",
                newName: "IX_Lojas_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_agendamentos_Status",
                schema: "public",
                table: "Agendamentos",
                newName: "IX_Agendamentos_Status");

            migrationBuilder.RenameIndex(
                name: "IX_agendamentos_ServicoId_Data",
                schema: "public",
                table: "Agendamentos",
                newName: "IX_Agendamentos_ServicoId_Data");

            migrationBuilder.RenameIndex(
                name: "IX_agendamentos_PrestadorId",
                schema: "public",
                table: "Agendamentos",
                newName: "IX_Agendamentos_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_agendamentos_LojaId_Data_Hora",
                schema: "public",
                table: "Agendamentos",
                newName: "IX_Agendamentos_LojaId_Data_Hora");

            migrationBuilder.RenameIndex(
                name: "IX_usuario_roles_RoleId",
                schema: "public",
                table: "UsuarioRoles",
                newName: "IX_UsuarioRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_usuario_logins_UserId",
                schema: "public",
                table: "UsuarioLogins",
                newName: "IX_UsuarioLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_usuario_claims_UserId",
                schema: "public",
                table: "UsuarioClaims",
                newName: "IX_UsuarioClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_role_claims_RoleId",
                schema: "public",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_horarios_disponiveis_PrestadorId",
                schema: "public",
                table: "HorariosDisponiveis",
                newName: "IX_HorariosDisponiveis_PrestadorId");

            migrationBuilder.RenameIndex(
                name: "IX_horarios_disponiveis_LojaId",
                schema: "public",
                table: "HorariosDisponiveis",
                newName: "IX_HorariosDisponiveis_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_application_users_LojaId",
                schema: "public",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_LojaId");

            migrationBuilder.RenameIndex(
                name: "IX_application_users_CNPJ",
                schema: "public",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_CNPJ");

            migrationBuilder.AlterColumn<Guid>(
                name: "PrestadorId",
                schema: "public",
                table: "Agendamentos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servicos",
                schema: "public",
                table: "Servicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "public",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prestadores",
                schema: "public",
                table: "Prestadores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lojas",
                schema: "public",
                table: "Lojas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agendamentos",
                schema: "public",
                table: "Agendamentos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioTokens",
                schema: "public",
                table: "UsuarioTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRoles",
                schema: "public",
                table: "UsuarioRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioLogins",
                schema: "public",
                table: "UsuarioLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioClaims",
                schema: "public",
                table: "UsuarioClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaims",
                schema: "public",
                table: "RoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrestadorServicos",
                schema: "public",
                table: "PrestadorServicos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HorariosDisponiveis",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUsers",
                schema: "public",
                table: "ApplicationUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Lojas_LojaId",
                schema: "public",
                table: "Agendamentos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Prestadores_PrestadorId",
                schema: "public",
                table: "Agendamentos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                schema: "public",
                table: "Agendamentos",
                column: "ServicoId",
                principalSchema: "public",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Lojas_LojaId",
                schema: "public",
                table: "ApplicationUsers",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HorariosDisponiveis_Lojas_LojaId",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HorariosDisponiveis_Prestadores_PrestadorId",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestadores_Lojas_LojaId",
                schema: "public",
                table: "Prestadores",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestadorServicos_Prestadores_PrestadorId",
                schema: "public",
                table: "PrestadorServicos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrestadorServicos_Servicos_ServicoId",
                schema: "public",
                table: "PrestadorServicos",
                column: "ServicoId",
                principalSchema: "public",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_AspNetRoles_RoleId",
                schema: "public",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "public",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Prestadores_PrestadorId",
                schema: "public",
                table: "Servicos",
                column: "PrestadorId",
                principalSchema: "public",
                principalTable: "Prestadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioClaims_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioClaims",
                column: "UserId",
                principalSchema: "public",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioLogins_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioLogins",
                column: "UserId",
                principalSchema: "public",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRoles_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioRoles",
                column: "UserId",
                principalSchema: "public",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRoles_AspNetRoles_RoleId",
                schema: "public",
                table: "UsuarioRoles",
                column: "RoleId",
                principalSchema: "public",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioTokens_ApplicationUsers_UserId",
                schema: "public",
                table: "UsuarioTokens",
                column: "UserId",
                principalSchema: "public",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
