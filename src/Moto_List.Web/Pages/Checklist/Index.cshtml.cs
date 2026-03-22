using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moto_List.Shared.DTOs;
using Moto_List.Web.Services;

namespace Moto_List.Web.Pages.Checklist;

public class IndexModel : PageModel
{
    private readonly ApiService _api;

    public IndexModel(ApiService api)
    {
        _api = api;
    }

    public List<MotoItemDto> Items { get; set; } = [];
    public List<CategoryDto> Categories { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("JwtToken") is null)
            return RedirectToPage("/Account/Login");

        Items = await _api.GetMotoItemsAsync();
        Categories = await _api.GetCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostToggleAsync(int itemId)
    {
        if (HttpContext.Session.GetString("JwtToken") is null)
            return RedirectToPage("/Account/Login");

        await _api.ToggleMotoItemAsync(itemId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int itemId)
    {
        if (HttpContext.Session.GetString("JwtToken") is null)
            return RedirectToPage("/Account/Login");

        await _api.DeleteMotoItemAsync(itemId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddAsync(string name, string? description, int categoryId)
    {
        if (HttpContext.Session.GetString("JwtToken") is null)
            return RedirectToPage("/Account/Login");

        await _api.CreateMotoItemAsync(new CreateMotoItemRequest
        {
            Name = name,
            Description = description,
            CategoryId = categoryId
        });
        return RedirectToPage();
    }
}
