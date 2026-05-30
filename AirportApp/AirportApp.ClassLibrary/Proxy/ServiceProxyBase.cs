using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Proxy;

public abstract class ServiceProxyBase(HttpClient httpClient)
{
    protected HttpClient HttpClient { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected async Task<List<T>> GetListAsync<T>(string requestUri)
    {
        return await GetRequiredAsync<List<T>>(requestUri);
    }

    protected async Task<T?> GetOptionalAsync<T>(string requestUri) where T : class
    {
        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(requestUri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            await EnsureSuccessStatusCodeAsync(response, requestUri);
            return await ReadContentAsync<T>(response.Content);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    protected async Task<T> GetRequiredAsync<T>(string requestUri)
    {
        using HttpResponseMessage response = await HttpClient.GetAsync(requestUri);
        await EnsureSuccessStatusCodeAsync(response, requestUri);
        return await ReadContentAsync<T>(response.Content)
            ?? throw new InvalidOperationException($"Empty response from '{requestUri}'.");
    }

    protected async Task<TResult> PostForResultAsync<TValue, TResult>(string requestUri, TValue value)
    {
        using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(requestUri, value, JsonOptions);
        await EnsureSuccessStatusCodeAsync(response, requestUri);
        return await ReadContentAsync<TResult>(response.Content)
            ?? throw new InvalidOperationException($"Empty response from '{requestUri}'.");
    }

    protected async Task PostAsync<TValue>(string requestUri, TValue value)
    {
        using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(requestUri, value, JsonOptions);
        await EnsureSuccessStatusCodeAsync(response, requestUri);
    }

    protected async Task PutAsync<TValue>(string requestUri, TValue value)
    {
        using HttpResponseMessage response = await HttpClient.PutAsJsonAsync(requestUri, value, JsonOptions);
        await EnsureSuccessStatusCodeAsync(response, requestUri);
    }

    protected async Task DeleteAsync(string requestUri)
    {
        using HttpResponseMessage response = await HttpClient.DeleteAsync(requestUri);
        await EnsureSuccessStatusCodeAsync(response, requestUri);
    }

    protected async Task<T?> DeleteForResultAsync<T>(string requestUri) where T : class
    {
        using HttpResponseMessage response = await HttpClient.DeleteAsync(requestUri);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessStatusCodeAsync(response, requestUri);
        return await ReadContentAsync<T>(response.Content);
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, string requestUri)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string errorBody = (await response.Content.ReadAsStringAsync()).Trim();
        string message = string.IsNullOrWhiteSpace(errorBody)
            ? $"Request to '{requestUri}' failed with status code {(int)response.StatusCode} ({response.ReasonPhrase})."
            : errorBody;

        throw new InvalidOperationException(message);
    }

    private static async Task<T?> ReadContentAsync<T>(HttpContent content)
    {
        if (typeof(T) == typeof(string))
        {
            string raw = await content.ReadAsStringAsync();
            if (raw.Length > 0 && raw.TrimStart().StartsWith('"'))
            {
                return JsonSerializer.Deserialize<T>(raw, JsonOptions);
            }

            return (T?)(object?)raw;
        }

        return await content.ReadFromJsonAsync<T>(JsonOptions);
    }
}
