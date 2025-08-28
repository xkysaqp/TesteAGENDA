using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Web.ViewModels;
using AgendaApp.Dominio.Constants;

namespace AgendaApp.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DashboardController> _logger;
    private readonly IServicoService _servicoService;
    private readonly IPrestadorService _prestadorService;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ILogger<DashboardController> logger,
        IServicoService servicoService,
        IPrestadorService prestadorService)
    {
        _userManager = userManager;
        _logger = logger;
        _servicoService = servicoService;
        _prestadorService = prestadorService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new DashboardViewModel
            {
                NomeUsuario = user.Nome,
                TipoUsuario = user.EhAdmin ? "Administrador" : "Usuário de Loja",
                LojaId = user.LojaId
            };

            if (user.EhAdmin)
            {
                await CarregarEstatisticasAdmin(viewModel);
            }
            else if (user.EhUsuarioLoja)
            {
                await CarregarEstatisticasLoja(viewModel, user.LojaId);
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar dashboard para usuário {UserId}", User.Identity?.Name);
            return View("Error");
        }
    }

    private async Task CarregarEstatisticasAdmin(DashboardViewModel viewModel)
    {
        try
        {
            var filtroTodosPrestadores = new AgendaApp.Aplicacao.DTOs.PrestadorFiltroDto();
            var prestadores = await _prestadorService.ObterComFiltroAsync(filtroTodosPrestadores);
            viewModel.TotalPrestadores = prestadores.Count();

            var filtroTodosServicos = new AgendaApp.Aplicacao.DTOs.ServicoFiltroDto();
            var servicos = await _servicoService.ObterComFiltroAsync(filtroTodosServicos);
            viewModel.TotalServicos = servicos.Count();

            var servicosPorLoja = servicos
                .Where(s => s.LojaId.HasValue)
                .GroupBy(s => s.LojaId!.Value)
                .Select(g => new { LojaId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            viewModel.DadosGraficos = servicosPorLoja.Select(x => (object)new 
            {
                Label = $"Loja {x.LojaId.ToString()[..8]}...",
                Value = x.Count
            }).ToList();

            _logger.LogInformation("Estatísticas admin carregadas: {Prestadores} prestadores, {Servicos} serviços", 
                viewModel.TotalPrestadores, viewModel.TotalServicos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar estatísticas do admin");
            viewModel.TotalPrestadores = 0;
            viewModel.TotalServicos = 0;
        }
    }

    private async Task CarregarEstatisticasLoja(DashboardViewModel viewModel, Guid lojaId)
    {
        try
        {
            var filtroPrestadores = new AgendaApp.Aplicacao.DTOs.PrestadorFiltroDto 
            { 
                LojaId = lojaId
            };
            var prestadores = await _prestadorService.ObterComFiltroAsync(filtroPrestadores);
            viewModel.TotalPrestadores = prestadores.Count();

            var filtroServicos = new AgendaApp.Aplicacao.DTOs.ServicoFiltroDto 
            { 
                LojaId = lojaId
            };
            var servicos = await _servicoService.ObterComFiltroAsync(filtroServicos);
            viewModel.TotalServicos = servicos.Count();

            var servicosPorPrestador = servicos
                .GroupBy(s => s.PrestadorNome ?? "Sem Prestador")
                .Select(g => new { 
                    Label = g.Key, 
                    Value = g.Count() 
                })
                .OrderByDescending(x => x.Value)
                .Take(10)
                .ToList();

            viewModel.DadosGraficos = servicosPorPrestador.Cast<object>().ToList();

            _logger.LogInformation("Estatísticas loja {LojaId} carregadas: {Prestadores} prestadores, {Servicos} serviços", 
                lojaId, viewModel.TotalPrestadores, viewModel.TotalServicos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar estatísticas da loja {LojaId}", lojaId);
            viewModel.TotalPrestadores = 0;
            viewModel.TotalServicos = 0;
        }
    }
} 