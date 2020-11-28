using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using CAFF_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAFF_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CaffController : ControllerBase
    {
        private ICaffService _caffService;
        private IMapper _mapper;
        private IConfiguration _config;
        public CaffController(ICaffService caffService, IMapper mapper, IConfiguration configuration)
        {
            _caffService = caffService;
            _mapper = mapper;
            _config = configuration;
        }

        [HttpGet("all")]
        public IActionResult GetAllCaff()
        {
            try
            {
                var caffs = _mapper.Map<IEnumerable<CAFFDTO>>(_caffService.GetAllCaff());
                if (caffs == null) return BadRequest();

                foreach(CAFFDTO caff in caffs)
                {
                    var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".bmp");
                    caff.Preview = System.IO.File.ReadAllBytes(path);
                    caff.StoredFileName = null;
                }
                return Ok(caffs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult GetCaffSearch([FromBody] string filename)
        {
            try
            {
                var caffs = _caffService.GetCaffSearch(filename);
                if (caffs == null) return BadRequest();
                else return Ok(caffs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetCaff(int id)
        {
            try
            {
                var caff = _mapper.Map<CAFFDTO>(_caffService.GetCaff(id));
                if (caff == null) return BadRequest();

                var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".bmp");
                caff.Preview = System.IO.File.ReadAllBytes(path);
                caff.StoredFileName = null;
                return Ok(caff);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadCaff(int id)
        {
            try
            {
                var caff = _caffService.GetCaff(id);
                if (caff == null) return BadRequest();

                var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".caff");

                byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                var file = File(fileBytes, "multipart/encrypted", caff.OriginalFileName);
                return Ok(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCaff([FromForm] FormModel file)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                if (file.File == null || !(file.File.Length > 0)) return BadRequest();
                else
                {
                    var storedname = Path.GetRandomFileName();
                    var filePath = Path.Combine(_config["StoredFilesPath"], storedname.Substring(0, storedname.Length-4) + ".caff");
                    
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.File.CopyToAsync(stream);
                    }

                    _caffService.AddCaff(Path.GetFullPath(filePath), file.File.FileName, userid);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles="Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCaff(int id)
        {
            try
            {
                _caffService.DeleteCaff(id);

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