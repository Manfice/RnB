using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbPaty : IEvents
    {
        private readonly Context _context = new Context();
        public IEnumerable<PatyCategory> GetCategorys => _context.PatyCategories.ToList();

        public async Task<PatyCategory> AddCategoryAsync(int p, int a,PatyCategory model, PatyImage image)
        {
            PatyCategory parent = null;
            PatyCategory current = null;

            if (p > 0)
            {
                parent = await _context.PatyCategories.FirstOrDefaultAsync(c => c.Id == p);
            }

            if (model.Id>0)
            {
                current = await _context.PatyCategories.FindAsync(model.Id);
                if (current != null)
                {
                    current.Title = model.Title;
                    current.Description = model.Description;
                    
                }
                else
                {
                    current = model;
                    _context.PatyCategories.Add(current);
                }
            }
            else
            {
                current = model;
                _context.PatyCategories.Add(current);
            }

            current.ParentCategory = parent;

            if (image != null)
            {
                _context.PatyImages.Add(image);
            }

            current.Avatar = image;
            if (a>0)
            {
                var dbava = await _context.PatyImages.FindAsync(a);
                _context.PatyImages.Remove(dbava);
            }
            await _context.SaveChangesAsync();
            return current;
        }

        public async Task<PatyImage> ImgToDeleteAsync(int id)
        {
            var result = await _context.PatyImages.FindAsync(id);

            return result;
        }
    }
}