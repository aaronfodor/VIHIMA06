using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAFF_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private IAdminService _adminService;
        private IMapper _mapper;
        private ILoggerService _loggerService;
        public AdminController(IAdminService adminService, IMapper mapper, ILoggerService loggerService)
        {
            _adminService = adminService;
            _mapper = mapper;
            _loggerService = loggerService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var result = await _adminService.DeleteUser(id);
                if (!result.Succeeded)
                {
                    _loggerService.Error(result.Errors.ToString(), adminid);
                    return BadRequest(result);
                }
                else
                {
                    _loggerService.Info("User deleted successfully", adminid);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var user = await _adminService.GetUser(id);

                if (user == null)
                {
                    _loggerService.Error("User not found", adminid);
                    return NotFound();
                }
                else
                {
                    _loggerService.Info("User retrieval successful", adminid);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var users = _adminService.GetUsers();
                if (users == null)
                {
                    _loggerService.Error("No users could be retrieved", adminid);
                    return NotFound();
                }
                else
                {
                    _loggerService.Info("All users retrieved", adminid);
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("search")]
        public IActionResult GetUsersSearch([FromBody] string userName)
        {
            if (userName == null) return BadRequest();
            var adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var users = _adminService.GetUsersSearch(userName);
                if (users == null)
                {
                    _loggerService.Error("No users could be retrieved", adminid);
                    return NotFound();
                }
                else
                {
                    _loggerService.Info("All users retrieved", adminid);
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditPermission([FromBody] JObject data)
        {
            var adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var user = _mapper.Map<UserDTO>(await _adminService.EditPermission(data["userid"].ToString(), data["admin"].ToObject<bool>()));
                if (user == null)
                {
                    _loggerService.Error("No user found", adminid);
                    return NotFound();
                }
                else
                {
                    _loggerService.Info("Permission changed", adminid);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
