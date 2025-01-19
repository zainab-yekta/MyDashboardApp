using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace MyDashboardApp.Controllers
{
    public class UtilityController : Controller
    {
        public IActionResult HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            string hashedPassword = passwordHasher.HashPassword(null, password);

            return Content($"Plain Password: {password}<br>Hashed Password: {hashedPassword}", "text/html");
        }
    }
}

//https:localhost:7035/Utility/HashPassword?password=Testuser@123 create for seeing hash code of password.
