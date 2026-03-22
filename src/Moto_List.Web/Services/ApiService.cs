using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Moto_List.Shared.DTOs;

namespace Moto_List.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AttachToken()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
    }

    public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        AttachToken();
        var response = await _http.GetAsync("api/categories");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<CategoryDto>>(JsonOptions) ?? [];
    }

    public async Task<List<MotoItemDto>> GetMotoItemsAsync()
    {
        AttachToken();
        var response = await _http.GetAsync("api/motoitems");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<MotoItemDto>>(JsonOptions) ?? [];
    }

    public async Task<MotoItemDto?> CreateMotoItemAsync(CreateMotoItemRequest request)
    {
        AttachToken();
        var response = await _http.PostAsJsonAsync("api/motoitems", request);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<MotoItemDto>(JsonOptions);
    }

    public async Task<bool> ToggleMotoItemAsync(int id)
    {
        AttachToken();
        var response = await _http.PatchAsync($"api/motoitems/{id}/toggle", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMotoItemAsync(int id)
    {
        AttachToken();
        var response = await _http.DeleteAsync($"api/motoitems/{id}");
        return response.IsSuccessStatusCode;
    }
}
