using AutoMapper;
using AgendaApp.Aplicacao.DTOs;
using AgendaApp.Dominio.Entities;
using AgendaApp.Dominio.Enums;
using AgendaApp.Web.ViewModels;

namespace AgendaApp.Web.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreatePrestadorMappings();
        CreateServicoMappings();
        CreatePrestadorServicoMappings();
        CreateHorarioDisponivelMappings();
        CreateLojaMappings();
        CreateAgendamentoMappings();
    }

    private void CreatePrestadorMappings()
    {
        CreateMap<Prestador, PrestadorViewModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Endereco))
            .ForMember(dest => dest.CNPJ, opt => opt.MapFrom(src => src.CNPJ != null ? src.CNPJ.Formatado : null));

        CreateMap<PrestadorViewModel, Prestador>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.CNPJ, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
            .ForMember(dest => dest.Servicos, opt => opt.Ignore())
            .ForMember(dest => dest.HorariosDisponiveis, opt => opt.Ignore());

        CreateMap<Prestador, PrestadorListViewModel>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Endereco))
            .ForMember(dest => dest.CNPJ, opt => opt.MapFrom(src => src.CNPJ != null ? src.CNPJ.Formatado : null))
            .ForMember(dest => dest.TotalServicos, opt => opt.MapFrom(src => src.Servicos.Count))
            .ForMember(dest => dest.TotalHorarios, opt => opt.MapFrom(src => src.HorariosDisponiveis.Count));
    }

    private void CreateServicoMappings()
    {
        CreateMap<Servico, ServicoViewModel>()
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => 
                src.PrestadorServicos != null && src.PrestadorServicos.Any() 
                    ? src.PrestadorServicos.First().Prestador.Nome 
                    : string.Empty));

        CreateMap<ServicoViewModel, Servico>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore());

        CreateMap<Servico, ServicoListViewModel>()
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => 
                src.PrestadorServicos != null && src.PrestadorServicos.Any() 
                    ? src.PrestadorServicos.First().Prestador.Nome 
                    : string.Empty))
            .ForMember(dest => dest.DuracaoFormatada, opt => opt.MapFrom(src => 
                src.DuracaoEmMinutos >= 60 
                    ? $"{src.DuracaoEmMinutos / 60}h {src.DuracaoEmMinutos % 60:00}min"
                    : $"{src.DuracaoEmMinutos} min"));

        CreateMap<ServicoResumoDto, ServicoListViewModel>();

        CreateMap<LojaDto, LojaViewModel>();
        CreateMap<CriarLojaViewModel, CriarLojaDto>();
        CreateMap<EditarLojaViewModel, AtualizarLojaDto>();
        CreateMap<LojaDto, EditarLojaViewModel>();
    }

    private void CreateHorarioDisponivelMappings()
    {
        CreateMap<HorarioDisponivelDto, HorarioDisponivelViewModel>()
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => src.PrestadorNome));

        CreateMap<HorarioDisponivelResumoDto, HorarioDisponivelListViewModel>();

        CreateMap<HorarioDisponivelViewModel, CriarHorarioDisponivelDto>();

        CreateMap<HorarioDisponivelViewModel, AtualizarHorarioDisponivelDto>();

        CreateMap<HorarioMultiploViewModel, CriarHorarioMultiploDto>();

        CreateMap<HorarioDisponivelFiltroViewModel, HorarioDisponivelFiltroDto>();

        CreateMap<HorarioDisponivel, HorarioDisponivelViewModel>()
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => src.Prestador != null ? src.Prestador.Nome : string.Empty));

        CreateMap<HorarioDisponivel, HorarioDisponivelListViewModel>()
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => src.Prestador != null ? src.Prestador.Nome : string.Empty))
            .ForMember(dest => dest.DiaSemanaTexto, opt => opt.MapFrom(src => GetDiaSemanaTexto(src.DiaSemana)))
            .ForMember(dest => dest.HorarioFormatado, opt => opt.MapFrom(src => $"{src.HoraInicio:HH:mm} às {src.HoraFim:HH:mm}"))
            .ForMember(dest => dest.DuracaoFormatada, opt => opt.MapFrom(src => GetDuracaoFormatada(src.HoraInicio, src.HoraFim)));
    }

    private void CreateLojaMappings()
    {
        CreateMap<LojaDto, LojaViewModel>();

        CreateMap<CriarLojaViewModel, CriarLojaDto>();

        CreateMap<EditarLojaViewModel, AtualizarLojaDto>();

        CreateMap<LojaDto, EditarLojaViewModel>();
    }

    private static string GetDiaSemanaTexto(DiaSemana diaSemana)
    {
        return diaSemana switch
        {
            DiaSemana.Domingo => "Domingo",
            DiaSemana.Segunda => "Segunda-feira",
            DiaSemana.Terca => "Terça-feira",
            DiaSemana.Quarta => "Quarta-feira",
            DiaSemana.Quinta => "Quinta-feira",
            DiaSemana.Sexta => "Sexta-feira",
            DiaSemana.Sabado => "Sábado",
            _ => "Indefinido"
        };
    }

    private void CreatePrestadorServicoMappings()
    {
        CreateMap<ServicoGlobalDisponivelDto, ServicoGlobalDisponivelViewModel>();
        
        CreateMap<ServicoVinculadoDto, ServicoVinculadoViewModel>();
        
        CreateMap<VincularServicoViewModel, VincularServicoDto>();
        
        CreateMap<EditarServicoVinculadoViewModel, AtualizarServicoVinculadoDto>();
    }

    private static string GetDuracaoFormatada(TimeOnly horaInicio, TimeOnly horaFim)
    {
        var duracao = horaFim - horaInicio;
        var totalMinutos = (int)duracao.TotalMinutes;
        return totalMinutos >= 60 
            ? $"{totalMinutos / 60}h {totalMinutos % 60:00}min"
            : $"{totalMinutos} min";
    }

    private void CreateAgendamentoMappings()
    {
        CreateMap<Agendamento, AgendamentoViewModel>()
            .ForMember(dest => dest.ClienteEmail, opt => opt.MapFrom(src => src.ClienteEmail != null ? src.ClienteEmail.Endereco : null))
            .ForMember(dest => dest.LojaNome, opt => opt.MapFrom(src => src.Loja != null ? src.Loja.Nome : string.Empty))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Nome : string.Empty))
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => src.Prestador != null ? src.Prestador.Nome : string.Empty))
            .ForMember(dest => dest.ServicoValor, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Valor : 0))
            .ForMember(dest => dest.ServicoDuracaoEmMinutos, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.DuracaoEmMinutos : 0));

        CreateMap<Agendamento, AgendamentoListViewModel>()
            .ForMember(dest => dest.ClienteEmail, opt => opt.MapFrom(src => src.ClienteEmail != null ? src.ClienteEmail.Endereco : null))
            .ForMember(dest => dest.LojaNome, opt => opt.MapFrom(src => src.Loja != null ? src.Loja.Nome : string.Empty))
            .ForMember(dest => dest.ServicoNome, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Nome : string.Empty))
            .ForMember(dest => dest.PrestadorNome, opt => opt.MapFrom(src => src.Prestador != null ? src.Prestador.Nome : string.Empty))
            .ForMember(dest => dest.ServicoValor, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.Valor : 0))
            .ForMember(dest => dest.ServicoDuracaoEmMinutos, opt => opt.MapFrom(src => src.Servico != null ? src.Servico.DuracaoEmMinutos : 0));

        CreateMap<AgendamentoViewModel, CriarAgendamentoDto>();
        CreateMap<AgendamentoViewModel, AtualizarAgendamentoDto>();
        CreateMap<AgendamentoFiltroViewModel, AgendamentoFiltroDto>();
        CreateMap<AgendamentoEstatisticasDto, AgendamentoEstatisticasViewModel>();
    }
} 