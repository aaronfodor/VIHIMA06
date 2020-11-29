using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CAFF_server.Services
{

    public interface IUserService
    {
        Task<IdentityResult> CreateUser(User user, string password);
        Task<UserDTO> GetUser(string userid);
        Task<IdentityResult> UpdateUser(User user);
        Task<IdentityResult> UpdateUserPassword(string userid, string oldpassword, string newpassword);
        Task<UserDTO> Login(string userName, string password);
        Task Logout();
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

        public async Task<IdentityResult> CreateUser(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, "User");
            return result;
        }

        public async Task<IdentityResult> UpdateUser(User user)
        {
            var userindb = await _userManager.FindByIdAsync(user.Id);
            if (user.Email != null) userindb.Email = user.Email;
            if (user.Name != null) userindb.Name = user.Name;
            if (user.UserName != null) userindb.UserName = user.UserName;
            var result = await _userManager.UpdateAsync(userindb);
            return result;
        }

        public async Task<IdentityResult> UpdateUserPassword(string userid, string oldpassword, string newpassword)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
            return result;
        }

        public async Task<UserDTO> Login(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);
            var tokenHandler = new JwtSecurityTokenHandler();

            if (result.Succeeded)
            {
                var user = await _userManager.Users.FirstAsync(x => x.UserName == userName);
                var roles = await _userManager.GetRolesAsync(user);

                var key = Encoding.UTF8.GetBytes(_configuration["Key"]);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

                foreach (string role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Token = tokenHandler.WriteToken(token);
                userDTO.Role = roles[0];
                return userDTO;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            return;
        }

        public async Task<UserDTO> GetUser(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var roles = _userManager.GetRolesAsync(user).Result; 
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = roles[0];
            return userDTO;
        }
    }
}
