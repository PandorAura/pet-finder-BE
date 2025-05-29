using AnimalAdoption.Models.DTOs;
using AnimalAdoption.Models;
using AutoMapper;

namespace AnimalAdoption.Utilities.MappingProfiles
{
    public class AnimalAdoptionProfile : Profile
    {
        public AnimalAdoptionProfile()
        {
            // Animal mappings
            CreateMap<Animal, AnimalDto>();
            CreateMap<Animal, AnimalSimpleDto>();
            CreateMap<AnimalCreateDto, Animal>();
            CreateMap<AnimalUpdateDto, Animal>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Adopter mappings
            CreateMap<User, UserDto>();
            CreateMap<User, UserSimpleDto>();
            CreateMap<UserCreateDto, User>();

            // Adoption mappings
            CreateMap<Adoption, AdoptionDto>();
            CreateMap<AdoptionCreateDto, Adoption>();

        }
    }
}
