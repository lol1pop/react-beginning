using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DeskAlerts.Controllers
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class AuthResult
    {
        public bool success;
        public int? userId = null;
        public string error = null;
    }
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        // GET: api/Auth
        [HttpPost("login")]
        public AuthResult Login([FromBody]User user) {
            var userId = DBManager.login.GetUserId(user.Login, Security.ComputeHash(user.Login, user.Password));
            if (userId != 0)
            return new AuthResult{success = true, userId = userId};
            else {
                return new AuthResult { success = false, error = "no login"};
            }
        }

        public int getUserId(string user) {
            return 0;
        }
    }
}