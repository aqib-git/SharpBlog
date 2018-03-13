using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blog.Controllers
{
    public class ApiBaseController : ApiController
    {
        protected int pageNumber = 1;
        protected int pageSize = 10;
        protected ApplicationDbContext db = new ApplicationDbContext();

        public class ApiResponseList<T>
        {
            public List<T> Data;
            public ApiResponsePagination Pagination;
            public ApiResponseError Error;
        }

        public class ApiResponse<T>
        {
            public T Data;
            public ApiResponseError Error;
        }

        public  class ApiResponsePagination
        {
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int Total { get; set; }
        }

        public class ApiResponseError
        {
            public string Code { get; set; }
            public string HttpStatus { get; set; }
            public object Errors { get; set; }
            public string ErrorMessage { get; set; }
        }

        public ApiResponsePagination ApiPaginate(int count)
        {
            var pageQueryParameter = Request.GetQueryNameValuePairs().Where(p => p.Key == "page").FirstOrDefault();
            if (!pageQueryParameter.Equals(null))
            {
                int.TryParse(pageQueryParameter.Value, out pageNumber);
            }
            var maxPages = Math.Ceiling((decimal) count / pageSize);
            if (pageNumber > maxPages || pageNumber < 1)
            {
                pageNumber = 1;
            }
            else if (pageNumber == maxPages)
            {
                pageSize = count % pageSize;
            }
            return new ApiResponsePagination
            {
                TotalPages = (int) maxPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }
    }
}
