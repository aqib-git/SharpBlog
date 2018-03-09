using Blog.Enums;
using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.IO;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [Authorize]
    [RoutePrefix("api/Media")]
    public class MediaController : ApiController
    {
        String[] mediaExtensions = new String[]{ "image/x-png", "image/gif", "image/jpeg", "video/mp4", "video/ogg", "video/webm" };

        decimal mediaSize = 5;

        private ApplicationDbContext db = new ApplicationDbContext();

       [AllowAnonymous]
       [Route("file/{filename}")]
       [HttpGet]
        public HttpResponseMessage GetMediaFile(string filename)
        {
            var mediaId = new Guid(Regex.Split(filename, "__").Last().Split('.').First());
            var media = db.Media.Where(u => u.Id == mediaId).FirstOrDefault();
            if(media == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            var mediaFilePath = HttpContext.Current.Server.MapPath("~" + media.Path);
            if(!File.Exists(mediaFilePath))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(mediaFilePath));
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(media.Mime);
            return response;
        }

        [ResponseType(typeof(MediaDto))]
        [HttpPost]
        public IHttpActionResult CreateMedia()
        {
            var userId = User.Identity.GetUserId();
            var request = HttpContext.Current.Request;
            if(request.Files.Count == 0)
            {
                return BadRequest("Media File is required");
            }

            var mediaFile = request.Files[0];
            if (!mediaExtensions.Contains(mediaFile.ContentType))
            {
                return BadRequest("File Types Supported are " + string.Join(", ", mediaExtensions));
            }
            if(this.bytesToMB(mediaFile.ContentLength) > this.mediaSize)
            {
                return BadRequest("Max Media size allowed is " + this.mediaSize + " MB");
            }
            var mediaType = mediaFile.ContentType.IndexOf("image/") >= 0 ? MediaTypesEnum.Image : MediaTypesEnum.Video;
            var mediaModel = db.Media.Add(new Media {
                Name = mediaFile.FileName,
                Type = mediaType,
                UserId = userId,
                Mime = mediaFile.ContentType
            });
            db.SaveChanges();
            var dirPath = HttpContext.Current.Server.MapPath("~/Content/Media/" + userId);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            var extension = this.extension(mediaFile);
            var virtualPath = "/Content/Media/" + userId + "/media__" + mediaModel.Id + '.' + extension;
            var filePath = HttpContext.Current.Server.MapPath("~" + virtualPath);
            if (!File.Exists(filePath))
            {
                mediaFile.SaveAs(filePath);
            }
            mediaModel.Path = virtualPath;
            mediaModel.Uri = "/api/media/file/media__" + mediaModel.Id + '.' + extension;
            db.SaveChanges();

            return Ok(Mapper.Map<MediaDto>(mediaModel));
        }

        protected decimal bytesToMB(decimal bytes)
        {
            return bytes / (1024 * 1024);
        }

        protected string extension(HttpPostedFile file)
        {
            return file.FileName.Split('.').Last();
        }
    }
}
