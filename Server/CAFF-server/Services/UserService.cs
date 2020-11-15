using AutoMapper;
using concertticket_webapp_appserver.DTOs;
using concertticket_webapp_appserver.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace concertticket_webapp_appserver.Services
{
    public interface IUserService
    {
        Task<IdentityResult> Create(User user, string password);
        Task<UserDTO> SignIn(string userName, string password);
        Task SignOut();
    }
    public class UserService: IUserService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IConfiguration _configuration;
        private IMapper _mapper;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IdentityResult> Create(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<UserDTO> SignIn(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);

            if (result.Succeeded)
            {
                var user = _userManager.Users.SingleOrDefault(x => x.UserName == userName);
                var roles = _userManager.GetRolesAsync(user).Result;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Key"]);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
                foreach (string role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Token = tokenHandler.WriteToken(token);
                userDTO.Role = roles[0];
                return userDTO;
            }

            return null;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
            return;
        }
    }
}
