using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Enums;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Web.Controllers;

public class AgendamentoPublicoController : Controller
{
    private readonly ILojaService _lojaService;
    private readonly IPrestadorService _prestadorService;
    private readonly IServicoService _servicoService;
    private readonly IHorarioDisponivelService _horarioService;
    private readonly IAgendamentoService _agendamentoService;
    private readonly ILogger<AgendamentoPublicoController> _logger;

    public AgendamentoPublicoController(
        ILojaService lojaService,
        IPrestadorService prestadorService,
        IServicoService servicoService,
        IHorarioDisponivelService horarioService,
        IAgendamentoService agendamentoService,
        ILogger<AgendamentoPublicoController> logger)
    {
        _lojaService = lojaService;
        _prestadorService = prestadorService;
        _servicoService = servicoService;
        _horarioService = horarioService;
        _agendamentoService = agendamentoService;
        _logger = logger;
    }

    [HttpGet]
    [Route("agendamento/{lojaSlug}")]
    [Route("agendamento/loja/{lojaId}")]
    public async Task<IActionResult> Index(string? lojaSlug, Guid? lojaId)
    {
        try
        {
            LojaDto? loja = null;

            if (!string.IsNullOrEmpty(lojaSlug))
            {
                _logger.LogInformation("Acessando agendamento público para loja por slug: {LojaSlug}", lojaSlug);
                loja = await _lojaService.ObterPorSlugAsync(lojaSlug);
            }
            else if (lojaId.HasValue)
            {
                _logger.LogInformation("Acessando agendamento público para loja por ID: {LojaId}", lojaId);
                loja = await _lojaService.ObterPorIdAsync(lojaId.Value);
            }

            if (loja == null)
            {
                _logger.LogWarning("Loja não encontrada para slug: {LojaSlug} ou ID: {LojaId}", lojaSlug, lojaId);
                return NotFound("Loja não encontrada");
            }

            // Buscar prestadores ativos da loja
            var prestadores = await _prestadorService.ObterComFiltroAsync(new PrestadorFiltroDto
            {
                LojaId = loja.Id,
                ApenasAtivos = true
            });

            // Buscar serviços disponíveis
            var servicos = await _servicoService.ObterComFiltroAsync(new ServicoFiltroDto { LojaId = loja.Id, ApenasAtivos = true });

            var viewModel = new AgendamentoPublicoViewModel
            {
                Loja = new LojaPublicaViewModel
                {
                    Id = loja.Id,
                    Nome = loja.Nome,
                    Slug = loja.Slug,
                    Descricao = string.Empty
                },
                Prestadores = prestadores.Select(p => new PrestadorPublicoViewModel
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    AreaAtuacao = p.AreaAtuacao
                }).ToList(),
                Servicos = servicos.Select(s => new ServicoPublicoViewModel
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    Valor = s.Valor,
                    DuracaoEmMinutos = s.DuracaoEmMinutos,
                    PrestadorId = s.PrestadorId ?? Guid.Empty
                }).ToList()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar agendamento público para loja: {LojaSlug}", lojaSlug);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet]
    [Route("agendamento/{lojaSlug}/horarios")]
    public async Task<IActionResult> ObterHorariosDisponiveis(
        string lojaSlug, 
        Guid prestadorId, 
        Guid servicoId, 
        DateOnly data)
    {
        try
        {
            _logger.LogInformation("Buscando horários disponíveis para loja: {LojaSlug}, prestador: {PrestadorId}, serviço: {ServicoId}, data: {Data}", 
                lojaSlug, prestadorId, servicoId, data);

            // Buscar loja pelo slug
            var loja = await _lojaService.ObterPorSlugAsync(lojaSlug);
            if (loja == null)
            {
                return NotFound("Loja não encontrada");
            }

            // Buscar horários disponíveis do prestador para a data
            var diaSemana = data.DayOfWeek switch
            {
                DayOfWeek.Sunday => DiaSemana.Domingo,
                DayOfWeek.Monday => DiaSemana.Segunda,
                DayOfWeek.Tuesday => DiaSemana.Terca,
                DayOfWeek.Wednesday => DiaSemana.Quarta,
                DayOfWeek.Thursday => DiaSemana.Quinta,
                DayOfWeek.Friday => DiaSemana.Sexta,
                DayOfWeek.Saturday => DiaSemana.Sabado,
                _ => DiaSemana.Segunda
            };
            
            var horariosDisponiveis = await _horarioService.ObterPorPrestadorEDiaAsync(prestadorId, diaSemana);

            // Buscar agendamentos existentes para a data
            var agendamentosExistentes = await _agendamentoService.ObterPorDataAsync(loja.Id, data);

            // Filtrar horários disponíveis (remover horários já agendados)
            var horariosLivres = new List<HorarioDisponivelDto>();
            foreach (var horario in horariosDisponiveis)
            {
                var horaInicio = horario.HoraInicio;
                var horaFim = horario.HoraFim;
                
                // Verificar se há conflitos com agendamentos existentes
                var conflitos = agendamentosExistentes.Where(a => 
                    a.PrestadorId == prestadorId && 
                    a.Status != StatusAgendamento.Cancelado &&
                    a.Hora < horaFim && a.Hora.AddMinutes(30) > horaInicio);

                if (!conflitos.Any())
                {
                    horariosLivres.Add(horario);
                }
            }

            return Json(new { success = true, data = horariosLivres });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar horários disponíveis");
            return Json(new { success = false, message = "Erro ao buscar horários disponíveis" });
        }
    }

    [HttpPost]
    [Route("agendamento/{lojaSlug}/criar")]
    public async Task<IActionResult> CriarAgendamento(string lojaSlug, [FromBody] CriarAgendamentoPublicoViewModel viewModel)
    {
        try
        {
            _logger.LogInformation("Criando agendamento público para loja: {LojaSlug}", lojaSlug);

            // Buscar loja pelo slug
            var loja = await _lojaService.ObterPorSlugAsync(lojaSlug);
            if (loja == null)
            {
                return NotFound("Loja não encontrada");
            }

            // Validar dados do agendamento
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Criar agendamento usando o serviço
            var criarAgendamentoDto = new CriarAgendamentoDto
            {
                Data = viewModel.Data,
                Hora = TimeOnly.Parse(viewModel.HoraInicio),
                ClienteNome = viewModel.NomeCliente,
                ClienteTelefone = viewModel.TelefoneCliente,
                ClienteEmail = viewModel.EmailCliente,
                Observacoes = viewModel.Observacoes,
                LojaId = loja.Id,
                ServicoId = viewModel.ServicoId,
                PrestadorId = viewModel.PrestadorId
            };

            var agendamento = await _agendamentoService.CriarAsync(criarAgendamentoDto);

            _logger.LogInformation("Agendamento criado com sucesso: {AgendamentoId}", agendamento.Id);

            return Json(new { 
                success = true, 
                message = "Agendamento criado com sucesso!",
                agendamentoId = agendamento.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar agendamento público");
            return Json(new { success = false, message = "Erro ao criar agendamento" });
        }
    }
}

