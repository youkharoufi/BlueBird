using AutoMapper;
using BlueBirds.Models;
using BlueBirds.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlueBirds.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AccountController(IMapper mapper, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
             ITokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromForm]RegisterUser registerUser)
        {
            var user = _mapper.Map<AppUser>(registerUser);

            user.Email = registerUser.Email;
            user.UserName = registerUser.UserName;

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if(!result.Succeeded)
            {
                return BadRequest("Error : Cannot create registered user");
            }

            bool roleExists = await _roleManager.RoleExistsAsync(registerUser.Role);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(registerUser.Role));
            }

            try
            {
                var test = await _userManager.AddToRoleAsync(user, registerUser.Role);
                user.Role = registerUser.Role;

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login([FromForm]LoginUser loginUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(r => r.Email == loginUser.Email);

            if (user==null) return NotFound("No such user has been registered");

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);

            if (!result) return BadRequest("Incorrect password");

            user.Token = await _tokenService.GenerateToken(user);

            return Ok(user);


        }

    }
}
