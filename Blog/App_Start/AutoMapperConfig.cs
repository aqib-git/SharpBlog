using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Blog.Models;

namespace Blog
{
    public class AutoMapperConfig
    {
        public static void configure()
        {
            Mapper.Initialize(m => {
                m.CreateMap<PostDto, Post>();

                m.CreateMap<UserDto, ApplicationUser>();
                m.CreateMap<ApplicationUser, UserDetailViewModel>();
                m.CreateMap<UserDetailViewModel, ApplicationUser>();

                m.CreateMap<MediaDto, Media>();
                m.CreateMap<MediaViewModel, Media>();
            });
        }
    }
}