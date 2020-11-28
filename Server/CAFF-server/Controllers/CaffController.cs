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
    [Authorize]
    public class CaffController : ControllerBase
    {
        private ICaffService _caffService;
        private IMapper _mapper;
        private IConfiguration _config;
        private ILoggerService _loggerService;
        public CaffController(ICaffService caffService, IMapper mapper, IConfiguration configuration, ILoggerService loggerService)
        {
            _caffService = caffService;
            _mapper = mapper;
            _config = configuration;
            _loggerService = loggerService;
        }

        [HttpGet("all")]
        public IActionResult GetAllCaff()
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var caffs = _mapper.Map<IEnumerable<CAFFDTO>>(_caffService.GetAllCaff());
                if (caffs == null)
                {
                    _loggerService.Error("No caffs found", userid);
                    return NotFound();
                }
                else
                {
                    foreach (CAFFDTO caff in caffs)
                    {
                        var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".bmp");
                        caff.Preview = System.IO.File.ReadAllBytes(path);
                        caff.StoredFileName = null;
                    }
                    _loggerService.Info("All caffs retrieved", userid);

                    return Ok(caffs);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("search")]
        public IActionResult GetCaffSearch([FromBody] string filename)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var caffs = _mapper.Map<IEnumerable<CAFFDTO>>(_caffService.GetCaffSearch(filename));
                if (caffs == null)
                {
                    _loggerService.Error("No caffs found", userid);
                    return NotFound();
                }
                else
                {
                    foreach (CAFFDTO caff in caffs)
                    {
                        var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".bmp");
                        caff.Preview = System.IO.File.ReadAllBytes(path);
                        caff.StoredFileName = null;
                    }
                    _loggerService.Info("Caffs retrieved", userid);

                    return Ok(caffs);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        [HttpGet("{id}")]
        public IActionResult GetCaff(int id)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var caff = _mapper.Map<CAFFDTO>(_caffService.GetCaff(id));
                if (caff == null)
                {
                    _loggerService.Error("No caff found", userid);
                    return NotFound();
                }
                else
                {
                    var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".bmp");
                    caff.Preview = System.IO.File.ReadAllBytes(path);
                    caff.StoredFileName = null;
                    _loggerService.Info("All caffs retrieved", userid);

                    return Ok(caff);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadCaff(int id)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var caff = _caffService.GetCaff(id);
                if (caff == null)
                {
                    _loggerService.Error("No caff found", userid);
                    return NotFound();
                }
                else
                {
                    var path = Path.Combine(_config["StoredFilesPath"], caff.StoredFileName + ".caff");
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    var file = File(fileBytes, "multipart/encrypted", caff.OriginalFileName);

                    _loggerService.Info("File downloaded", userid);
                    return Ok(file);
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
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

                    var resultCode = _caffService.AddCaff(Path.GetFullPath(filePath), file.File.FileName, userid);
                    if (resultCode == 0) { _loggerService.Info("Caff file saved", userid); return Ok(); }
                    else if (resultCode == 2) { _loggerService.Error("Hiba a CAFF fájl megnyitása közben", userid); return StatusCode(StatusCodes.Status500InternalServerError); }
                    else { _loggerService.Error("Hiba parsolás közben: hibás fájl", userid); return BadRequest("Hiba parsolás közben: hibás fájl"); }
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles="Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCaff(int id)
        {
            string adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                _caffService.DeleteCaff(id);
                _loggerService.Info("Caff deleted successfully", adminid);
                return Ok();
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, adminid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}