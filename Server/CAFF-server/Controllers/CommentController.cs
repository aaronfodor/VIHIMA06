using AutoMapper;
using CAFF_server.DTOs;
using CAFF_server.Entities;
using CAFF_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAFF_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private ICommentService _commentService;
        private IMapper _mapper;
        private ILoggerService _loggerService;
        public CommentController(ICommentService commentService, IMapper mapper, ILoggerService loggerService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _loggerService = loggerService;
        }

        [HttpGet("all/{caffid}")]
        public IActionResult GetAllComments(int caffid)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var comments = _commentService.GetAllComment(caffid);
                if (comments == null)
                {
                    _loggerService.Error("Comments not found", userid);
                    return NotFound();
                }
                else
                {
                    _loggerService.Info("Comments retrieved", userid);
                    return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{caffid}")]
        public IActionResult AddComment([FromBody] CommentDTO commentDTO, int caffid)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var com = _commentService.AddComment(_mapper.Map<Comment>(commentDTO), caffid, userid);
                if (com == null)
                {
                    _loggerService.Error("Comment could not be added", userid);
                    return BadRequest();
                }
                else
                {
                    _loggerService.Info("Comment added", userid);
                    return Ok(_mapper.Map<CommentDTO>(com));
                }
            }
            catch (Exception ex)
            {
                _loggerService.Debug(ex.Message, userid);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            string adminid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                _commentService.DeleteComment(id);
                _loggerService.Info("Comment deleted", adminid);
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