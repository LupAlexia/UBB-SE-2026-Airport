using System.Text.Json.Serialization;

namespace AirportApp.ClassLibrary.Entity.Dto
{
    public record AdministratorDTO(
        int id,
        [property: JsonPropertyName("fullName")] string name,
        [property: JsonPropertyName("emailAddress")] string email);
}
