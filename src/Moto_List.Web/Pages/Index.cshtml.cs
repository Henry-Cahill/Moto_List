using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Moto_List.Web.Pages;

public class IndexModel : PageModel
{
    public bool IsLoggedIn => HttpContext.Session.GetString("JwtToken") != null;
}
