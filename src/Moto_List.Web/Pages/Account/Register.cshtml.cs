using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moto_List.Shared.DTOs;
using Moto_List.Web.Services;

namespace Moto_List.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly ApiService _api;

    public RegisterModel(ApiService api)
    {
        _api = api;
    }

    [BindProperty]
    public RegisterRequest Input { get; set; } = new() { Username = "", Email = "", Password = "" };

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("JwtToken") != null)
            return RedirectToPage("/Checklist/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var result = await _api.RegisterAsync(Input);
        if (result is null)
        {
            ErrorMessage = "Username or email already exists.";
            return Page();
        }

        HttpContext.Session.SetString("JwtToken", result.Token);
        HttpContext.Session.SetString("Username", result.Username);

        return RedirectToPage("/Checklist/Index");
    }
}
