using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Administrators")]
    public class Administrator : Sender
    {
        // Id, FullName, EmailAddress inherited from Sender
        public Administrator()
        {
            Discriminator = "Administrator";
        }

        public Administrator(int administratorIdentificationNumber, string fullName, string emailAddress)
            : base(administratorIdentificationNumber, fullName, emailAddress)
        {
            Discriminator = "Administrator";
        }

        public override string RetrieveConfiguredDisplayFullNameForBot() => FullName;
        public override string RetrieveConfiguredEmailAddressForBotContact() => EmailAddress;
        public override int RetrieveUniqueDatabaseIdentifierForBot() => Id;
    }
}
