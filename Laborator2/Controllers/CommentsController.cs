using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laborator2.Services;
using Laborator2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laborator2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ICommentService commentService;

        public CommentsController(ICommentService commentService)
        {
            this.commentService = commentService;
        }
        /// <summary>
        /// GET: api/Comments
        /// </summary>
        /// <param name="filter">Optional , filter by text or a part of text</param>
        /// <returns>A list of comments</returns>
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "Regular,Admin")]
        [HttpGet]
        public IEnumerable<CommentGetModel> Get([FromQuery]string filter)
        {
            return commentService.GetAllComments(filter);

        }
    }
}
