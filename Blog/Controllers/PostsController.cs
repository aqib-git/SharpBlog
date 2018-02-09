using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blog.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    [Authorize]
    public class PostsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public IHttpActionResult Index()
        {
            var UserId = User.Identity.GetUserId();
            return Ok(db.Posts.Include("User").Where(p => p.UserId == UserId));
        }

        [HttpPost]
        public async Task<IHttpActionResult> Create(Post Post)
        {
            Post.UserId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.Posts.Add(Post);

            await db.SaveChangesAsync();

            return Ok(Post);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
