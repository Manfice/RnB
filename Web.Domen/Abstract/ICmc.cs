﻿using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface ICmc
    {
        IEnumerable<Blog> GetBlogs { get; }
        Blog GetBlogById(int id);
        Blog SaveBlog(Blog model);
        void DeleteBlog(int id);
        IEnumerable<Otziv> GetOtzivs { get; }
        void AddOtziv(Otziv model);
        void DeleteOtziv(int id);
    }
}