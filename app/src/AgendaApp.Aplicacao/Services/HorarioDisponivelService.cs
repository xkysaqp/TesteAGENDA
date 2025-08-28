using AutoMapper;
using Microsoft.Extensions.Logging;
using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Aplicacao.Interfaces;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Interfaces;
using AgendaApp.Dominio.Enums;

namespace AgendaApp.Aplicacao.Services;

public class HorarioDisponivelService : IHorarioDisponivelService
{
    private readonly IHorarioDisponivelRepository _horarioRepository;
    private readonly IPrestadorRepository _prestadorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<HorarioDisponivelService> _logger;

    public HorarioDisponivelService(
        IHorarioDisponivelRepository horarioRepository,
        IPrestadorRepository prestadorRepository,
        IMapper mapper,
        ILogger<HorarioDisponivelService> logger)
    {
        _horarioRepository = horarioRepository;
        _prestadorRepository = prestadorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HorarioDisponivelDto> CriarAsync(CriarHorarioDisponivelDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando horário disponível: {DiaSemana} {HoraInicio}-{HoraFim} para prestador {PrestadorId}", 
            dto.DiaSemana, dto.HoraInicio, dto.HoraFim, dto.PrestadorId);

        try
        {
            await ValidarDtoParaCriacaoAsync(dto, cancellationToken);

            var prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId, cancellationToken);
            if (prestador == null)
            {
                throw new ArgumentException($"Prestador com ID '{dto.PrestadorId}' não encontrado");
            }

            if (await ExisteConflitoAsync(dto.PrestadorId, dto.DiaSemana, dto.HoraInicio, dto.HoraFim, null, cancellationToken))
            {
                throw new InvalidOperationException($"Existe conflito com horário já cadastrado para o prestador '{prestador.Nome}' no {ObterTextodiaSemana(dto.DiaSemana)}");
            }

            var horario = new HorarioDisponivel(
                dto.DiaSemana,
                dto.HoraInicio,
                dto.HoraFim,
                dto.PrestadorId,
                dto.LojaId 
            );

            var horarioSalvo = await _horarioRepository.AdicionarAsync(horario, cancellationToken);
            await _horarioRepository.SalvarAsync(cancellationToken);

            _logger.LogInformation("Horário disponível criado com sucesso: {Id} - {DiaSemana} {HoraInicio}-{HoraFim}", 
                horarioSalvo.Id, horarioSalvo.DiaSemana, horarioSalvo.HoraInicio, horarioSalvo.HoraFim);

            return ConverterParaDto(horarioSalvo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar horário disponível");
            throw;
        }
    }

    public async Task<ResultadoCriacaoMultiplaDto> CriarMultiplosAsync(CriarHorarioMultiploDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando múltiplos horários para prestador {PrestadorId}: {DiasSemana}", 
            dto.PrestadorId, string.Join(", ", dto.DiasSemana));

        try
        {
            await ValidarDtoParaCriacaoMultiplaAsync(dto, cancellationToken);

            var prestador = await _prestadorRepository.ObterPorIdAsync(dto.PrestadorId, cancellationToken);
            if (prestador == null)
            {
                throw new ArgumentException($"Prestador com ID '{dto.PrestadorId}' não encontrado");
            }

            var resultado = new ResultadoCriacaoMultiplaDto();

            foreach (var dia in dto.DiasSemana)
            {
                if (await ExisteConflitoAsync(dto.PrestadorId, dia, dto.HoraInicio, dto.HoraFim, null, cancellationToken))
                {
                    resultado.Conflitos.Add($"{ObterTextodiaSemana(dia)}: {dto.HoraInicio:HH:mm} às {dto.HoraFim:HH:mm}");
                    continue;
                }

                try
                {
                    var horario = new HorarioDisponivel(dia, dto.HoraInicio, dto.HoraFim, dto.PrestadorId, dto.LojaId);
                    var horarioSalvo = await _horarioRepository.AdicionarAsync(horario, cancellationToken);
                    
                    resultado.HorariosAdicionados.Add(ConverterParaDto(horarioSalvo));
                    resultado.HorariosCriados++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erro ao criar horário para {Dia}", dia);
                    resultado.Conflitos.Add($"{ObterTextodiaSemana(dia)}: Erro interno ao criar horário");
                }
            }

            if (resultado.HorariosCriados > 0)
            {
                await _horarioRepository.SalvarAsync(cancellationToken);
            }

            _logger.LogInformation("Criação múltipla concluída: {HorariosCriados} horários criados, {Conflitos} conflitos", 
                resultado.HorariosCriados, resultado.Conflitos.Count);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na criação múltipla de horários");
            throw;
        }
    }

    public async Task<HorarioDisponivelDto> AtualizarAsync(AtualizarHorarioDisponivelDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando horário disponível: {Id}", dto.Id);

        try
        {
            var horario = await _horarioRepository.ObterPorIdAsync(dto.Id, cancellationToken);
            if (horario == null)
            {
                throw new ArgumentException($"Horário com ID '{dto.Id}' não encontrado");
            }

            await ValidarDtoParaAtualizacaoAsync(dto, cancellationToken);

            if (await ExisteConflitoAsync(dto.PrestadorId, dto.DiaSemana, dto.HoraInicio, dto.HoraFim, dto.Id, cancellationToken))
            {
                throw new InvalidOperationException("Existe conflito com outro horário já cadastrado");
            }

            horario.AtualizarHorario(dto.DiaSemana, dto.HoraInicio, dto.HoraFim);

            if (dto.Ativo && !horario.Ativo)
            {
                horario.Ativar();
            }
            else if (!dto.Ativo && horario.Ativo)
            {
                horario.Desativar();
            }

            var horarioAtualizado = await _horarioRepository.AtualizarAsync(horario, cancellationToken);
            await _horarioRepository.SalvarAsync(cancellationToken);

            _logger.LogInformation("Horário atualizado com sucesso: {Id}", horarioAtualizado.Id);

            return ConverterParaDto(horarioAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar horário: {Id}", dto.Id);
            throw;
        }
    }

    public async Task<HorarioDisponivelDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando horário por ID: {Id}", id);

        var horario = await _horarioRepository.ObterPorIdAsync(id, cancellationToken);
        return horario != null ? ConverterParaDto(horario) : null;
    }

    public async Task<IEnumerable<HorarioDisponivelResumoDto>> ObterComFiltroAsync(HorarioDisponivelFiltroDto filtro, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Buscando horários com filtros");

        var horarios = await _horarioRepository.ObterTodosAsync(cancellationToken);

        var resultado = horarios.AsEnumerable();

        if (filtro.PrestadorId.HasValue)
        {
            resultado = resultado.Where(h => h.PrestadorId == filtro.PrestadorId.Value);
        }

        if (filtro.DiaSemana.HasValue)
        {
            resultado = resultado.Where(h => h.DiaSemana == filtro.DiaSemana.Value);
        }

        if (filtro.HoraMinima.HasValue)
        {
            resultado = resultado.Where(h => h.HoraInicio >= filtro.HoraMinima.Value);
        }

        if (filtro.HoraMaxima.HasValue)
        {
            resultado = resultado.Where(h => h.HoraFim <= filtro.HoraMaxima.Value);
        }

        if (filtro.ApenasAtivos)
        {
            resultado = resultado.Where(h => h.Ativo);
        }

        return resultado.Select(ConverterParaResumoDto);
    }

    public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Desativando horário: {Id}", id);

        var horario = await _horarioRepository.ObterPorIdAsync(id, cancellationToken);
        if (horario == null) return false;

        horario.Desativar();
        await _horarioRepository.AtualizarAsync(horario, cancellationToken);
        await _horarioRepository.SalvarAsync(cancellationToken);

        _logger.LogInformation("Horário desativado: {Id}", horario.Id);
        return true;
    }

    public async Task<bool> ExisteConflitoAsync(Guid prestadorId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFim, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Verificando conflito de horário: {PrestadorId} {DiaSemana} {HoraInicio}-{HoraFim}", 
            prestadorId, diaSemana, horaInicio, horaFim);

        return await _horarioRepository.ExisteConflitoAsync(prestadorId, diaSemana, horaInicio, horaFim, excludeId, cancellationToken);
    }

    public async Task<HorarioEstatisticasDto> ObterEstatisticasAsync(Guid? prestadorId = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obtendo estatísticas de horários para prestador: {PrestadorId}", prestadorId);

        var estatisticas = await _horarioRepository.ObterEstatisticasAsync(prestadorId, cancellationToken);

        return new HorarioEstatisticasDto
        {
            TotalHorarios = estatisticas.TotalHorarios,
            HorariosPorDia = estatisticas.HorariosPorDia,
            HorarioInicioMaisEarly = estatisticas.HorarioInicioMaisEarly,
            HorarioFimMaisTarde = estatisticas.HorarioFimMaisTarde,
            DuracaoMediaMinutos = estatisticas.DuracaoMediaMinutos,
            DuracaoMediaFormatada = FormatarDuracao(estatisticas.DuracaoMediaMinutos),
            HorariosPorDiaSemana = estatisticas.HorariosPorDiaSemana
        };
    }

    private static async Task ValidarDtoParaCriacaoAsync(CriarHorarioDisponivelDto dto, CancellationToken cancellationToken)
    {
        if (dto.PrestadorId == Guid.Empty)
            throw new ArgumentException("Prestador é obrigatório");

        if (dto.HoraInicio >= dto.HoraFim)
            throw new ArgumentException("Hora de início deve ser anterior à hora de fim");

        var duracao = dto.HoraFim - dto.HoraInicio;
        if (duracao.TotalMinutes < 30)
            throw new ArgumentException("Período deve ter pelo menos 30 minutos");

        await Task.CompletedTask;
    }

    private static async Task ValidarDtoParaCriacaoMultiplaAsync(CriarHorarioMultiploDto dto, CancellationToken cancellationToken)
    {
        if (dto.PrestadorId == Guid.Empty)
            throw new ArgumentException("Prestador é obrigatório");

        if (!dto.DiasSemana.Any())
            throw new ArgumentException("Pelo menos um dia da semana deve ser selecionado");

        if (dto.HoraInicio >= dto.HoraFim)
            throw new ArgumentException("Hora de início deve ser anterior à hora de fim");

        await Task.CompletedTask;
    }

    private static async Task ValidarDtoParaAtualizacaoAsync(AtualizarHorarioDisponivelDto dto, CancellationToken cancellationToken)
    {
        if (dto.Id == Guid.Empty)
            throw new ArgumentException("ID é obrigatório para atualização");

        if (dto.PrestadorId == Guid.Empty)
            throw new ArgumentException("Prestador é obrigatório");

        if (dto.HoraInicio >= dto.HoraFim)
            throw new ArgumentException("Hora de início deve ser anterior à hora de fim");

        await Task.CompletedTask;
    }

    private static HorarioDisponivelDto ConverterParaDto(HorarioDisponivel horario)
    {
        return new HorarioDisponivelDto
        {
            Id = horario.Id,
            DiaSemana = horario.DiaSemana,
            DiaSemanaTexto = ObterTextodiaSemana(horario.DiaSemana),
            HoraInicio = horario.HoraInicio,
            HoraFim = horario.HoraFim,
            HorarioFormatado = $"{horario.HoraInicio:HH:mm} às {horario.HoraFim:HH:mm}",
            DuracaoEmMinutos = horario.DuracaoEmMinutos(),
            DuracaoFormatada = FormatarDuracao(horario.DuracaoEmMinutos()),
            PrestadorId = horario.PrestadorId,
            PrestadorNome = horario.Prestador?.Nome,
            DataCriacao = horario.DataCriacao,
            DataAtualizacao = horario.DataAtualizacao,
            Ativo = horario.Ativo
        };
    }

    private static HorarioDisponivelResumoDto ConverterParaResumoDto(HorarioDisponivel horario)
    {
        return new HorarioDisponivelResumoDto
        {
            Id = horario.Id,
            DiaSemana = horario.DiaSemana,
            DiaSemanaTexto = ObterTextodiaSemana(horario.DiaSemana),
            HoraInicio = horario.HoraInicio,
            HoraFim = horario.HoraFim,
            HorarioFormatado = $"{horario.HoraInicio:HH:mm} às {horario.HoraFim:HH:mm}",
            DuracaoFormatada = FormatarDuracao(horario.DuracaoEmMinutos()),
            PrestadorId = horario.PrestadorId,
            PrestadorNome = horario.Prestador?.Nome ?? string.Empty,
            DataCriacao = horario.DataCriacao,
            Ativo = horario.Ativo
        };
    }

    private static string ObterTextodiaSemana(DiaSemana diaSemana)
    {
        return diaSemana switch
        {
            DiaSemana.Segunda => "Segunda-feira",
            DiaSemana.Terca => "Terça-feira",
            DiaSemana.Quarta => "Quarta-feira",
            DiaSemana.Quinta => "Quinta-feira",
            DiaSemana.Sexta => "Sexta-feira",
            DiaSemana.Sabado => "Sábado",
            DiaSemana.Domingo => "Domingo",
            _ => diaSemana.ToString()
        };
    }

    private static string FormatarDuracao(int minutos)
    {
        return minutos >= 60 
            ? $"{minutos / 60}h {minutos % 60}min" 
            : $"{minutos} min";
    }

    public async Task<IEnumerable<HorarioDisponivelDto>> ObterPorPrestadorEDiaAsync(Guid prestadorId, DiaSemana diaSemana, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Obtendo horários disponíveis para prestador {PrestadorId} no {DiaSemana}", prestadorId, diaSemana);

            var horarios = await _horarioRepository.ObterPorPrestadorEDiaAsync(prestadorId, diaSemana, cancellationToken);
            
            return horarios.Select(ConverterParaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter horários disponíveis para prestador {PrestadorId} no {DiaSemana}", prestadorId, diaSemana);
            throw;
        }
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesCriacao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode criar horários para prestadores da sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesEdicao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode editar horários de prestadores da sua loja.");
            }
        }

        return (true, null);
    }

    public (bool IsValid, string? ErrorMessage) ValidarPermissoesExclusao(Guid? userLojaId, Guid prestadorLojaId, bool ehUsuarioLoja)
    {
        if (ehUsuarioLoja)
        {
            if (!userLojaId.HasValue || userLojaId.Value != prestadorLojaId)
            {
                return (false, "Você só pode excluir horários de prestadores da sua loja.");
            }
        }

        return (true, null);
    }
} 