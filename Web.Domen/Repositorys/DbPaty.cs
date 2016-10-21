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
        public IEnumerable<Paty> GetPatys => _context.Paties.ToList(); 

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

        public async Task<Paty> AddPatyAsync(int c, int a, Paty model, PatyImage image)
        {
            var result = new Paty();
            var dbCat = await _context.PatyCategories.FindAsync(c);
            if (dbCat==null)
            {
                return null;
            }
            if (model.Id > 0)
            {
                result = await _context.Paties.FindAsync(model.Id);
                result.AddRate = model.AddRate;
                result.Descr = model.Descr;
                result.Dres = model.Dres;
                result.MaxGuests = model.MaxGuests;
                result.PatyDate = model.PatyDate;
                result.PatyInterest = model.PatyInterest;
                result.Price = model.Price;
                result.Title = model.Title;
            }
            else
            {
                result = model;
                _context.Paties.Add(result);
                result.Category = dbCat;
            }

            if (image!=null)
            {
                _context.PatyImages.Add(image);
                result.Avatar = image;
                if (a > 0)
                {
                    var dbImage = await _context.PatyImages.FindAsync(a);
                    _context.PatyImages.Remove(dbImage);
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<PatyCategory> DeleteCategoryAsync(int id)
        {
            var dbCat = await _context.PatyCategories.FindAsync(id);

            if (dbCat!=null)
            {
                var pCat = await _context.PatyCategories.Where(c => c.ParentCategory.Id == dbCat.Id).ToListAsync();
                if (pCat.Any())
                {
                    _context.PatyCategories.RemoveRange(pCat);
                }
                _context.PatyCategories.Remove(dbCat);

            }
            await _context.SaveChangesAsync();
            return dbCat;
        }

        public async void DeleteImagesAsync(List<PatyImage> images)
        {
            if (images.Any())
            {
                _context.PatyImages.RemoveRange(images);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<PatyImage>> GetImagesAsync(int id)
        {
            var imgList = new List<PatyImage>();
            var dbCat = await _context.PatyCategories.FindAsync(id);
            var sDb = await _context.PatyCategories.Where(c => c.ParentCategory.Id == dbCat.Id).ToListAsync();
            if (sDb!=null)
            {
                imgList.AddRange(sDb.Select(category => category.Avatar));
            }
            imgList.Add(dbCat.Avatar);
            return imgList;
        }

        public async Task<PatyImage> ImgToDeleteAsync(int id)
        {
            var result = await _context.PatyImages.FindAsync(id);

            return result;
        }
    }
}