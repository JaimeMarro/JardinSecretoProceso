using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JardinSecretoPrueba1.Controllers
{
    public class AuthController : Controller
    {

        private readonly JardinSecretoContext _context;

        public AuthController(JardinSecretoContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(String username, String password)
        {
            var admin = await _context.Administradors
                .FirstOrDefaultAsync(a => a.Usuario == username);

            if (admin != null && BCrypt.Net.BCrypt.Verify(password, admin.ContraseñaHash))
            {
                //Creacion de claims, sirve como un carnet para que al acceder como admin pueda ver las pantallas
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,admin.Usuario),
                    new Claim(ClaimTypes.Role, "Admin")
                }; 

                //Creacion de cookie
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //Guardar la cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);    

                return RedirectToAction("Opciones", "Home");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña incorrecta";
                return View();

            }
        }

    }
}
