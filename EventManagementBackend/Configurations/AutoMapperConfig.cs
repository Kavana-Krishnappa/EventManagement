using AutoMapper;
using EventManagement.Models;
using EventManagement.DTOs;

namespace EventManagement.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<EventDTO, Event>().ReverseMap();
            CreateMap<ParticipantDTO, Participant>().ReverseMap();
            CreateMap<AdminDTO, Admin>().ReverseMap();
            CreateMap<RegistrationDTO, Registration>().ReverseMap();
            CreateMap<RegistrationCreateDTO,Registration>().ReverseMap();



        }
    }
}
