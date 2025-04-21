using AuthenticationAPI.Interfaces;
using AuthenticationAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController(IJwtTokenProvider tokenProvider) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Login(LoginDetail loginDetail)
        {
            string token = await tokenProvider.GenerateJwtTokenAsync(loginDetail);

            return Ok(new
            {
                token
            });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "Hello, this is a protected resource!"
            });
        }
    }
}
