using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using CAFF_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CAFF_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private ILoggerService _loggerService;
        public UserController(IUserService userService, IMapper mapper, ILoggerService loggerService)
        {
            _userService = userService;
            _mapper = mapper;
            _loggerService = loggerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            try
            {
                var result = await _userService.CreateUser(user, userDTO.Password);
                if (!result.Succeeded) {
                    _loggerService.Error(result.Errors.ToString(), null);
                    return BadRequest(result); 
                }
                else
                {
                    _loggerService.Info("Successful registration", user.Id);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                var user = await _userService.Login(userDTO.UserName, userDTO.Password);
                if (user == null) 
                {
                    _loggerService.Error("User not found", null);
                    return NotFound(); 
                }
                else
                {
                    _loggerService.Info("Login successful", user.Id);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                await _userService.Logout();
                _loggerService.Info("Logout successful", userid);
                return Ok();
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            try
            {
                var user = await _userService.GetUser(userid);

                if (user == null)
                {
                    _loggerService.Error("User not found", userid);
                    return BadRequest();
                }
                else
                {
                    _loggerService.Info("Successful user retrieval", userid);
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditProfile([FromBody] UserDTO userDTO)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _mapper.Map<User>(userDTO);
            user.Id = userid;

            try
            {
                var result = await _userService.UpdateUser(user);
                if (!result.Succeeded)
                {
                    _loggerService.Error(result.Errors.ToString(), userid);
                    return BadRequest(result);
                }
                else
                {
                    _loggerService.Info("Profile edit successful", userid);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("editpassword")]
        public async Task<IActionResult> EditPassword([FromBody] JObject data)
        {
            var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var result = await _userService.UpdateUserPassword(userid, data["oldpassword"].ToString(), data["newpassword"].ToString());
                if(!result.Succeeded)
                {
                    _loggerService.Error(result.Errors.ToString(), userid);
                    return BadRequest();
                }
                else
                {
                    _loggerService.Info("Password edit successful", userid);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
    }
}