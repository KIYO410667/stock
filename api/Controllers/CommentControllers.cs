using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Dtos.Comment;
using Microsoft.AspNetCore.Http.HttpResults;
using api.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentControllers : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        private readonly UserManager<AppUser> _userManager;
        public CommentControllers(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentRepo.GetAllAsync();

            var commentDto = comment.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var comment = await _commentRepo.GetByIdAsync(id);
            
            if(comment == null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }


        [HttpPost("{stockId:int}")]
        [Authorize]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            if(!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock doesn't exist");
            }

            var username = User.FindFirstValue(ClaimTypes.GivenName);
            if(string.IsNullOrEmpty(username))
            {
                return Unauthorized("User not authenticated or username Claim not be found");
            }
            var appUser = await _userManager.FindByNameAsync(username);

 
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = appUser.UserName;

            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());

        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());

            if(comment == null)
            {
                NotFound("Comment does not exist");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var commentModel = await _commentRepo.DeleteAsync(id);
            if(commentModel == null)
            {
                NotFound("Comment does not exist");
            }
            return Ok(commentModel);

        }
    }
}