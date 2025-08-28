using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApp.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class TornarLojaIdObrigatorioMultitenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Lojas_LojaId",
                schema: "public",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HorariosDisponiveis_Lojas_LojaId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestadores_Lojas_LojaId",
                schema: "public",
                table: "Prestadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos");

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "Servicos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "Prestadores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // Removido AlterColumn para Plano - será feito em migração específica

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "HorariosDisponiveis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "ApplicationUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
                name: "FK_Prestadores_Lojas_LojaId",
                schema: "public",
                table: "Prestadores",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Lojas_LojaId",
                schema: "public",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HorariosDisponiveis_Lojas_LojaId",
                schema: "public",
                table: "HorariosDisponiveis");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestadores_Lojas_LojaId",
                schema: "public",
                table: "Prestadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos");

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "Servicos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "Prestadores",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // Removido AlterColumn para Plano - será feito em migração específica

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "HorariosDisponiveis",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "LojaId",
                schema: "public",
                table: "ApplicationUsers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Lojas_LojaId",
                schema: "public",
                table: "ApplicationUsers",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HorariosDisponiveis_Lojas_LojaId",
                schema: "public",
                table: "HorariosDisponiveis",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
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
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_Lojas_LojaId",
                schema: "public",
                table: "Servicos",
                column: "LojaId",
                principalSchema: "public",
                principalTable: "Lojas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
