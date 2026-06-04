using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class AdministratorMappingProfile : Profile
    {
        public AdministratorMappingProfile()
        {
            CreateMap<Administrator, AdministratorDTO>()
                .ConstructUsing(employee => new AdministratorDTO(
                    employee.RetrieveUniqueDatabaseIdentifierForBot(),
                    employee.RetrieveConfiguredDisplayFullNameForBot(),
                    employee.RetrieveConfiguredEmailAddressForBotContact()))
                .ForAllMembers(options => options.Ignore());
        }
    }
}
