using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server.Services
{
    public interface IAdminService
    {
        Task<IdentityResult> DeleteUser(string id);
        Task<UserDTO> GetUser(string id);
        IEnumerable<UserDTO> GetUsers();
        IEnumerable<UserDTO> GetUsersSearch(string userName);
        Task<User> EditPermission(string id, bool admin);
    }

    public class AdminService : IAdminService
    {
        private UserManager<User> _userManager;
        private IMapper _mapper;

        public AdminService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user != null) user.Banned = true;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<User> EditPermission(string id, bool admin)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            if(admin)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "User");
                if (result.Succeeded) result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded) return user;
                else return null;
            }
            else
            {
                var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (result.Succeeded) result = await _userManager.AddToRoleAsync(user, "User");
                if (result.Succeeded) return user;
                else return null;
            }
        }

        public async Task<UserDTO> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = _userManager.GetRolesAsync(user).Result;
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = roles[0];
            return userDTO;
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            var users = _userManager.Users.Where(x => !x.Banned).OrderBy(x => x.Name).ToList();
            List<UserDTO> userDTOs = new List<UserDTO>();
            foreach(User user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = roles[0];
                userDTOs.Add(userDTO);
            }
            return userDTOs;
        }

        public IEnumerable<UserDTO> GetUsersSearch(string userName)
        {
            var users = _userManager.Users.Where(x => x.UserName.Contains(userName) && !x.Banned).OrderBy(x => x.Name).ToList();
            List<UserDTO> userDTOs = new List<UserDTO>();
            foreach (User user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Role = roles[0];
                userDTOs.Add(userDTO);
            }
            return userDTOs;
        }
    }
}
