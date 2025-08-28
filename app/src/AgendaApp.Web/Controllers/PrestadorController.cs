using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.Enums;
using AgendaApp.Infraestrutura.Identity;
using AgendaApp.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AgendaApp.Aplicacao.Interfaces;

namespace AgendaApp.Web.Controllers;

[Authorize]
public class PrestadorController : Controller
{
    private readonly IMapper _mapper;
    private readonly ILogger<PrestadorController> _logger;
    private readonly IPrestadorService _prestadorService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILojaRepository _lojaRepository;
    private readonly IServicoService _servicoService;
    private readonly IPrestadorServicoService _prestadorServicoService;
    private readonly IHorarioDisponivelService _horarioService;

    public PrestadorController(
        IMapper mapper,
        ILogger<PrestadorController> logger,
        IPrestadorService prestadorService,
        UserManager<ApplicationUser> userManager,
        ILojaRepository lojaRepository,
        IServicoService servicoService,
        IPrestadorServicoService prestadorServicoService,
        IHorarioDisponivelService horarioService)
    {
        _mapper = mapper;
        _logger = logger;
        _prestadorService = prestadorService;
        _userManager = userManager;
        _lojaRepository = lojaRepository;
        _servicoService = servicoService;
        _prestadorServicoService = prestadorServicoService;
        _horarioService = horarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(PrestadorFiltroViewModel? filtro)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _logger.LogInformation("Listando prestadores com filtros: {Filtro} para usuário {Email}", filtro?.Nome, user.Email);

            var filtroDto = new PrestadorFiltroDto
            {
                Nome = filtro?.Nome,
                AreaAtuacao = filtro?.AreaAtuacao,
                Email = filtro?.Email,
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

            var prestadoresDto = await _prestadorService.ObterComFiltroAsync(filtroDto);

            var prestadores = prestadoresDto.Select(dto => new PrestadorListViewModel
            {
                Id = dto.Id,
                Nome = dto.Nome,
                CNPJ = dto.CNPJFormatado,
                AreaAtuacao = dto.AreaAtuacao,
                Email = dto.Email,
                Telefone = dto.Telefone,
                DataCriacao = dto.DataCriacao,
                Ativo = dto.Ativo,
                TotalServicos = dto.TotalServicos,
                TotalHorarios = dto.TotalHorarios
            }).ToList();

            ViewBag.Filtro = filtro ?? new PrestadorFiltroViewModel();
            return View(prestadores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar prestadores");
            TempData["ErrorMessage"] = "Erro ao carregar a lista de prestadores.";
            return View(new List<PrestadorListViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo detalhes do prestador {Id}", id);

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var servicosDto = await _servicoService.ObterPorPrestadorAsync(id);

            var viewModel = new PrestadorViewModel
            {
                Id = prestadorDto.Id,
                Nome = prestadorDto.Nome,
                CNPJ = prestadorDto.CNPJFormatado,
                AreaAtuacao = prestadorDto.AreaAtuacao,
                Email = prestadorDto.Email,
                Telefone = prestadorDto.Telefone,
                DataCriacao = prestadorDto.DataCriacao,
                Ativo = prestadorDto.Ativo,
                Servicos = _mapper.Map<List<ServicoListViewModel>>(servicosDto)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exibir detalhes do prestador {Id}", id);
            TempData["ErrorMessage"] = "Erro interno do servidor.";
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

        _logger.LogInformation("Exibindo formulário de criação de prestador para usuário {Email}", user.Email);

        var viewModel = new PrestadorViewModel();

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PrestadorViewModel viewModel)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (isValid, errorMessage) = _prestadorService.ValidarPermissoesCriacao(user.LojaId, viewModel.LojaId, user.EhUsuarioLoja);
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
                await PrepararViewBags(user, viewModel.LojaId);

                if (viewModel.ServicosCreate?.Any() == true)
                {
                    ViewBag.ServicosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.ServicosCreate);
                    _logger.LogInformation("Preservando {Count} serviços para recarregar na view", viewModel.ServicosCreate.Count);
                }

                if (viewModel.HorariosDisponiveis?.Any() == true)
                {
                    ViewBag.HorariosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.HorariosDisponiveis);
                    _logger.LogInformation("Preservando {Count} horários para recarregar na view", viewModel.HorariosDisponiveis.Count);
                }

                return View(viewModel);
            }

            var prestadorCriado = await _prestadorService.CriarAsync(new()
            {
                Nome = viewModel.Nome,
                CNPJ = viewModel.CNPJ,
                AreaAtuacao = viewModel.AreaAtuacao,
                Email = viewModel.Email,
                Telefone = viewModel.Telefone,
                LojaId = viewModel.LojaId
            });

            int totalServicosAdicionados = 0;

            if (viewModel.ServicosCreate != null && viewModel.ServicosCreate.Any())
            {
                foreach (var servicoViewModel in viewModel.ServicosCreate)
                {
                    await _servicoService.CriarAsync(new CriarServicoDto
                    {
                        Nome = servicoViewModel.Nome,
                        Descricao = servicoViewModel.Descricao,
                        Valor = servicoViewModel.Valor,
                        DuracaoEmMinutos = servicoViewModel.DuracaoEmMinutos,
                        PrestadorId = prestadorCriado.Id,
                        LojaId = viewModel.LojaId
                    });
                }
                totalServicosAdicionados += viewModel.ServicosCreate.Count;
            }

            if (viewModel.ServicosVinculados != null && viewModel.ServicosVinculados.Any())
            {
                foreach (var vinculoViewModel in viewModel.ServicosVinculados)
                {
                    await _prestadorServicoService.VincularServicoAsync(new VincularServicoDto
                    {
                        PrestadorId = prestadorCriado.Id,
                        ServicoId = vinculoViewModel.ServicoId,
                        ValorPersonalizado = vinculoViewModel.ValorPersonalizado,
                        DuracaoPersonalizadaEmMinutos = vinculoViewModel.DuracaoPersonalizadaEmMinutos
                    });
                }
                totalServicosAdicionados += viewModel.ServicosVinculados.Count;
            }

            // Criar horários disponíveis
            int totalHorariosAdicionados = 0;
            if (viewModel.HorariosDisponiveis != null && viewModel.HorariosDisponiveis.Any())
            {
                foreach (var horarioViewModel in viewModel.HorariosDisponiveis)
                {
                    if (TimeOnly.TryParse(horarioViewModel.HoraInicio, out var horaInicio) && 
                        TimeOnly.TryParse(horarioViewModel.HoraFim, out var horaFim))
                    {
                        await _horarioService.CriarAsync(new CriarHorarioDisponivelDto
                        {
                                                    DiaSemana = (DiaSemana)horarioViewModel.DiaSemana,
                        HoraInicio = horaInicio,
                        HoraFim = horaFim,
                        PrestadorId = prestadorCriado.Id,
                        LojaId = viewModel.LojaId
                        });
                        totalHorariosAdicionados++;
                    }
                }
            }

            // Preparar mensagem de sucesso
            var mensagemSucesso = $"Prestador '{prestadorCriado.Nome}' criado com sucesso!";
            if (totalServicosAdicionados > 0)
            {
                mensagemSucesso += $" {totalServicosAdicionados} serviço(s) adicionado(s).";
            }
            if (totalHorariosAdicionados > 0)
            {
                mensagemSucesso += $" {totalHorariosAdicionados} horário(s) configurado(s).";
            }

            TempData["SuccessMessage"] = mensagemSucesso;

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar prestador");
            if (ex.Message.Contains("e-mail"))
                ModelState.AddModelError(nameof(viewModel.Email), ex.Message);
            else if (ex.Message.Contains("CNPJ"))
                ModelState.AddModelError(nameof(viewModel.CNPJ), ex.Message);
            else
                ModelState.AddModelError("", ex.Message);

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await PrepararViewBags(user, viewModel.LojaId);

                if (viewModel.ServicosCreate?.Any() == true)
                {
                    ViewBag.ServicosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.ServicosCreate);
                }

                if (viewModel.HorariosDisponiveis?.Any() == true)
                {
                    ViewBag.HorariosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.HorariosDisponiveis);
                }
            }
            return View(viewModel);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de argumentos ao criar prestador");
            ModelState.AddModelError("", ex.Message);

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await PrepararViewBags(user, viewModel.LojaId);

                if (viewModel.ServicosCreate?.Any() == true)
                {
                    ViewBag.ServicosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.ServicosCreate);
                }

                if (viewModel.HorariosDisponiveis?.Any() == true)
                {
                    ViewBag.HorariosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.HorariosDisponiveis);
                }
            }
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar prestador");
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await PrepararViewBags(user, viewModel.LojaId);

                if (viewModel.ServicosCreate?.Any() == true)
                {
                    ViewBag.ServicosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.ServicosCreate);
                }

                if (viewModel.HorariosDisponiveis?.Any() == true)
                {
                    ViewBag.HorariosParaRecarregar = System.Text.Json.JsonSerializer.Serialize(viewModel.HorariosDisponiveis);
                }
            }
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo formulário de edição do prestador {Id}", id);

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var servicosDto = await _servicoService.ObterPorPrestadorAsync(id);

            var viewModel = new PrestadorViewModel
            {
                Id = prestadorDto.Id,
                Nome = prestadorDto.Nome,
                CNPJ = prestadorDto.CNPJFormatado,
                AreaAtuacao = prestadorDto.AreaAtuacao,
                Email = prestadorDto.Email,
                Telefone = prestadorDto.Telefone,
                DataCriacao = prestadorDto.DataCriacao,
                Ativo = prestadorDto.Ativo,
                Servicos = _mapper.Map<List<ServicoListViewModel>>(servicosDto)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar prestador para edição {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do prestador.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PrestadorViewModel viewModel)
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

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _prestadorService.ValidarPermissoesEdicao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para editar este prestador.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para edição de prestador {Id}", id);
                return View(viewModel);
            }

            _logger.LogInformation("Atualizando prestador {Id}: {Nome}", id, viewModel.Nome);

            var dto = new AtualizarPrestadorDto
            {
                Id = viewModel.Id,
                Nome = viewModel.Nome,
                CNPJ = viewModel.CNPJ,
                AreaAtuacao = viewModel.AreaAtuacao,
                Email = viewModel.Email,
                Telefone = viewModel.Telefone,
                Ativo = viewModel.Ativo
            };

            var prestadorAtualizado = await _prestadorService.AtualizarAsync(dto);

            TempData["SuccessMessage"] = $"Prestador '{prestadorAtualizado.Nome}' atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao atualizar prestador {Id}", id);
            if (ex.Message.Contains("e-mail"))
                ModelState.AddModelError(nameof(viewModel.Email), ex.Message);
            else if (ex.Message.Contains("CNPJ"))
                ModelState.AddModelError(nameof(viewModel.CNPJ), ex.Message);
            else
                ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de argumentos ao atualizar prestador {Id}", id);
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar prestador {Id}", id);
            ModelState.AddModelError("", "Erro interno do servidor. Tente novamente.");
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Exibindo confirmação de exclusão do prestador {Id}", id);

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PrestadorViewModel
            {
                Id = prestadorDto.Id,
                Nome = prestadorDto.Nome,
                CNPJ = prestadorDto.CNPJFormatado,
                AreaAtuacao = prestadorDto.AreaAtuacao,
                Email = prestadorDto.Email,
                Telefone = prestadorDto.Telefone,
                DataCriacao = prestadorDto.DataCriacao,
                Ativo = prestadorDto.Ativo
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar prestador para exclusão {Id}", id);
            TempData["ErrorMessage"] = "Erro ao carregar os dados do prestador.";
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

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var (isValid, errorMessage) = _prestadorService.ValidarPermissoesExclusao(user.LojaId, prestadorDto.LojaId, user.EhUsuarioLoja);
            if (!isValid)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Você não tem permissão para excluir este prestador.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Excluindo prestador {Id}", id);

            var sucesso = await _prestadorService.DesativarAsync(id);
            if (!sucesso)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado ou já foi excluído.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Prestador excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir prestador {Id}", id);
            TempData["ErrorMessage"] = "Erro ao excluir o prestador.";
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

    #region Métodos para Gerenciar Serviços

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarServico(AdicionarServicoModalViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray(),
                        AttemptedValue = x.Value.AttemptedValue?.ToString()
                    })
                    .ToArray();

                return BadRequest(new
                {
                    success = false,
                    message = "Dados inválidos",
                    errors = errors
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuário não autenticado" });
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(model.PrestadorId);
            if (prestadorDto == null)
            {
                return NotFound(new { message = "Prestador não encontrado" });
            }

            var criarServicoDto = new CriarServicoDto
            {
                Nome = model.Nome,
                Descricao = model.Descricao ?? string.Empty,
                Valor = model.Valor,
                DuracaoEmMinutos = model.DuracaoEmMinutos,
                PrestadorId = model.PrestadorId,
                LojaId = user.LojaId
            };

            var servicoCriado = await _servicoService.CriarAsync(criarServicoDto);

            _logger.LogInformation("Serviço {ServicoNome} adicionado ao prestador {PrestadorId} pelo usuário {UserId}",
                servicoCriado.Nome, model.PrestadorId, user.Id);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar serviço ao prestador {PrestadorId}", model.PrestadorId);
            return BadRequest(new { message = "Erro interno do servidor", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> RemoverServico(Guid servicoId, Guid prestadorId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var prestadorDto = await _prestadorService.ObterPorIdAsync(prestadorId);
            if (prestadorDto == null)
            {
                TempData["ErrorMessage"] = "Prestador não encontrado.";
                return RedirectToAction(nameof(Details), new { id = prestadorId });
            }

            var desativado = await _servicoService.DesativarAsync(servicoId);

            if (desativado)
            {
                _logger.LogInformation("Serviço {ServicoId} removido do prestador {PrestadorId} pelo usuário {UserId}",
                    servicoId, prestadorId, user.Id);
                TempData["SuccessMessage"] = "Serviço removido com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erro ao remover o serviço.";
            }

            return RedirectToAction(nameof(Details), new { id = prestadorId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover serviço {ServicoId} do prestador {PrestadorId}", servicoId, prestadorId);
            TempData["ErrorMessage"] = "Erro ao remover o serviço.";
            return RedirectToAction(nameof(Details), new { id = prestadorId });
        }
    }

    #endregion

    #region Debug Temporário

    [HttpPost]
    public IActionResult TesteAdicionarServico(AdicionarServicoModalViewModel model)
    {
        _logger.LogInformation("=== TESTE DEBUG ADICIONAR SERVIÇO ===");
        _logger.LogInformation("Model recebido: {@Model}", model);
        _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray(),
                    AttemptedValue = x.Value.AttemptedValue?.ToString()
                })
                .ToArray();

            _logger.LogWarning("Erros de validação: {@Errors}", errors);

            return Ok(new
            {
                success = false,
                message = "Teste - Dados inválidos",
                errors = errors,
                modelData = new
                {
                    model.Nome,
                    model.Valor,
                    model.DuracaoEmMinutos,
                    model.PrestadorId,
                    model.Ativo,
                    model.Descricao
                }
            });
        }

        return Ok(new
        {
            success = true,
            message = "Teste - Dados válidos!",
            modelData = new
            {
                model.Nome,
                model.Valor,
                model.DuracaoEmMinutos,
                model.PrestadorId,
                model.Ativo,
                model.Descricao
            }
        });
    }

    #endregion

    #region Métodos para Gerenciar Vinculação de Serviços Globais

    [HttpGet]
    public async Task<IActionResult> ObterServicosGlobaisDisponiveis(Guid prestadorId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var servicosDto = await _prestadorServicoService.ObterServicosGlobaisDisponiveisAsync(prestadorId);
            var servicosViewModel = _mapper.Map<List<ServicoGlobalDisponivelViewModel>>(servicosDto);

            return Json(new { success = true, data = servicosViewModel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter serviços globais disponíveis para prestador {PrestadorId}", prestadorId);
            return Json(new { success = false, message = "Erro ao carregar serviços disponíveis." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterServicosVinculados(Guid prestadorId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var servicosDto = await _prestadorServicoService.ObterServicosVinculadosAsync(prestadorId);
            var servicosViewModel = _mapper.Map<List<ServicoVinculadoViewModel>>(servicosDto);

            return Json(new { success = true, data = servicosViewModel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter serviços vinculados ao prestador {PrestadorId}", prestadorId);
            return Json(new { success = false, message = "Erro ao carregar serviços vinculados." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VincularServico(VincularServicoViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                    .ToArray();

                return Json(new { success = false, message = "Dados inválidos", errors = errors });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var dto = _mapper.Map<VincularServicoDto>(viewModel);
            var servicoVinculado = await _prestadorServicoService.VincularServicoAsync(dto);

            _logger.LogInformation("Serviço {ServicoId} vinculado ao prestador {PrestadorId} pelo usuário {UserId}",
                dto.ServicoId, dto.PrestadorId, user.Id);

            return Json(new
            {
                success = true,
                message = $"Serviço '{servicoVinculado.ServicoNome}' vinculado com sucesso!",
                data = _mapper.Map<ServicoVinculadoViewModel>(servicoVinculado)
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao vincular serviço");
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular serviço ao prestador {PrestadorId}", viewModel.PrestadorId);
            return Json(new { success = false, message = "Erro interno do servidor." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarServicoVinculado(EditarServicoVinculadoViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                    .ToArray();

                return Json(new { success = false, message = "Dados inválidos", errors = errors });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var dto = _mapper.Map<AtualizarServicoVinculadoDto>(viewModel);
            var servicoAtualizado = await _prestadorServicoService.AtualizarServicoVinculadoAsync(dto);

            _logger.LogInformation("Valores personalizados do serviço vinculado {PrestadorServicoId} atualizados pelo usuário {UserId}",
                dto.PrestadorServicoId, user.Id);

            return Json(new
            {
                success = true,
                message = $"Valores personalizados do serviço '{servicoAtualizado.ServicoNome}' atualizados com sucesso!",
                data = _mapper.Map<ServicoVinculadoViewModel>(servicoAtualizado)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar serviço vinculado {PrestadorServicoId}", viewModel.PrestadorServicoId);
            return Json(new { success = false, message = "Erro interno do servidor." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoverVinculacaoServico(Guid prestadorServicoId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Usuário não autenticado." });
            }

            var sucesso = await _prestadorServicoService.RemoverVinculacaoAsync(prestadorServicoId);

            if (sucesso)
            {
                _logger.LogInformation("Vinculação de serviço {PrestadorServicoId} removida pelo usuário {Usuario} em {Data}",
                    prestadorServicoId, user.Email, DateTime.UtcNow);

                return Json(new { success = true, message = "Vinculação removida com sucesso!" });
            }
            else
            {
                return Json(new { success = false, message = "Vinculação não encontrada ou já foi removida." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover vinculação {PrestadorServicoId}", prestadorServicoId);
            return Json(new { success = false, message = "Erro ao remover vinculação." });
        }
    }

    #endregion

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

            var prestadorDto = await _prestadorService.ObterPorIdAsync(id);
            if (prestadorDto == null)
            {
                return Json(new { success = false, message = "Prestador não encontrado." });
            }

            _logger.LogInformation("Excluindo prestador {Id} via Ajax pelo usuário {Email}", id, user.Email);

            var sucesso = await _prestadorService.DesativarAsync(id);
            if (!sucesso)
            {
                return Json(new { success = false, message = "Prestador não encontrado ou já foi excluído." });
            }

            _logger.LogInformation("Prestador {PrestadorId} '{PrestadorNome}' excluído pelo usuário {Usuario} em {Data}",
                id, prestadorDto.Nome, user.Email, DateTime.UtcNow);

            return Json(new
            {
                success = true,
                message = "Prestador excluído com sucesso!",
                redirectUrl = Url.Action("Index")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir prestador {Id} via Ajax", id);
            return Json(new { success = false, message = "Erro ao excluir o prestador." });
        }
    }
}