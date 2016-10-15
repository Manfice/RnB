using System;
using System.Collections.Generic;
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

        public async Task<PatyCategory> AddCategoryAsync(PatyCategory model, PatyImage image)
        {
            if (image!=null)
            {
                _context.PatyImages.Add(image);
            }
            _context.PatyCategories.Add(model);
            model.Avatar = image;
            await _context.SaveChangesAsync();
            return model;
        }
    }
}