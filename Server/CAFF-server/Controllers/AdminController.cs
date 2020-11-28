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
        public AdminController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _adminService.DeleteUser(id);
                if (!result.Succeeded) return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _adminService.GetUser(id);

                if (user == null) return BadRequest();
                else return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _adminService.GetUsers();
                if (users == null) return BadRequest();
                else return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("search")]
        public IActionResult GetUsersSearch([FromBody] string userName)
        {
            if (userName == null) return BadRequest();
            try
            {
                var users = _adminService.GetUsersSearch(userName);
                if (users == null) return BadRequest();
                else return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditPermission([FromBody] JObject data)
        {
            try
            {
                var user = _mapper.Map<UserDTO>(await _adminService.EditPermission(data["userid"].ToString(), data["admin"].ToObject<bool>()));
                if (user == null) return BadRequest();
                else return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
