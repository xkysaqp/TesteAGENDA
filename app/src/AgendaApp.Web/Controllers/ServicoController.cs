using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgendaApp.Web.Controllers;

[Authorize]
public class ServicoController : Controller
{
    private readonly IMapper _mapper;
    private readonly ILogger<ServicoController> _logger;
    private readonly IServicoService _servicoService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILojaRepository _lojaRepository;
    private readonly IPrestadorService _prestadorService;

    public ServicoController(
        IMapper mapper, 
        ILogger<ServicoController> logger,
        IServicoService servicoService,
        UserManager<ApplicationUser> userManager,
        ILojaRepository lojaRepository,
        IPrestadorService prestadorService)
    {
        _mapper = mapper;
        _logger = logger;
        _servicoService = servicoService;
        _userManager = userManager;
        _lojaRepository = lojaRepository;
        _prestadorService = prestadorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ServicoFiltroViewModel? filtro)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Listando serviços com filtros: {Filtro} para usuário {Email}", filtro?.Nome, user.Email);

            var filtroDto = new ServicoFiltroDto
            {
                Nome = filtro?.Nome,
                PrestadorId = filtro?.PrestadorId,
                ValorMinimo = filtro?.ValorMinimo,
                ValorMaximo = filtro?.ValorMaximo,
                DuracaoMaxima = filtro?.DuracaoMaxima,
                ApenasAtivos = filtro?.ApenasAtivos ?? true
            };

            if (user.EhUsuarioLoja)
            {
                filtroDto.LojaId = user.LojaId;
                ViewBag.IsLojaUser = true;
                ViewBag.LojaId = user.LojaId;
            }
            else
            {
                if (filtro?.LojaId.HasValue == true)
                {
                    filtroDto.LojaId = filtro.LojaId;
                }
                ViewBag.IsLojaUser = false;
                ViewBag.Lojas = await ObterLojasParaSelect();
            }

            var servicosDto = await _servicoService.ObterComFiltroAsync(filtroDto);

            var servicos = servicosDto.Select(dto => new ServicoListViewModel
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                DuracaoEmMinutos = dto.DuracaoEmMinutos,
                PrestadorId = dto.PrestadorId,
                PrestadorNome = dto.PrestadorNome ?? string.Empty,
                DataCriacao = dto.DataCriacao,
                Ativo = dto.Ativo
            }).ToList();

            ViewBag.Filtro = filtro ?? new ServicoFiltroViewModel();
            ViewBag.Prestadores = await ObterPrestadoresParaSelect(user);

            return View(servicos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar serviços");
            TempData["ErrorMessage"] = "Erro ao carregar a lista de serviços.";
            return View(new List<ServicoListViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Exibindo detalhes do serviço {Id} para usuário {Email}", id, user.Email);

            var servicoDto = await _servicoService.ObterPorIdAsync(id);
            if (servicoDto == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _servicoService.PodeAcessarServico(user.LojaId, servicoDto.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para acessar este serviço.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ServicoViewModel
            {
                Id = servicoDto.Id,
                Nome = servicoDto.Nome,
                Descricao = servicoDto.Descricao,
                Valor = servicoDto.Valor,
                DuracaoEmMinutos = servicoDto.DuracaoEmMinutos,
                PrestadorId = servicoDto.PrestadorId,
                DataCriacao = servicoDto.DataCriacao,
                Ativo = servicoDto.Ativo
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exibir detalhes do serviço {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os detalhes do serviço.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        _logger.LogInformation("Exibindo formulário de criação de serviço para usuário {Email}", user.Email);
        
        var viewModel = new ServicoViewModel();

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

        ViewBag.Prestadores = await ObterPrestadoresParaSelect(user);
        
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServicoViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (isValid, errorMessage) = _servicoService.ValidarPermissoesCriacao(user.LojaId, viewModel.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ModelState.AddModelError("", errorMessage);
                }
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para criação de serviço");
                await PrepararViewBags(user, viewModel.LojaId);
                return View(viewModel);
            }

            _logger.LogInformation("Criando serviço: {Nome} para prestador {PrestadorId} na loja {LojaId}", 
                viewModel.Nome, viewModel.PrestadorId, viewModel.LojaId);

            var dto = new CriarServicoDto
            {
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao,
                Valor = viewModel.Valor,
                DuracaoEmMinutos = viewModel.DuracaoEmMinutos,
                PrestadorId = viewModel.PrestadorId,
                LojaId = viewModel.LojaId
            };

            var servicoCriado = await _servicoService.CriarAsync(dto);

            TempData["SuccessMessage"] = $"Serviço '{servicoCriado.Nome}' criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar serviço");
            ModelState.AddModelError("", ex.Message);
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de argumentos ao criar serviço");
            ModelState.AddModelError("", ex.Message);
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar serviço");
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Exibindo formulário de edição do serviço {Id} para usuário {Email}", id, user.Email);

            var servicoDto = await _servicoService.ObterPorIdAsync(id);
            if (servicoDto == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _servicoService.PodeAcessarServico(user.LojaId, servicoDto.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para editar este serviço.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ServicoViewModel
            {
                Id = servicoDto.Id,
                Nome = servicoDto.Nome,
                Descricao = servicoDto.Descricao,
                Valor = servicoDto.Valor,
                DuracaoEmMinutos = servicoDto.DuracaoEmMinutos,
                PrestadorId = servicoDto.PrestadorId,
                DataCriacao = servicoDto.DataCriacao,
                Ativo = servicoDto.Ativo
            };

            await PrepararViewBags(user, viewModel.LojaId);
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar serviço para edição {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do serviço.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ServicoViewModel viewModel)
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

            var servicoAtual = await _servicoService.ObterPorIdAsync(id);
            if (servicoAtual == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _servicoService.ValidarPermissoesEdicao(user.LojaId, servicoAtual.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para editar este serviço.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para edição de serviço {Id}", id);
                await PrepararViewBags(user, viewModel.LojaId);
                return View(viewModel);
            }

            _logger.LogInformation("Atualizando serviço {Id}: {Nome} para usuário {Email}", id, viewModel.Nome, user.Email);

            var dto = new AtualizarServicoDto
            {
                Id = viewModel.Id,
                Nome = viewModel.Nome,
                Descricao = viewModel.Descricao,
                Valor = viewModel.Valor,
                DuracaoEmMinutos = viewModel.DuracaoEmMinutos,
                PrestadorId = viewModel.PrestadorId,
                Ativo = viewModel.Ativo,
                LojaId = servicoAtual.LojaId
            };

            var servicoAtualizado = await _servicoService.AtualizarAsync(dto);

            TempData["SuccessMessage"] = $"Serviço '{servicoAtualizado.Nome}' atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao atualizar serviço {Id}", id);
            ModelState.AddModelError("", ex.Message);
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de argumentos ao atualizar serviço {Id}", id);
            ModelState.AddModelError("", ex.Message);
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar serviço {Id}", id);
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null) await PrepararViewBags(user, viewModel.LojaId);
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Exibindo confirmação de exclusão do serviço {Id} para usuário {Email}", id, user.Email);

            var servicoDto = await _servicoService.ObterPorIdAsync(id);
            if (servicoDto == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _servicoService.PodeAcessarServico(user.LojaId, servicoDto.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para excluir este serviço.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ServicoViewModel
            {
                Id = servicoDto.Id,
                Nome = servicoDto.Nome,
                Descricao = servicoDto.Descricao,
                Valor = servicoDto.Valor,
                DuracaoEmMinutos = servicoDto.DuracaoEmMinutos,
                PrestadorId = servicoDto.PrestadorId,
                DataCriacao = servicoDto.DataCriacao,
                Ativo = servicoDto.Ativo
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar serviço para exclusão {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do serviço.";
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

            _logger.LogInformation("Excluindo serviço {Id} para usuário {Email}", id, user.Email);

            var servicoDto = await _servicoService.ObterPorIdAsync(id);
            if (servicoDto == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _servicoService.ValidarPermissoesExclusao(user.LojaId, servicoDto.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para excluir este serviço.";
                return RedirectToAction(nameof(Index));
            }

            var sucesso = await _servicoService.DesativarAsync(id);
            if (!sucesso)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado ou já foi excluído.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Serviço '{servicoDto.Nome}' excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir serviço {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir o serviço.";
            return RedirectToAction(nameof(Index));
        }
    }

    #region Métodos Auxiliares

    private async Task<SelectList> ObterLojasParaSelect()
    {
        try
        {
            var lojas = await _lojaRepository.ObterTodosAsync();
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
            
            var loja = await _lojaRepository.ObterPorIdAsync(lojaId.Value);
            return loja?.Nome;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter nome da loja {LojaId}", lojaId);
            return null;
        }
    }

    private async Task<SelectList> ObterPrestadoresParaSelect(ApplicationUser user)
    {
        try
        {
            var filtro = new PrestadorFiltroDto { ApenasAtivos = true };
            
            if (user.EhUsuarioLoja)
            {
                filtro.LojaId = user.LojaId;
            }

            var prestadores = await _prestadorService.ObterComFiltroAsync(filtro);
            return new SelectList(
                prestadores.OrderBy(p => p.Nome)
                          .Select(p => new { Id = p.Id, Nome = p.Nome }), 
                "Id", 
                "Nome");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar prestadores para select");
            return new SelectList(new List<object>(), "Id", "Nome");
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

        ViewBag.Prestadores = await ObterPrestadoresParaSelect(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            _logger.LogInformation("Excluindo serviço {Id} via Ajax pelo usuário {Email}", id, user.Email);

            var servicoDto = await _servicoService.ObterPorIdAsync(id);
            if (servicoDto == null)
            {
                return Json(new { success = false, message = "Serviço não encontrado." });
            }

            var (isValid, errorMessage) = _servicoService.ValidarPermissoesExclusao(user.LojaId, servicoDto.LojaId ?? Guid.Empty, user.EhUsuarioLoja);
            if (!isValid)
            {
                return Json(new { success = false, message = errorMessage ?? "Você não tem permissão para excluir este serviço." });
            }

            var sucesso = await _servicoService.DesativarAsync(id);
            if (!sucesso)
            {
                return Json(new { success = false, message = "Serviço não encontrado ou já foi excluído." });
            }

            _logger.LogInformation("Serviço {ServicoId} '{ServicoNome}' excluído pelo usuário {Usuario} em {Data}", 
                id, servicoDto.Nome, user.Email, DateTime.UtcNow);

            return Json(new { 
                success = true, 
                message = $"Serviço '{servicoDto.Nome}' excluído com sucesso!",
                redirectUrl = Url.Action("Index")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir serviço {Id} via Ajax", id);
            return Json(new { success = false, message = "Erro ao excluir o serviço." });
        }
    }

    #endregion
} 