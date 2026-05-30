using System;
using System.Net.Http;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.ClassLibrary.Proxy;

public class AuthServiceProxy(HttpClient httpClient) : ServiceProxyBase(httpClient), IAuthService
{
    private const string BaseUrl = "api/customer";

    public async Task<Customer> LoginAsync(string email, string password, int? currentUserId = null)
    {
        var request = new LoginRequestDTO
        {
            Email = email?.Trim() ?? string.Empty,
            Password = password ?? string.Empty,
            CurrentUserId = currentUserId
        };

        var customerDTO = await PostForResultAsync<LoginRequestDTO, CustomerDTO>($"{BaseUrl}/login", request);
        return MapCustomerFromTransferObject(customerDTO);
    }

    public async Task RegisterAsync(string email, string phone, string username, string password)
    {
        var request = new RegisterRequestDTO
        {
            Email = email?.Trim() ?? string.Empty,
            Phone = phone?.Trim() ?? string.Empty,
            Username = username?.Trim() ?? string.Empty,
            Password = password ?? string.Empty
        };

        await PostAsync($"{BaseUrl}/register", request);
    }

    public void Logout()
    {
        UserSession.CurrentUser = null;
        UserSession.PendingBookingParameters = null;
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        var customerDTO = await GetOptionalAsync<CustomerDTO>($"{BaseUrl}/{id}");
        return customerDTO is null ? null! : MapCustomerFromTransferObject(customerDTO);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        var customerDTO = await GetOptionalAsync<CustomerDTO>($"{BaseUrl}/by-email?email={Uri.EscapeDataString(email)}");
        return customerDTO is null ? null : MapCustomerFromTransferObject(customerDTO);
    }

    public async Task AddUserAsync(Customer customer)
    {
        var customerDTO = new CustomerDTO(
            customer.Id,
            customer.Email,
            customer.Phone,
            customer.Username,
            customer.PasswordHash,
            customer.Membership?.Id,
            null);

        var created = await PostForResultAsync<CustomerDTO, CustomerDTO>(BaseUrl, customerDTO);
        if (created is not null)
        {
            customer.Id = created.id;
        }
    }

    private static Customer MapCustomerFromTransferObject(CustomerDTO customerTransferObject)
    {
        return new Customer
        {
            Id = customerTransferObject.id,
            Email = customerTransferObject.email,
            Phone = customerTransferObject.phone,
            Username = customerTransferObject.username,
            PasswordHash = customerTransferObject.passwordHash,
            Membership = customerTransferObject.membership is not null
                ? new Membership
                {
                    Id = customerTransferObject.membership.id,
                    Name = customerTransferObject.membership.name,
                    FlightDiscountPercentage = customerTransferObject.membership.flightDiscountPercentage
                }
                : null
        };
    }
}
