using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QRprolink_API.Entity;
using QRprolink_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace QRprolink_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),

        };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("signin")]
        public async Task<IActionResult> signin(SignInModel signInModel)
        {
            var user = await _userManager.FindByNameAsync(signInModel.UserName);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı");
            }
            var result = await _signInManager.PasswordSignInAsync(signInModel.UserName, signInModel.Password, false, false);
            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(token);
            }
            else
            {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }

        }

        [HttpPost("signup")]
        public async Task<IActionResult> signup(SignUpModel signUpModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = signUpModel.Username,
                    Email = signUpModel.Email

                };
                var result = await _userManager.CreateAsync(user, signUpModel.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
    }
}
