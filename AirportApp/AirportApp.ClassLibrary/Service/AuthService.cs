using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace AirportApp.ClassLibrary.Service;

public class AuthService(ICustomerRepository userRepository) : IAuthService
{
    private const int MinimumUsernameLength = 3;
    private const int MinimumPasswordLength = 6;
    private readonly PasswordHasher<Customer> passwordHasher = new();

    public async Task<Customer> GetByIdAsync(int id)
    {
        Customer? customer = await userRepository.GetByIdAsync(id);
        return customer ?? throw new KeyNotFoundException($"Customer with id {id} was not found.");
    }

    public async Task<Customer> LoginAsync(string email, string password, int? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        Customer? existingUser = await userRepository.GetByEmailAsync(email.Trim());
        if (existingUser == null)
            throw new InvalidOperationException("No account found with this email.");
        if (currentUserId.HasValue && existingUser.Id != currentUserId.Value)
            throw new InvalidOperationException("This account does not belong to the current user.");

        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Invalid email or password.");

        return existingUser;
    }

    public async Task RegisterAsync(string email, string phone, string username, string password)
    {
        string? normalizedEmail = email?.Trim();
        string? normalizedUsername = username?.Trim();
        string? normalizedPhone = phone?.Trim();

        ValidateRegistrationData(normalizedEmail, normalizedPhone, normalizedUsername, password);

        Customer? existingUser = await userRepository.GetByEmailAsync(normalizedEmail!);
        if (existingUser != null)
            throw new InvalidOperationException("An account with this email already exists.");

        Customer newUser = new Customer
        {
            Email = normalizedEmail!,
            Phone = normalizedPhone,
            Username = normalizedUsername!,
            Membership = null
        };

        string hashedPassword = passwordHasher.HashPassword(newUser, password);
        newUser.PasswordHash = hashedPassword;

        await userRepository.AddAsync(newUser);
    }

    public void Logout()
    {
        UserSession.CurrentUser = null;
        UserSession.PendingBookingParameters = null;
    }

    private void ValidateRegistrationData(string? email, string? phone, string? username, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (!ValidationHelper.IsValidEmail(email))
            throw new ArgumentException("Email format is invalid.");
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.");
        if (username.Length < MinimumUsernameLength)
            throw new ArgumentException("Username must have at least 3 characters.");
        if (!username.All(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == ' '))
            throw new ArgumentException("Username contains invalid characters.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.");
        if (!ValidationHelper.IsValidPhone(phone))
            throw new ArgumentException("Phone number must contain only digits and have 10 to 15 digits.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");
        if (password.Length < MinimumPasswordLength)
            throw new ArgumentException("Password must be at least 6 characters long.");
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await userRepository.GetByEmailAsync(email);
    }

    public async Task AddUserAsync(Customer customer)
    {
        await userRepository.AddAsync(customer);
    }
}
