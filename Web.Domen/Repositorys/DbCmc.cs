using System;
using System.Collections.Generic;
using System.Linq;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbCmc : ICmc
    {
        private readonly Context _context;

        public DbCmc(Context context)
        {
            _context = context;
        }

        public IEnumerable<Blog> GetBlogs => _context.Blogs.ToList();

        public void DeleteBlog(int id)
        {
            var dbBlog = _context.Blogs.Find(id);
            _context.Blogs.Remove(dbBlog);
            _context.SaveChanges();
        }

        public Blog GetBlogById(int id)
        {
            return _context.Blogs.Find(id);
        }

        public Blog SaveBlog(Blog model)
        {
            if (model.Id > 0)
            {
                var dbBlog = GetBlogById(model.Id);
                dbBlog.Title = model.Title;
                dbBlog.BlogData = model.BlogData;
                dbBlog.BlogBody = model.BlogBody;
                _context.SaveChanges();
                return dbBlog;
            }
            else
            {
                _context.Blogs.Add(model);
                _context.SaveChanges();
                return model;
            }
        }
    }
}