using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using AgendaApp.Web.ViewModels;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Dominio.Enums;
using AgendaApp.Infraestrutura.Identity;

namespace AgendaApp.Web.Controllers;

[Authorize]
public class HorarioDisponivelController : Controller
{
    private readonly IHorarioDisponivelService _horarioService;
    private readonly IPrestadorService _prestadorService;
    private readonly IMapper _mapper;
    private readonly ILogger<HorarioDisponivelController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HorarioDisponivelController(
        IHorarioDisponivelService horarioService,
        IPrestadorService prestadorService,
        IMapper mapper, 
        ILogger<HorarioDisponivelController> logger,
        UserManager<ApplicationUser> userManager)
    {
        _horarioService = horarioService;
        _prestadorService = prestadorService;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(HorarioDisponivelFiltroViewModel? filtro)
    {
        try
        {
            _logger.LogInformation("Listando horários disponíveis com filtros");

            var filtroDto = _mapper.Map<HorarioDisponivelFiltroDto>(filtro ?? new HorarioDisponivelFiltroViewModel());
            var horariosDto = await _horarioService.ObterComFiltroAsync(filtroDto);
            var horarios = _mapper.Map<IEnumerable<HorarioDisponivelListViewModel>>(horariosDto);

            ViewBag.Filtro = filtro ?? new HorarioDisponivelFiltroViewModel();
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();

            return View(horarios.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar horários disponíveis");
            TempData["ErrorMessage"] = "Erro ao carregar a lista de horários disponíveis.";
            return View(new List<HorarioDisponivelListViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo detalhes do horário {Id}", id);

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<HorarioDisponivelViewModel>(horarioDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exibir detalhes do horário {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os detalhes do horário.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Exibindo formulário de criação de horário");
        
        var viewModel = new HorarioDisponivelViewModel
        {
            HoraInicio = new TimeOnly(8, 0),
            HoraFim = new TimeOnly(18, 0)
        };
        
        ViewBag.Prestadores = await ObterPrestadoresParaSelect();
        ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
        
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HorarioDisponivelViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(viewModel.PrestadorId);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _horarioService.ValidarPermissoesCriacao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para criar horários para este prestador.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para criação de horário");
                ViewBag.Prestadores = await ObterPrestadoresParaSelect();
                ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
                return View(viewModel);
            }

            _logger.LogInformation("Criando horário: {DiaSemana} {HoraInicio}-{HoraFim} para prestador {PrestadorId}", 
                viewModel.DiaSemana, viewModel.HoraInicio, viewModel.HoraFim, viewModel.PrestadorId);

            var criarDto = _mapper.Map<CriarHorarioDisponivelDto>(viewModel);
            var horarioCriado = await _horarioService.CriarAsync(criarDto);

            TempData["SuccessMessage"] = $"Horário {viewModel.DiaSemanaTexto} ({viewModel.HorarioFormatado}) criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflito ao criar horário");
            ModelState.AddModelError("", ex.Message);
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar horário");
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateMultiple()
    {
        _logger.LogInformation("Exibindo formulário de criação múltipla de horários");
        
        var viewModel = new HorarioMultiploViewModel
        {
            HoraInicio = new TimeOnly(8, 0),
            HoraFim = new TimeOnly(18, 0)
        };
        
        ViewBag.Prestadores = await ObterPrestadoresParaSelect();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMultiple(HorarioMultiploViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(viewModel.PrestadorId);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _horarioService.ValidarPermissoesCriacao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para criar horários para este prestador.";
                return RedirectToAction(nameof(Index));
            }

            viewModel.DiasSemana.Clear();
            if (viewModel.Segunda) viewModel.DiasSemana.Add(DiaSemana.Segunda);
            if (viewModel.Terca) viewModel.DiasSemana.Add(DiaSemana.Terca);
            if (viewModel.Quarta) viewModel.DiasSemana.Add(DiaSemana.Quarta);
            if (viewModel.Quinta) viewModel.DiasSemana.Add(DiaSemana.Quinta);
            if (viewModel.Sexta) viewModel.DiasSemana.Add(DiaSemana.Sexta);
            if (viewModel.Sabado) viewModel.DiasSemana.Add(DiaSemana.Sabado);
            if (viewModel.Domingo) viewModel.DiasSemana.Add(DiaSemana.Domingo);

            if (!viewModel.DiasSemana.Any())
            {
                ModelState.AddModelError("", "Selecione pelo menos um dia da semana.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para criação múltipla de horários");
                ViewBag.Prestadores = await ObterPrestadoresParaSelect();
                return View(viewModel);
            }

            _logger.LogInformation("Criando múltiplos horários para prestador {PrestadorId}: {Dias}", 
                viewModel.PrestadorId, string.Join(", ", viewModel.DiasSemana));

            var criarMultiploDto = _mapper.Map<CriarHorarioMultiploDto>(viewModel);
            var resultado = await _horarioService.CriarMultiplosAsync(criarMultiploDto);

            if (resultado.Conflitos.Any())
            {
                var mensagemConflitos = string.Join(", ", resultado.Conflitos);
                TempData["WarningMessage"] = $"Alguns horários não foram criados devido a conflitos: {mensagemConflitos}";
            }

            if (resultado.HorariosCriados > 0)
            {
                TempData["SuccessMessage"] = $"{resultado.HorariosCriados} horários criados com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Nenhum horário foi criado devido a conflitos.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar múltiplos horários");
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo formulário de edição do horário {Id}", id);

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<HorarioDisponivelViewModel>(horarioDto);
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar horário para edição {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do horário.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, HorarioDisponivelViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != viewModel.Id)
            {
                TempData["ErrorMessage"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(horarioDto.PrestadorId);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _horarioService.ValidarPermissoesEdicao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para editar horários deste prestador.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para edição de horário {Id}", id);
                ViewBag.Prestadores = await ObterPrestadoresParaSelect();
                ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
                return View(viewModel);
            }

            _logger.LogInformation("Atualizando horário {Id}", id);

            var atualizarDto = _mapper.Map<AtualizarHorarioDisponivelDto>(viewModel);
            await _horarioService.AtualizarAsync(atualizarDto);

            TempData["SuccessMessage"] = "Horário atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflito ao atualizar horário {Id}", id);
            ModelState.AddModelError("", ex.Message);
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar horário {Id}", id);
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            ViewBag.Prestadores = await ObterPrestadoresParaSelect();
            ViewBag.DiasSemana = ObterDiasSemanaParaSelect();
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo confirmação de exclusão do horário {Id}", id);

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<HorarioDisponivelViewModel>(horarioDto);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar horário para exclusão {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do horário.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(horarioDto.PrestadorId);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _horarioService.ValidarPermissoesExclusao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para excluir horários deste prestador.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Excluindo horário {Id}", id);

            var sucesso = await _horarioService.DesativarAsync(id);
            
            if (sucesso)
            {
                TempData["SuccessMessage"] = "Horário excluído com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Horário não encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir horário {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir o horário.";
            return RedirectToAction(nameof(Index));
        }
    }

    #region Métodos Auxiliares

    private async Task<SelectList> ObterPrestadoresParaSelect()
    {
        try
        {
            var filtro = new PrestadorFiltroDto();
            var prestadoresDto = await _prestadorService.ObterComFiltroAsync(filtro);
            var prestadores = prestadoresDto.Select(p => new { Id = p.Id, Nome = p.Nome }).ToList();
            return new SelectList(prestadores, "Id", "Nome");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar prestadores para select");
            return new SelectList(new List<object>(), "Id", "Nome");
        }
    }

    private static SelectList ObterDiasSemanaParaSelect()
    {
        var diasSemana = new List<object>
        {
            new { Value = (int)DiaSemana.Domingo, Text = "Domingo" },
            new { Value = (int)DiaSemana.Segunda, Text = "Segunda-feira" },
            new { Value = (int)DiaSemana.Terca, Text = "Terça-feira" },
            new { Value = (int)DiaSemana.Quarta, Text = "Quarta-feira" },
            new { Value = (int)DiaSemana.Quinta, Text = "Quinta-feira" },
            new { Value = (int)DiaSemana.Sexta, Text = "Sexta-feira" },
            new { Value = (int)DiaSemana.Sabado, Text = "Sábado" }
        };

        return new SelectList(diasSemana, "Value", "Text");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        try
        {
            _logger.LogInformation("Excluindo horário {Id} via Ajax pelo usuário {Usuario}", id, User.Identity?.Name);

            var horarioDto = await _horarioService.ObterPorIdAsync(id);
            if (horarioDto == null)
            {
                return Json(new { success = false, message = "Horário não encontrado." });
            }

            var sucesso = await _horarioService.DesativarAsync(id);
            
            if (sucesso)
            {
                _logger.LogInformation("Horário {HorarioId} '{DiaSemana} {HoraInicio}-{HoraFim}' excluído pelo usuário {Usuario} em {Data}", 
                    id, horarioDto.DiaSemana, horarioDto.HoraInicio, horarioDto.HoraFim, User.Identity?.Name, DateTime.UtcNow);

                return Json(new { 
                    success = true, 
                    message = "Horário excluído com sucesso!",
                    redirectUrl = Url.Action("Index")
                });
            }
            else
            {
                return Json(new { success = false, message = "Horário não encontrado." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir horário {Id} via Ajax", id);
            return Json(new { success = false, message = "Erro ao excluir o horário." });
        }
    }

    #endregion
} 