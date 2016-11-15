using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
        public IEnumerable<Paty> GetPatys => _context.Paties.Include(paty => paty.Orders).ToList(); 

        public async Task<PatyCategory> AddCategoryAsync(int p, int a,PatyCategory model, PatyImage image)
        {
            PatyCategory parent = null;
            PatyCategory current;

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

        public async Task<PatyActionResult> AddPatyAsync(int a, Paty model, PatyImage image)
        {
            Paty result = null;
            var action = new PatyActionResult();
            var dbCat = await _context.PatyCategories.FindAsync(model.Category.Id);
            if (dbCat==null)
            {
                action.Success = false;
                action.Errors = new[] { "Событие не имеет категории." };
                return action;
            }

            if (model.Id > 0)
            {
                var orders = await _context.Orders.Where(o => o.Paty.Id == model.Id).ToListAsync();
                if (!orders.Any())
                {
                    result = await _context.Paties.FindAsync(model.Id); //1
                    result.PatyDate = model.PatyDate; //2
                    result.Title = model.Title; //3
                    result.Descr = model.Descr; //4
                    result.MaxGuests = model.MaxGuests; //5
                    result.Price = model.Price; //6
                    result.PatyInterest = model.PatyInterest; //7
                    result.AddRate = model.AddRate; //8
                    result.Dres = model.Dres; //9
                    result.Place = model.Place; //11
                    result.Seets = MakeSeats(model.MaxGuests); //10
                }
                else
                {
                    action.Success = false;
                    action.Errors = new[] {"Нельзя редактировать компанию, в которой есть заказы!"};
                    return action;
                }
            }
            else
            {
                result = model;
                result.Seets = MakeSeats(model.MaxGuests); //10
                result.Category = dbCat;
                _context.Paties.Add(result);
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
            action.Success = true;
            action.Paty = result;
            return action;
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

        public async Task<PatyActionResult> DeletePatyAsync(int id)
        {
            var result = await _context.Paties.FindAsync(id);
            if (result.Orders.Any())
            {
                return new PatyActionResult
                {
                    Success = false,
                    Errors = new []{"Нельзя удалить мероприятия на которое проданы места!"},
                    Paty = result
                };
            }
            _context.Paties.Remove(result);
            await _context.SaveChangesAsync();
            return new PatyActionResult
            {
                Success = true,
                Errors = null,
                Paty = result
            };
        }

        public PatyCategory GetCategoryById(int id)
        {
            return _context.PatyCategories.Include(category => category.Paties).Include(category => category.Avatar).Include(category => category.ParentCategory).FirstOrDefault(category => category.Id == id);
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

        //public async Task<PatyActionResult> GetPatyByIdAsync(int id)
        //{
        //    var dbPaty = await _context.Paties.FindAsync(id);

        //    return dbPaty == null
        //        ? new PatyActionResult
        //        {
        //            Success = false,
        //            Paty = null,
        //            Errors = new[] {"Такого мероприятия не существует! Обновите страницу"}
        //        }
        //        : new PatyActionResult
        //        {
        //            Paty = dbPaty,
        //            Success = true,
        //            Errors = null
        //        };

        //}

        public async Task<PatyImage> ImgToDeleteAsync(int id)
        {
            var result = await _context.PatyImages.FindAsync(id);

            return result;
        }

        public PatyCategory SavePatyCategory(int parent, PatyCategory model, PatyImage image)
        {
            PatyCategory _parent = null;
            PatyCategory _current;
            var img = 0;
            if (parent>0)
            {
                _parent = GetCategoryById(parent);
            }

            if (model.Id > 0)
            {
                _current = GetCategoryById(model.Id);
                _current.Title = model.Title;
                _current.Description = model.Description;
                img = _current.Avatar.Id;
            }
            else
            {
                _current = model;
                _current.ParentCategory = _parent;
                _context.PatyCategories.Add(_current);
            }
            if (image != null)
            {
                _context.PatyImages.Add(image);
            }
            _current.Avatar = image;
            if (img>0)
            {
                var dbAva = _context.PatyImages.Find(img);
                _context.PatyImages.Remove(dbAva);
            }
            _context.SaveChanges();
            return _current;
        }

        private static string MakeSeats(int maxGuests)
        {
            if (maxGuests<=0)
            {
                return string.Empty;
            }
            var seets = new StringBuilder();
            for (var i = 1; i < maxGuests; i++)
            {
                seets.Append(i+",");
            }
            seets.Append(maxGuests);
            return seets.ToString();
        }

        public async Task<Paty> GetPatyByIdAsync(int id)
        {
            return await _context.Paties.FindAsync(id);
        }

        public PatyCategory GetCategoryByRoute(string routeUrl)
        {
            return _context.PatyCategories.FirstOrDefault(category => category.RouteTitle.Contains(routeUrl));
        }
    }
}