using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Constants;
using AgendaApp.Web.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApp.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class LojaController : Controller
{
    private readonly ILojaService _lojaService;
    private readonly IMapper _mapper;
    private readonly ILogger<LojaController> _logger;

    public LojaController(
        ILojaService lojaService,
        IMapper mapper,
        ILogger<LojaController> logger)
    {
        _lojaService = lojaService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var lojasDto = await _lojaService.ObterTodasAsync();
            var lojas = _mapper.Map<IEnumerable<LojaViewModel>>(lojasDto);
            return View(lojas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista de lojas");
            TempData["Error"] = "Erro ao carregar a lista de lojas.";
            return View(new List<LojaViewModel>());
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var lojaDto = await _lojaService.ObterPorIdAsync(id);
            if (lojaDto == null)
            {
                TempData["Error"] = "Loja não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var loja = _mapper.Map<LojaViewModel>(lojaDto);
            return View(loja);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar detalhes da loja {LojaId}", id);
            TempData["Error"] = "Erro ao carregar os detalhes da loja.";
            return RedirectToAction(nameof(Index));
        }
    }

    public IActionResult Create()
    {
        return View(new CriarLojaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CriarLojaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (await _lojaService.SlugJaExisteAsync(model.Slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "Este slug já está sendo usado por outra loja.");
                return View(model);
            }

            if (await _lojaService.CnpjJaExisteAsync(model.CNPJ))
            {
                ModelState.AddModelError(nameof(model.CNPJ), "Este CNPJ já está cadastrado para outra loja.");
                return View(model);
            }

            var criarLojaDto = _mapper.Map<CriarLojaDto>(model);
            var lojaDto = await _lojaService.CriarAsync(criarLojaDto);

            TempData["Success"] = $"Loja '{lojaDto.Nome}' criada com sucesso!";
            return RedirectToAction(nameof(Details), new { id = lojaDto.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar loja {Nome}", model.Nome);
            ModelState.AddModelError("", "Erro interno. Tente novamente.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var lojaDto = await _lojaService.ObterPorIdAsync(id);
            if (lojaDto == null)
            {
                TempData["Error"] = "Loja não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var model = _mapper.Map<EditarLojaViewModel>(lojaDto);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar formulário de edição da loja {LojaId}", id);
            TempData["Error"] = "Erro ao carregar o formulário de edição.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditarLojaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (await _lojaService.SlugJaExisteAsync(model.Slug, model.Id))
            {
                ModelState.AddModelError(nameof(model.Slug), "Este slug já está sendo usado por outra loja.");
                return View(model);
            }

            if (await _lojaService.CnpjJaExisteAsync(model.CNPJ, model.Id))
            {
                ModelState.AddModelError(nameof(model.CNPJ), "Este CNPJ já está cadastrado para outra loja.");
                return View(model);
            }

            var atualizarLojaDto = _mapper.Map<AtualizarLojaDto>(model);
            var lojaDto = await _lojaService.AtualizarAsync(atualizarLojaDto);

            TempData["Success"] = $"Loja '{lojaDto.Nome}' atualizada com sucesso!";
            return RedirectToAction(nameof(Details), new { id = lojaDto.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar loja {LojaId}", model.Id);
            ModelState.AddModelError("", "Erro interno. Tente novamente.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var lojaDto = await _lojaService.ObterPorIdAsync(id);
            if (lojaDto == null)
            {
                TempData["Error"] = "Loja não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var loja = _mapper.Map<LojaViewModel>(lojaDto);
            return View(loja);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar confirmação de exclusão da loja {LojaId}", id);
            TempData["Error"] = "Erro ao carregar a confirmação de exclusão.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var lojaDto = await _lojaService.ObterPorIdAsync(id);
            if (lojaDto == null)
            {
                TempData["Error"] = "Loja não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var sucesso = await _lojaService.ExcluirAsync(id);
            if (sucesso)
            {
                TempData["Success"] = $"Loja '{lojaDto.Nome}' excluída com sucesso!";
            }
            else
            {
                TempData["Error"] = "Erro ao excluir a loja.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir loja {LojaId}", id);
            TempData["Error"] = "Erro interno. Não foi possível excluir a loja.";
            return RedirectToAction(nameof(Index));
        }
    }


    public async Task<IActionResult> LojasVencidas()
    {
        try
        {
            var lojasDto = await _lojaService.ObterLojasVencidasAsync();
            var lojas = _mapper.Map<IEnumerable<LojaViewModel>>(lojasDto);
            ViewBag.Title = "Lojas com Assinatura Vencida";
            return View("Index", lojas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lojas vencidas");
            TempData["Error"] = "Erro ao carregar lojas vencidas.";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> LojasProximasVencimento()
    {
        try
        {
            var lojasDto = await _lojaService.ObterLojasProximasVencimentoAsync();
            var lojas = _mapper.Map<IEnumerable<LojaViewModel>>(lojasDto);
            ViewBag.Title = "Lojas Próximas do Vencimento";
            return View("Index", lojas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lojas próximas do vencimento");
            TempData["Error"] = "Erro ao carregar lojas próximas do vencimento.";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpGet]
    public async Task<IActionResult> ValidarSlug(string slug, Guid? id)
    {
        if (string.IsNullOrEmpty(slug))
            return Json(false);

        var existe = await _lojaService.SlugJaExisteAsync(slug, id);
        return Json(!existe);
    }

    [HttpGet]
    public async Task<IActionResult> ValidarCnpj(string cnpj, Guid? id)
    {
        if (string.IsNullOrEmpty(cnpj))
            return Json(false);

        var existe = await _lojaService.CnpjJaExisteAsync(cnpj, id);
        return Json(!existe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(Guid id)
    {
        try
        {
            var lojaDto = await _lojaService.ObterPorIdAsync(id);
            if (lojaDto == null)
            {
                return Json(new { success = false, message = "Loja não encontrada." });
            }

            var sucesso = await _lojaService.ExcluirAsync(id);
            if (sucesso)
            {
                _logger.LogInformation("Loja {LojaId} '{LojaNome}' excluída pelo usuário {Usuario} em {Data}", 
                    id, lojaDto.Nome, User.Identity?.Name, DateTime.UtcNow);

                return Json(new { 
                    success = true, 
                    message = $"Loja '{lojaDto.Nome}' excluída com sucesso!",
                    redirectUrl = Url.Action("Index")
                });
            }
            else
            {
                return Json(new { success = false, message = "Erro ao excluir a loja." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir loja {LojaId} via Ajax pelo usuário {Usuario}", id, User.Identity?.Name);
            return Json(new { success = false, message = "Erro interno. Não foi possível excluir a loja." });
        }
    }
} 