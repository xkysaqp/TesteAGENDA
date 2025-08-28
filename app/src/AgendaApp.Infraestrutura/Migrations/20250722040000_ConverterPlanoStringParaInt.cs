using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApp.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class ConverterPlanoStringParaInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Converter coluna Plano de string para integer usando USING clause
            migrationBuilder.Sql(@"
                ALTER TABLE ""public"".""Lojas"" 
                ALTER COLUMN ""Plano"" TYPE integer 
                USING CASE
                    WHEN ""Plano"" = 'Básico' OR ""Plano"" = 'Basico' THEN 1
                    WHEN ""Plano"" = 'Intermediário' OR ""Plano"" = 'Intermediario' THEN 2
                    WHEN ""Plano"" = 'Premium' THEN 3
                    WHEN ""Plano"" = 'Enterprise' THEN 4
                    ELSE 1
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Converter de volta para string
            migrationBuilder.Sql(@"
                ALTER TABLE ""public"".""Lojas"" 
                ALTER COLUMN ""Plano"" TYPE varchar(50)
                USING CASE
                    WHEN ""Plano"" = 1 THEN 'Básico'
                    WHEN ""Plano"" = 2 THEN 'Intermediário'
                    WHEN ""Plano"" = 3 THEN 'Premium'
                    WHEN ""Plano"" = 4 THEN 'Enterprise'
                    ELSE 'Básico'
                END;
            ");
        }
    }
} 