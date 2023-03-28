using AutoMapper;
using BlueBirds.Claims;
using BlueBirds.EmailService;
using BlueBirds.Models;
using BlueBirds.TokenService;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmtpServer.Text;

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
        private readonly IEmailSender _emailSender;

        public AccountController(IMapper mapper, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
             ITokenService tokenService, IEmailSender emailService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailSender = emailService;
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

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var message = new Message(new string[] { user.Email }, "Confirmation email link", registerUser.Link+emailConfirmationToken);

            await _emailSender.SendEmailAsync(message);

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login([FromForm]LoginUser loginUser)
        {
            var user = await _userManager.FindByEmailAsync(loginUser.Email);

            if (user==null) return NotFound("No such user has been registered");

            if (!user.EmailConfirmed) return BadRequest("You need to confirm your email first");

            var result = await _userManager.CheckPasswordAsync(user, loginUser.Password);

            if (!result) return BadRequest("Incorrect password");

            user.Token = await _tokenService.GenerateToken(user);

            return Ok(user);


        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<AppUser>> ForgotPassword([FromForm]Models.ForgotPasswordModel fpmodel)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(o=>o.Email == fpmodel.Email);

            if (user == null) return NotFound("No User with such email registered :/");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            user.PasswordToken = token;

            await _userManager.UpdateAsync(user);
          

            var message = new Message(new string[] { user.Email }, "Test email", fpmodel.Link+token);
            await _emailSender.SendEmailAsync(message);


            return Ok(user);

        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<AppUser>> ResetPassword([FromForm] ResetPassword resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);

            if (user == null) return NotFound("No such user email registered in the application");

            var code = resetPasswordModel.Token.Replace(" ", "+");

            var resetPassResult = await _userManager.ResetPasswordAsync(user, code, resetPasswordModel.Password);

            if (resetPassResult.Succeeded) return user;

            return BadRequest("Cannot reset password");



        }

        [HttpGet]
        public async Task<ActionResult<AppUser>> GetUser()
        {
            var username = User.GetUsername();

            var user = _userManager.FindByNameAsync(username);

            return Ok(user);
        }


        [HttpPost("email-confirmation")]
        public async Task<ActionResult<AppUser>> EmailConfirmation([FromForm]ConfirmationEmailModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound("No such user registered in the app");

            var code = model.Token.Replace(" ", "+");

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if(result.Succeeded) return Ok(user);

            return BadRequest("Cannot confirm email");
        }

    }
}
