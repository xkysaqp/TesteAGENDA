using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgendaApp.Web.Controllers;

[Authorize]
public class AgendamentoController : Controller
{
    private readonly IAgendamentoService _agendamentoService;
    private readonly IPrestadorService _prestadorService;
    private readonly IServicoService _servicoService;
    private readonly ILojaService _lojaService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<AgendamentoController> _logger;

    public AgendamentoController(
        IAgendamentoService agendamentoService,
        IPrestadorService prestadorService,
        IServicoService servicoService,
        ILojaService lojaService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<AgendamentoController> logger)
    {
        _agendamentoService = agendamentoService;
        _prestadorService = prestadorService;
        _servicoService = servicoService;
        _lojaService = lojaService;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(AgendamentoFiltroViewModel? filtro)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Listando agendamentos com filtros para usuário {Email}", user.Email);

            var filtroDto = new AgendamentoFiltroDto
            {
                LojaId = user.EhUsuarioLoja ? user.LojaId : filtro?.LojaId,
                PrestadorId = filtro?.PrestadorId,
                ServicoId = filtro?.ServicoId,
                Status = filtro?.Status,
                DataInicio = filtro?.DataInicio,
                DataFim = filtro?.DataFim,
                ClienteNome = filtro?.ClienteNome,
                ApenasAtivos = filtro?.ApenasAtivos ?? true
            };

            var agendamentosDto = await _agendamentoService.ObterPorLojaAsync(filtroDto.LojaId ?? Guid.Empty);
            
            // Aplicar filtros adicionais
            if (filtroDto.PrestadorId.HasValue)
                agendamentosDto = agendamentosDto.Where(a => a.PrestadorId == filtroDto.PrestadorId.Value);
            
            if (filtroDto.ServicoId.HasValue)
                agendamentosDto = agendamentosDto.Where(a => a.ServicoId == filtroDto.ServicoId.Value);
            
            if (filtroDto.Status.HasValue)
                agendamentosDto = agendamentosDto.Where(a => a.Status == filtroDto.Status.Value);
            
            if (!string.IsNullOrWhiteSpace(filtroDto.ClienteNome))
                agendamentosDto = agendamentosDto.Where(a => a.ClienteNome.Contains(filtroDto.ClienteNome, StringComparison.OrdinalIgnoreCase));

            var agendamentos = _mapper.Map<List<AgendamentoListViewModel>>(agendamentosDto);

            ViewBag.Filtro = filtro ?? new AgendamentoFiltroViewModel();
            ViewBag.IsLojaUser = user.EhUsuarioLoja;
            
            if (user.EhAdmin)
            {
                ViewBag.Lojas = await ObterLojasParaSelect();
            }

            return View(agendamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar agendamentos");
            TempData["ErrorMessage"] = "Erro ao carregar a lista de agendamentos.";
            return View(new List<AgendamentoListViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo detalhes do agendamento {Id}", id);

            var agendamentoDto = await _agendamentoService.ObterPorIdAsync(id);
            if (agendamentoDto == null)
            {
                TempData["ErrorMessage"] = "Agendamento não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamentoDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exibir detalhes do agendamento {Id}", id);
            TempData["ErrorMessage"] = "Erro interno do servidor.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Exibindo formulário de criação de agendamento para usuário {Email}", user.Email);

            var viewModel = new AgendamentoViewModel();

            if (user.EhAdmin)
            {
                ViewBag.Lojas = await ObterLojasParaSelect();
                ViewBag.IsAdmin = true;
            }
            else
            {
                viewModel.LojaId = user.LojaId;
                ViewBag.IsAdmin = false;
                ViewBag.LojaNome = await ObterNomeLoja(user.LojaId);
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar formulário de criação de agendamento");
            TempData["ErrorMessage"] = "Erro ao carregar formulário.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AgendamentoViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                await PrepararViewBags(user, viewModel.LojaId);
                return View(viewModel);
            }

            var criarAgendamentoDto = new CriarAgendamentoDto
            {
                Data = viewModel.Data,
                Hora = viewModel.Hora,
                ClienteNome = viewModel.ClienteNome,
                ClienteTelefone = viewModel.ClienteTelefone,
                ClienteEmail = viewModel.ClienteEmail,
                Observacoes = viewModel.Observacoes,
                LojaId = viewModel.LojaId,
                ServicoId = viewModel.ServicoId,
                PrestadorId = viewModel.PrestadorId
            };

            var agendamentoCriado = await _agendamentoService.CriarAsync(criarAgendamentoDto);

            TempData["SuccessMessage"] = $"Agendamento criado com sucesso para {agendamentoCriado.ClienteNome}!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar agendamento");
            ModelState.AddModelError("", ex.Message);
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await PrepararViewBags(user, viewModel.LojaId);
            }
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar agendamento");
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await PrepararViewBags(user, viewModel.LojaId);
            }
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo formulário de edição do agendamento {Id}", id);

            var agendamentoDto = await _agendamentoService.ObterPorIdAsync(id);
            if (agendamentoDto == null)
            {
                TempData["ErrorMessage"] = "Agendamento não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<AgendamentoViewModel>(agendamentoDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar agendamento para edição {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do agendamento.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AgendamentoViewModel viewModel)
    {
        try
        {
            if (id != viewModel.Id)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var atualizarAgendamentoDto = new AtualizarAgendamentoDto
            {
                Id = viewModel.Id,
                Data = viewModel.Data,
                Hora = viewModel.Hora,
                ClienteNome = viewModel.ClienteNome,
                ClienteTelefone = viewModel.ClienteTelefone,
                ClienteEmail = viewModel.ClienteEmail,
                Observacoes = viewModel.Observacoes
            };

            var agendamentoAtualizado = await _agendamentoService.AtualizarAsync(atualizarAgendamentoDto);

            TempData["SuccessMessage"] = $"Agendamento atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar agendamento {Id}", id);
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(Guid id, decimal? valor = null)
    {
        try
        {
            var agendamento = await _agendamentoService.ConfirmarAsync(id, valor);
            
            TempData["SuccessMessage"] = $"Agendamento confirmado com sucesso!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar agendamento {Id}", id);
            TempData["ErrorMessage"] = "Erro ao confirmar agendamento.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(Guid id, string motivo)
    {
        try
        {
            var agendamento = await _agendamentoService.CancelarAsync(id, motivo);
            
            TempData["SuccessMessage"] = $"Agendamento cancelado com sucesso!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar agendamento {Id}", id);
            TempData["ErrorMessage"] = "Erro ao cancelar agendamento.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Concluir(Guid id, decimal? valorFinal = null)
    {
        try
        {
            var agendamento = await _agendamentoService.ConcluirAsync(id, valorFinal);
            
            TempData["SuccessMessage"] = $"Agendamento concluído com sucesso!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao concluir agendamento {Id}", id);
            TempData["ErrorMessage"] = "Erro ao concluir agendamento.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    #region Métodos Auxiliares

    private async Task<SelectList> ObterLojasParaSelect()
    {
        try
        {
            var lojas = await _lojaService.ObterTodasAsync();
            return new SelectList(
                lojas.Where(l => l.Ativa)
                     .OrderBy(l => l.Nome)
                     .Select(l => new { Id = l.Id, Nome = l.Nome }),
                "Id",
                "Nome");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lojas para select");
            return new SelectList(new List<object>(), "Id", "Nome");
        }
    }

    private async Task<string?> ObterNomeLoja(Guid? lojaId)
    {
        try
        {
            if (!lojaId.HasValue) return null;

            var loja = await _lojaService.ObterPorIdAsync(lojaId.Value);
            return loja?.Nome;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter nome da loja {LojaId}", lojaId);
            return null;
        }
    }

    private async Task PrepararViewBags(ApplicationUser user, Guid? lojaIdSelecionada)
    {
        if (user.EhAdmin)
        {
            ViewBag.Lojas = await ObterLojasParaSelect();
            ViewBag.IsAdmin = true;
        }
        else
        {
            ViewBag.IsAdmin = false;
            ViewBag.LojaNome = await ObterNomeLoja(user.LojaId);
        }
    }

    #endregion
}
