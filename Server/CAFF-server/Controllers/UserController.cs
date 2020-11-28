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
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            try
            {
                var result = await _userService.CreateUser(user, userDTO.Password);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Register", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                var user = await _userService.Login(userDTO.UserName, userDTO.Password);
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userService.Logout();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

                if (user == null) return BadRequest();
                else return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
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
                if (!result.Succeeded) return BadRequest(result);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [Authorize]
        [HttpPut("editpassword")]
        public async Task<IActionResult> EditPassword([FromBody] JObject data)
        {
            try
            {
                var userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _userService.UpdateUserPassword(userid, data["oldpassword"].ToString(), data["newpassword"].ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
        
    }
}