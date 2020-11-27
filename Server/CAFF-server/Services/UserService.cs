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
        Task<IdentityResult> RegisterUser(UserDTO userDTO);
        Task<IdentityResult> CreateUser(UserDTO userDTO);
        Task<IdentityResult> UpdateUser(UserDTO user);
        Task<IdentityResult> DeleteUser(string userEmail);
        Task<UserDTO> GetUserByEmail(string userEmail);
        Task<List<UserDTO>> GetUsers();
        Task<string> Login(string userName, string password);
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

        public async Task<IdentityResult> CreateUser(UserDTO userDTO)
        {
            var userWithSameEmail = await _userManager.FindByEmailAsync(userDTO.Email);
            if (userWithSameEmail != null)
            {
                throw new Exception("The provided email is already occupied");
            }

            var userWithSameName = await _userManager.FindByNameAsync(userDTO.UserName);
            if (userWithSameName != null) throw new Exception("The provided name is already occupied");

            User newUser = _mapper.Map<User>(new UserDTO { Email = userDTO.Email, UserName = userDTO.UserName, Password = userDTO.Password, Role = userDTO.Role});
            IdentityResult result = await _userManager.CreateAsync(newUser, userDTO.Password);

            return result;
        }

        public async Task<IdentityResult> RegisterUser(UserDTO userDTO)
        {
            userDTO.Role = Role.API_USER + "," + Role.SELF_MODIFICATION;
            return await CreateUser(userDTO);
        }

        public async Task<IdentityResult> UpdateUser(UserDTO userDTO)
        {
            var updatedUser = _mapper.Map<User>(new UserDTO { Email = userDTO.Email, UserName = userDTO.UserName, Password = userDTO.Password, Role = userDTO.Role } );
            updatedUser.ConcurrencyStamp = DateTime.UtcNow.ToString();
            var result = await _userManager.UpdateAsync(updatedUser);
            return result;
        }

        public async Task<IdentityResult> DeleteUser(string userEmail)
        {
            var toDelete = await _userManager.FindByEmailAsync(userEmail);
            if(toDelete == null) throw new Exception("User not found by email");
            var result = await _userManager.DeleteAsync(toDelete);
            return result;
        }

        public async Task<UserDTO> GetUserByEmail(string userEmail)
        {
            var user = await _userManager.Users.FirstAsync(it => it.Email == userEmail);
            if (user == null) throw new Exception("User not found by email");
            var userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersDTO = _mapper.Map<List<UserDTO>>(users);
            return usersDTO;
        }

        public async Task<string> Login(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);
            var tokenHandler = new JwtSecurityTokenHandler();

            if (result.Succeeded)
            {
                var user = await _userManager.Users.FirstAsync(x => x.UserName == userName);
                var roles = await _userManager.GetRolesAsync(user);

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
                return tokenHandler.WriteToken(token);
            }

            throw new Exception("Invalid credentials");
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            return;
        }
    }
}
