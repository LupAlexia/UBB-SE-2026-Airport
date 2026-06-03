using AirportApp.ClassLibrary.Entity.Domain;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class AirportDTO
    {
        public int Id { get; init; }
        public string AirportCode { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;


        public AirportDTO(
            int id,
            string airportCode,
            string city,
            string name)
        {
            Id = id;
            AirportCode = airportCode;
            City = city;
            Name = name;
        }
            
    }

}
