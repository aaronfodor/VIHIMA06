using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using concertticket_webapp_appserver.DTOs;
using concertticket_webapp_appserver.Entities;
using concertticket_webapp_appserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace concertticket_webapp_appserver.Controllers
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

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody]UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            try
            {
                var result = await _userService.Create(user, userDTO.Password);
                if (!result.Succeeded) return BadRequest(result);

                return CreatedAtAction("Registration",result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]UserDTO userDTO)
        {
            try
            {
                var user = await _userService.SignIn(userDTO.UserName, userDTO.Password);

                if (user == null) return BadRequest(new { message = "Hibás felhasználónév vagy jelszó!" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                await _userService.SignOut();
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