using AgendaApp.Dominio.Constants;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Enums;
using AgendaApp.Infraestrutura.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace AgendaApp.Infraestrutura.Data;

public static class AgendaAppDbSeed
{
    public static async Task ExecutarSeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AgendaAppDbSeed");

        try
        {
            var context = services.GetRequiredService<AgendaAppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            logger.LogInformation("Iniciando seed do banco de dados...");

            await context.Database.EnsureCreatedAsync();

            await SeedRolesAsync(roleManager, logger);
            var lojaSistemaId = await SeedLojaSistemaAsync(context, logger);
            await SeedAdminAsync(userManager, lojaSistemaId, logger);
            await SeedLojaTesteAsync(context, userManager, logger);
            await SeedServicosGlobaisAsync(context, logger);

            logger.LogInformation("Seed do banco de dados concluído com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar seed do banco de dados");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Criando roles padrão...");

        foreach (var roleName in new[] { Roles.Admin, Roles.Loja })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Descricao = roleName == Roles.Admin ? "Administrador do sistema" : "Usuário de loja",
                    DataCriacao = DateTime.UtcNow
                };

                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    logger.LogInformation("✅ Role '{RoleName}' criada com sucesso", roleName);
                }
                else
                {
                    logger.LogError("❌ Erro ao criar role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task<Guid> SeedLojaSistemaAsync(AgendaAppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando loja sistema para admins...");

        const string slugSistema = "sistema";
        
        var lojaSistema = await context.Lojas
            .FirstOrDefaultAsync(l => l.Slug == slugSistema);

        if (lojaSistema == null)
        {
            lojaSistema = new Loja(
                nome: "Sistema AgendaApp",
                slug: slugSistema,
                cnpj: "11.444.777/0001-61", // CNPJ fictício válido para loja sistema
                emailContato: "sistema@agendaapp.com",
                telefoneContato: "(11) 0000-0000",
                plano: TipoPlano.Enterprise // Loja sistema tem plano enterprise
            );

            lojaSistema.DefinirInformacoesComplementares(
                endereco: "Loja virtual do sistema",
                descricao: "Loja especial para usuários administradores do sistema",
                logoUrl: null
            );

            context.Lojas.Add(lojaSistema);
            await context.SaveChangesAsync();

            logger.LogInformation("Loja sistema criada: {Slug}", lojaSistema.Slug);
        }
        else
        {
            logger.LogInformation("Loja sistema já existe: {Slug}", lojaSistema.Slug);
        }

        return lojaSistema.Id;
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, Guid lojaSistemaId, ILogger logger)
    {
        logger.LogInformation("Criando usuário admin padrão...");

        const string adminEmail = "admin@agendaapp.com";
        const string adminPassword = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                Nome = "Administrador Global",
                LojaId = lojaSistemaId, // Admin agora vinculado à loja sistema
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                logger.LogInformation("Admin criado: {Email} vinculado à loja sistema", adminEmail);
            }
            else
            {
                logger.LogError("Erro ao criar admin: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Atualizar admin existente para garantir que tenha LojaId
            if (existingAdmin.LojaId == Guid.Empty)
            {
                existingAdmin.VincularALoja(lojaSistemaId);
                await userManager.UpdateAsync(existingAdmin);
                logger.LogInformation("Admin existente atualizado com LojaId da loja sistema");
            }
            
            logger.LogInformation("Admin já existe: {Email}", adminEmail);
        }
    }

    private static async Task SeedLojaTesteAsync(AgendaAppDbContext context, UserManager<ApplicationUser> userManager, ILogger logger)
    {
        logger.LogInformation("Criando loja de teste...");

        // Verificar se já existe loja de teste
        var lojaExistente = await context.Lojas
            .FirstOrDefaultAsync(l => l.Slug == "loja-teste");

        if (lojaExistente == null)
        {
            // Criar loja de teste
            var lojaTeste = new Loja(
                nome: "Loja de Teste",
                slug: "loja-teste",
                cnpj: "11.222.333/0001-81",
                emailContato: "contato@lojateste.com",
                telefoneContato: "(11) 98765-4321",
                plano: TipoPlano.Intermediario // Corrigir para valor existente no enum
            );

            lojaTeste.DefinirInformacoesComplementares(
                endereco: "Rua de Teste, 123 - São Paulo/SP",
                descricao: "Loja criada automaticamente para testes do sistema",
                logoUrl: null
            );

            context.Lojas.Add(lojaTeste);
            await context.SaveChangesAsync();

            logger.LogInformation("Loja de teste criada: {Slug}", lojaTeste.Slug);

            // Criar usuário da loja de teste
            await CriarUsuarioLojaAsync(userManager, lojaTeste.Id, logger);

            // Criar dados de exemplo para a loja
            await CriarDadosExemploAsync(context, lojaTeste.Id, logger);
        }
        else
        {
            logger.LogInformation("Loja de teste já existe: {Slug}", lojaExistente.Slug);
        }
    }

    private static async Task CriarUsuarioLojaAsync(UserManager<ApplicationUser> userManager, Guid lojaId, ILogger logger)
    {
        const string userEmail = "usuario@lojateste.com";
        const string userPassword = "Usuario123!";

        var existingUser = await userManager.FindByEmailAsync(userEmail);
        if (existingUser == null)
        {
            var usuarioLoja = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = userEmail,
                Email = userEmail,
                NormalizedUserName = userEmail.ToUpper(),
                NormalizedEmail = userEmail.ToUpper(),
                EmailConfirmed = true,
                Nome = "Usuário Loja Teste",
                LojaId = lojaId,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(usuarioLoja, userPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(usuarioLoja, Roles.Loja);
                logger.LogInformation("Usuário da loja criado: {Email}", userEmail);
            }
            else
            {
                logger.LogError("Erro ao criar usuário da loja: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task CriarDadosExemploAsync(AgendaAppDbContext context, Guid lojaId, ILogger logger)
    {
        logger.LogInformation("Criando dados de exemplo para loja de teste...");

        // Verificar se já existem dados
        var prestadorExistente = await context.Prestadores
            .FirstOrDefaultAsync(p => p.LojaId == lojaId);

        if (prestadorExistente == null)
        {
            // Criar prestadores de exemplo de diferentes áreas
            var prestadores = new[]
            {
                new Prestador(
                    nome: "Dr. João Silva",
                    areaAtuacao: "Medicina Geral",
                    email: "joao@lojateste.com", 
                    telefone: "(11) 91234-5678",
                    cnpj: null,
                    lojaId: lojaId
                ),
                new Prestador(
                    nome: "Ana Martins",
                    areaAtuacao: "Massoterapia",
                    email: "ana@lojateste.com", 
                    telefone: "(11) 92345-6789",
                    cnpj: null,
                    lojaId: lojaId
                ),
                new Prestador(
                    nome: "Carlos Santos",
                    areaAtuacao: "Personal Trainer",
                    email: "carlos@lojateste.com", 
                    telefone: "(11) 93456-7890",
                    cnpj: null,
                    lojaId: lojaId
                ),
                new Prestador(
                    nome: "Mariana Costa",
                    areaAtuacao: "Estética",
                    email: "mariana@lojateste.com", 
                    telefone: "(11) 94567-8901",
                    cnpj: null,
                    lojaId: lojaId
                )
            };

            context.Prestadores.AddRange(prestadores);
            await context.SaveChangesAsync();

            // Criar alguns serviços específicos de exemplo (apenas para o Dr. João)
            var servicosEspecificos = new[]
            {
                new Servico("Consulta Médica Particular", "Consulta médica particular do Dr. João", 180.00m, 60, lojaId),
                new Servico("Retorno Dr. João", "Consulta de retorno específica", 90.00m, 30, lojaId),
                new Servico("Exame Físico Completo", "Exame físico detalhado do Dr. João", 220.00m, 90, lojaId)
            };

            context.Servicos.AddRange(servicosEspecificos);

            // Criar horários disponíveis de exemplo (apenas para o Dr. João)
            var horarios = new[]
            {
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Segunda, new TimeOnly(8, 0), new TimeOnly(12, 0), prestadores[0].Id, lojaId),
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Segunda, new TimeOnly(14, 0), new TimeOnly(18, 0), prestadores[0].Id, lojaId),
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Terca, new TimeOnly(8, 0), new TimeOnly(12, 0), prestadores[0].Id, lojaId),
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Quarta, new TimeOnly(8, 0), new TimeOnly(17, 0), prestadores[0].Id, lojaId),
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Quinta, new TimeOnly(14, 0), new TimeOnly(18, 0), prestadores[0].Id, lojaId),
                new HorarioDisponivel(Dominio.Enums.DiaSemana.Sexta, new TimeOnly(8, 0), new TimeOnly(16, 0), prestadores[0].Id, lojaId)
            };

            context.HorariosDisponiveis.AddRange(horarios);

            // Criar alguns agendamentos de exemplo
            var dataHoje = DateOnly.FromDateTime(DateTime.Today);
            var agendamentos = new[]
            {
                new Agendamento(
                    dataHoje.AddDays(1), new TimeOnly(9, 0), 
                    "Maria Santos", "(11) 98765-4321", 
                    lojaId, servicosEspecificos[0].Id, prestadores[0].Id,
                    "maria@email.com", "Primeira consulta"),
                
                new Agendamento(
                    dataHoje.AddDays(2), new TimeOnly(14, 30), 
                    "Carlos Oliveira", "(11) 97654-3210", 
                    lojaId, servicosEspecificos[1].Id, prestadores[0].Id,
                    null, "Retorno da consulta anterior")
            };

            // Confirmar um dos agendamentos
            agendamentos[0].Confirmar(150.00m);

            context.Agendamentos.AddRange(agendamentos);
            await context.SaveChangesAsync();

            logger.LogInformation("Dados de exemplo criados com sucesso!");
        }
    }

    private static async Task SeedServicosGlobaisAsync(AgendaAppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando serviços globais de exemplo...");

        // Buscar a loja de teste para criar serviços globais
        var lojaTeste = await context.Lojas
            .FirstOrDefaultAsync(l => l.Slug == "loja-teste");

        if (lojaTeste == null)
        {
            logger.LogWarning("Loja de teste não encontrada. Serviços globais não foram criados.");
            return;
        }

        // Verificar se já existem serviços globais
        var servicosGlobaisExistentes = await context.Servicos
            .Where(s => s.LojaId == lojaTeste.Id && !s.PrestadorServicos.Any())
            .AnyAsync();

        if (!servicosGlobaisExistentes)
        {
            // Criar serviços globais de exemplo
            var servicosGlobais = new[]
            {
                // Serviços Médicos Gerais
                new Servico(
                    nome: "Consulta de Check-up",
                    descricao: "Avaliação médica preventiva completa com exames básicos",
                    valor: 180.00m,
                    duracaoEmMinutos: 90,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Teleconsulta",
                    descricao: "Consulta médica realizada por videoconferência",
                    valor: 120.00m,
                    duracaoEmMinutos: 45,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Consulta de Urgência",
                    descricao: "Atendimento médico para casos urgentes no mesmo dia",
                    valor: 250.00m,
                    duracaoEmMinutos: 30,
                    lojaId: lojaTeste.Id
                ),

                // Serviços de Bem-estar
                new Servico(
                    nome: "Massagem Relaxante",
                    descricao: "Massagem terapêutica para alívio de tensões musculares",
                    valor: 80.00m,
                    duracaoEmMinutos: 60,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Acupuntura",
                    descricao: "Sessão de acupuntura para tratamento holístico",
                    valor: 100.00m,
                    duracaoEmMinutos: 60,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Reflexologia",
                    descricao: "Tratamento através de massagem nos pés",
                    valor: 70.00m,
                    duracaoEmMinutos: 45,
                    lojaId: lojaTeste.Id
                ),

                // Serviços de Estética
                new Servico(
                    nome: "Limpeza de Pele",
                    descricao: "Limpeza facial profunda com extração de cravos",
                    valor: 85.00m,
                    duracaoEmMinutos: 90,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Manicure e Pedicure",
                    descricao: "Cuidados completos para unhas das mãos e pés",
                    valor: 45.00m,
                    duracaoEmMinutos: 75,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Design de Sobrancelhas",
                    descricao: "Modelagem e design de sobrancelhas com pinça",
                    valor: 35.00m,
                    duracaoEmMinutos: 30,
                    lojaId: lojaTeste.Id
                ),

                // Serviços Fitness
                new Servico(
                    nome: "Personal Training",
                    descricao: "Aula particular de exercícios personalizados",
                    valor: 90.00m,
                    duracaoEmMinutos: 60,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Avaliação Física",
                    descricao: "Análise completa da composição corporal e condicionamento",
                    valor: 65.00m,
                    duracaoEmMinutos: 45,
                    lojaId: lojaTeste.Id
                ),
                new Servico(
                    nome: "Aula de Pilates",
                    descricao: "Sessão de pilates individual ou em dupla",
                    valor: 75.00m,
                    duracaoEmMinutos: 50,
                    lojaId: lojaTeste.Id
                )
            };

            context.Servicos.AddRange(servicosGlobais);
            await context.SaveChangesAsync();

            logger.LogInformation("Criados {Count} serviços globais de exemplo!", servicosGlobais.Length);

            // Log detalhado dos serviços criados
            foreach (var servico in servicosGlobais)
            {
                logger.LogDebug("Serviço global criado: {Nome} - R$ {Valor:F2} ({Duracao}min)", 
                    servico.Nome, servico.Valor, servico.DuracaoEmMinutos);
            }
        }
        else
        {
            logger.LogInformation("Serviços globais já existem na loja de teste.");
        }
    }
} 