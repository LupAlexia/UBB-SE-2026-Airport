using System.Net.Mail;

namespace AirportApp.ClassLibrary.Utility;

public static class ValidationHelper
{
    private const int MinimumPhoneLength = 10;
    private const int MaximumPhoneLength = 15;

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        bool hasOnlyDigits = phone.All(char.IsDigit);
        bool hasValidLength = phone.Length >= MinimumPhoneLength && phone.Length <= MaximumPhoneLength;

        return hasOnlyDigits && hasValidLength;
    }
}