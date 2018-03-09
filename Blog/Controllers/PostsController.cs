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
    public class PostsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Posts
        [ResponseType(typeof(List<PostDto>))]
        public IHttpActionResult GetPosts()
        {
            var userId = User.Identity.GetUserId();

            var posts = db.Posts.Where(p => p.User.Id == userId);

            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                postDtos.Add(Mapper.Map<PostDto>(post));
            }

            return Ok(postDtos);
        }

        // GET: api/Posts/5
        [ResponseType(typeof(PostDto))]
        public async Task<IHttpActionResult> GetPost(Guid id)
        {
            Post post = await db.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var postDto = Mapper.Map<PostDto>(post);

            return Ok(postDto);
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
        public async Task<IHttpActionResult> PostPost(PostDto postDto)
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

        [Route("api/Posts/all")]
        // Posts: api/posts/all
        public async Task<IHttpActionResult> PostAll(IEnumerable<PostDto> postDtos)
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