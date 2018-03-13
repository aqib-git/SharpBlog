using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Blog.Models;
using Microsoft.AspNet.Identity;
using AutoMapper;

namespace Blog.Controllers
{
    [Authorize]
    [RoutePrefix("api/Posts")]
    public class PostsController : ApiBaseController
    {
        // GET: api/Posts
        [ResponseType(typeof(ApiResponseList<PostViewModel>))]
        [AllowAnonymous]
        public IHttpActionResult GetPosts()
        {
            var apiPagination = ApiPaginate(db.Posts.Count());

            var posts = db.Posts
                .Include(m => m.Media)
                .Include(u => u.User)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((apiPagination.CurrentPage-1) * apiPagination.PageSize)
                .Take(apiPagination.PageSize);

            var postViewModels = new List<PostViewModel>();
            foreach (var post in posts)
            {
                var postViewModel = Mapper.Map<PostViewModel>(post);
                postViewModel.Media = Mapper.Map<MediaViewModel>(post.Media);
                postViewModel.User = Mapper.Map<UserDetailViewModel>(post.User);
                postViewModels.Add(postViewModel);
            }
            var apiResponse = new ApiResponseList<PostViewModel>
            {
                Data = postViewModels,
                Pagination = apiPagination
            };
            return Ok(apiResponse);
        }

        // GET: api/Posts/5
        [ResponseType(typeof(ApiResponse<PostViewModel>))]
        [AllowAnonymous]
        public IHttpActionResult GetPost(Guid id)
        {
            var post = db.Posts
                .Include(m => m.Media)
                .Include(p => p.User)
                .Where(u => u.Id == id)
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            var postVM = Mapper.Map<PostViewModel>(post);
            postVM.Media = Mapper.Map<MediaViewModel>(post.Media);
            postVM.User = Mapper.Map<UserDetailViewModel>(post.User);

            return Ok(new ApiResponse<PostViewModel> { Data = postVM });
        }

        // PUT: api/Posts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPost(Guid id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != post.Id)
            {
                return BadRequest();
            }

            db.Entry(post).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Posts
        [ResponseType(typeof(PostDto))]
        [HttpPost]
        public async Task<IHttpActionResult> CreatePost(PostDto postDto)
        {
            
            postDto.UserId = User.Identity.GetUserId();

            var post = Mapper.Map<Post>(postDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Posts.Add(post);

            await db.SaveChangesAsync();

            return Ok(postDto);
        }

        [Route("bulk")]
        // Posts: api/posts/all
        public async Task<IHttpActionResult> CreatePostInBulk(IEnumerable<PostDto> postDtos)
        {
            foreach (var postDto in postDtos)
            {
                postDto.UserId = User.Identity.GetUserId();
                postDto.Show = true;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posts = new List<Post>();

            foreach (var postDto in postDtos)
            {
                posts.Add(Mapper.Map<Post>(postDto));
            }

            var postCount = db.Posts.AddRange(posts).Count();

            await db.SaveChangesAsync();

            return Ok(postCount);
        }

        // DELETE: api/Posts/5
        [ResponseType(typeof(PostDto))]
        public async Task<IHttpActionResult> DeletePost(Guid id)
        {
            Post post = await db.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var postDto = Mapper.Map<PostDto>(post);
            db.Posts.Remove(post);
            await db.SaveChangesAsync();

            return Ok(post);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostExists(Guid id)
        {
            return db.Posts.Count(e => e.Id == id) > 0;
        }
    }
}