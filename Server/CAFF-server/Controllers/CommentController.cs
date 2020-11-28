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
        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet("all/{caffid}")]
        public IActionResult GetAllComments(int caffid)
        {
            try
            {
                var comments = _commentService.GetAllComment(caffid);
                if (comments == null) return BadRequest();
                else return Ok(_mapper.Map<IEnumerable<CommentDTO>>(comments));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("{caffid}")]
        public IActionResult AddComment([FromBody] CommentDTO commentDTO, int caffid)
        {
            string userid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                var com = _commentService.AddComment(_mapper.Map<Comment>(commentDTO), caffid, userid);
                if (com == null) return BadRequest();

                return Ok(_mapper.Map<CommentDTO>(com));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            try
            {
                _commentService.DeleteComment(id);
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