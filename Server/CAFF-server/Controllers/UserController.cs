using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        //[Authorize(Roles = Role.ADMIN)]
        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
        {
            var identity = User.Identity;
            try
            {
                var result = await _userService.CreateUser(userDTO);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Create", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            try
            {
                var result = await _userService.RegisterUser(userDTO);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Register", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize(Roles = Role.ADMIN)]
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] UserDTO userDTO)
        {
            try
            {
                var result = await _userService.UpdateUser(userDTO);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Update", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize(Roles = Role.SELF_MODIFICATION)]
        [HttpPut("self")]
        public async Task<IActionResult> UpdateSelf([FromBody] UserDTO userDTO)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            if (userEmail != userDTO.Email) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var result = await _userService.UpdateUser(userDTO);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Update self", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize(Roles = Role.ADMIN)]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            try
            {
                var result = await _userService.DeleteUser(email);
                if (!result.Succeeded) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize(Roles = Role.SELF_MODIFICATION)]
        [HttpDelete("self")]
        public async Task<IActionResult> DeleteSelf()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            if (userEmail == null) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var result = await _userService.DeleteUser(userEmail);
                if (!result.Succeeded) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize(Roles = Role.ADMIN)]
        [HttpGet("{email}")]
        public async Task<UserDTO> GetUserByEmail(string email)
        {
            try
            {
                var result = await _userService.GetUserByEmail(email);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //[Authorize(Roles = Role.ADMIN)]
        [HttpGet("all")]
        public async Task<List<UserDTO>> GetUsers()
        {
            try
            {
                var result = await _userService.GetUsers();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
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

        //[Authorize]
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
    }
}