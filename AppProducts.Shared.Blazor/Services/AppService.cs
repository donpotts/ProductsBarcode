using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Web;
using AppProducts.Shared.Blazor.Authorization;
using AppProducts.Shared.Blazor.Models;
using AppProducts.Shared.Models;

namespace AppProducts.Shared.Blazor.Services;

public class AppService(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider)
{
    private readonly IdentityAuthenticationStateProvider authenticationStateProvider
            = authenticationStateProvider as IdentityAuthenticationStateProvider
                ?? throw new InvalidOperationException();

    private static async Task HandleResponseErrorsAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode
            && response.StatusCode != HttpStatusCode.Unauthorized
            && response.StatusCode != HttpStatusCode.NotFound)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new Exception(message);
        }

        response.EnsureSuccessStatusCode();
    }

    public class ODataResult<T>
    {
        [JsonPropertyName("@odata.count")]
        public int? Count { get; set; }

        public IEnumerable<T>? Value { get; set; }
    }

    public async Task<ODataResult<T>?> GetODataAsync<T>(
            string entity,
            int? top = null,
            int? skip = null,
            string? orderby = null,
            string? filter = null,
            bool count = false,
            string? expand = null)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        var queryString = HttpUtility.ParseQueryString(string.Empty);

        if (top.HasValue)
        {
            queryString.Add("$top", top.ToString());
        }

        if (skip.HasValue)
        {
            queryString.Add("$skip", skip.ToString());
        }

        if (!string.IsNullOrEmpty(orderby))
        {
            queryString.Add("$orderby", orderby);
        }

        if (!string.IsNullOrEmpty(filter))
        {
            queryString.Add("$filter", filter);
        }

        if (count)
        {
            queryString.Add("$count", "true");
        }

        if (!string.IsNullOrEmpty(expand))
        {
            queryString.Add("$expand", expand);
        }

        var uri = $"/odata/{entity}?{queryString}";

        HttpRequestMessage request = new(HttpMethod.Get, uri);
        request.Headers.Authorization = new("Bearer", token);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<ODataResult<T>>();
    }


    public async Task<Dictionary<string, List<string>>> RegisterUserAsync(RegisterModel registerModel)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/identity/register",
            new { registerModel.Email, registerModel.Password });

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var json = await response.Content.ReadAsStringAsync();

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            return problemDetails?.Errors != null
                ? problemDetails.Errors
                : throw new Exception("Bad Request");
        }

        response.EnsureSuccessStatusCode();

        response = await httpClient.PostAsJsonAsync(
            "/identity/login",
            new { registerModel.Email, registerModel.Password });

        response.EnsureSuccessStatusCode();

        var accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>()
            ?? throw new Exception("Failed to authenticate");

        HttpRequestMessage request = new(HttpMethod.Put, "/api/user/@me");
        request.Headers.Authorization = new("Bearer", accessTokenResponse.AccessToken);
        request.Content = JsonContent.Create(new UpdateApplicationUserDto
        {
            FirstName = registerModel.FirstName,
            LastName = registerModel.LastName,
            Title = registerModel.Title,
            CompanyName = registerModel.CompanyName,
            Photo = registerModel.Photo,
        });

        response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return [];
    }

    public async Task<ApplicationUserDto[]?> ListUserAsync()
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Get, "/api/user");
        request.Headers.Authorization = new("Bearer", token);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<ApplicationUserDto[]>();
    }

    public Task<ODataResult<ApplicationUserDto>?> ListUserODataAsync(
        int? top = null,
        int? skip = null,
        string? orderby = null,
        string? filter = null,
        bool count = false,
        string? expand = null)
    {
        return GetODataAsync<ApplicationUserDto>("User", top, skip, orderby, filter, count, expand);
    }

    public async Task<ApplicationUserWithRolesDto?> GetUserByIdAsync(string id)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Get, $"/api/user/{id}");
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<ApplicationUserWithRolesDto>();
    }

    public async Task UpdateUserAsync(string id, UpdateApplicationUserDto data)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Put, $"/api/user/{id}");
        request.Headers.Add("Authorization", $"Bearer {token}");
        request.Content = JsonContent.Create(data);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }

    public async Task DeleteUserAsync(string id)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Delete, $"/api/user/{id}");
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }

    public async Task<Products[]?> ListProductsAsync()
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Get, "/api/products");
        request.Headers.Authorization = new("Bearer", token);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<Products[]>();
    }

    public Task<ODataResult<Products>?> ListProductsODataAsync(
        int? top = null,
        int? skip = null,
        string? orderby = null,
        string? filter = null,
        bool count = false,
        string? expand = null)
    {
        return GetODataAsync<Products>("Products", top, skip, orderby, filter, count, expand);
    }

    public async Task<Products?> GetProductsByIdAsync(long key)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Get, $"/api/products/{key}");
        request.Headers.Authorization = new("Bearer", token);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<Products>();
    }

    public async Task UpdateProductsAsync(long key, Products data)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Put, $"/api/products/{key}");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(data);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }

    public async Task<Products?> InsertProductsAsync(Products data)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Post, "/api/products");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(data);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<Products>();
    }

    public async Task DeleteProductsAsync(long key)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Delete, $"/api/products/{key}");
        request.Headers.Authorization = new("Bearer", token);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }

    public async Task<string?> UploadImageAsync(Stream stream, int bufferSize, string contentType)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        MultipartFormDataContent content = [];
        StreamContent fileContent = new(stream, bufferSize);
        fileContent.Headers.ContentType = new(contentType);
        content.Add(fileContent, "image", "image");

        HttpRequestMessage request = new(HttpMethod.Post, $"/api/image");
        request.Headers.Add("Authorization", $"Bearer {token}");
        request.Content = content;

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);

        return await response.Content.ReadFromJsonAsync<string>();
    }

    public async Task<string?> UploadImageAsync(IBrowserFile image)
    {
        using var stream = image.OpenReadStream(image.Size);

        return await UploadImageAsync(stream, Convert.ToInt32(image.Size), image.ContentType);
    }

    public async Task<string?> SkiaUploadImageAsync(byte[] imageBytes, string contentType)
    {
        using var stream = new MemoryStream(imageBytes);

        return await UploadImageAsync(stream, imageBytes.Length, contentType);
    }

    public async Task ChangePasswordAsync(string oldPassword, string newPassword)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Post, $"/identity/manage/info");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(new { oldPassword, newPassword });

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }

    public async Task ModifyRolesAsync(string key, IEnumerable<string> roles)
    {
        var token = await authenticationStateProvider.GetBearerTokenAsync()
            ?? throw new Exception("Not authorized");

        HttpRequestMessage request = new(HttpMethod.Put, $"/api/user/{key}/roles");
        request.Headers.Authorization = new("Bearer", token);
        request.Content = JsonContent.Create(roles);

        var response = await httpClient.SendAsync(request);

        await HandleResponseErrorsAsync(response);
    }
}
