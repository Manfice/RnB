using System;
using System.Collections.Generic;
using System.Linq;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbPhoto:IPhoto
    {
        private readonly Context _context = new Context();

        public IEnumerable<PhotoAlbom> GetAlboms => _context.Alboms.ToList();

        public void BulkAddPhotos(List<ImageData> data)
        {
            if (data.Any())
            {
                _context.Photos.AddRange(data);
            }
            _context.SaveChanges();
        }

        public void DeleteAlbom(int id)
        {
            var albom = _context.Alboms.Find(id);
            _context.Alboms.Remove(albom);
            _context.SaveChanges();
        }

        public void DeletePhoto(int id)
        {
            var data = _context.Photos.Find(id);
            _context.Photos.Remove(data);
            _context.SaveChanges();
        }

        public PhotoAlbom GetAlbomById(int id)
        {
            return _context.Alboms.Find(id);
        }

        public PatyCategory GetCategoryById(int id)
        {
            return _context.PatyCategories.Find(id);
        }

        public IEnumerable<string> GetChildElements(int albomId)
        {
            var photos = _context.Photos.Where(p => p.Region.Albom.Id == albomId).Select(data => data.FullPath);
            return photos;
        }

        public ImageData Photo(int id)
        {
            return _context.Photos.Find(id);
        }

        public void SaveAlbom(PhotoAlbom albom)
        {
            PatyCategory cat = null;
            if (albom.Category.Id>0)
            {
                cat = GetCategoryById(albom.Category.Id);
            }
            if (albom.Id > 0)
            {
                var dbAlbom = _context.Alboms.Find(albom.Id);
                dbAlbom.AvatarFullPath = albom.AvatarFullPath;
                dbAlbom.Avatar = albom.Avatar;
                dbAlbom.Title = albom.Title;
                dbAlbom.Description = albom.Description;
                dbAlbom.Category = cat;
            }
            else
            {
                albom.Category = cat;
                _context.Alboms.Add(albom);
            }
            _context.SaveChanges();
        }

        public ImageGalary SaveRegion(ImageGalary model)
        {
            var albom = GetAlbomById(model.Albom.Id);
            if (albom == null)
            {
                return null;
            }
            if (model.Id > 0)
            {
                var dbRegion = _context.Galaries.Find(model.Id);
                dbRegion.Title = model.Title;
                dbRegion.Description = model.Description;
            }
            else
            {
                model.Albom = albom;
                _context.Galaries.Add(model);
            }
            _context.SaveChanges();
            return new ImageGalary
            {
                Id = model.Id, Title = model.Title, Albom = albom, Description = model.Description
            };
        }

        public ImageGalary GetGalaryById(int id)
        {
            return _context.Galaries.Find(id);
        }

        public ImageData ShowOnTitlePage(int id)
        {
            var data = _context.Photos.Find(id);
            if (data!=null)
            {
                data.TitleView = !data.TitleView;
            }
            _context.SaveChanges();
            return data;
        }
      
    }
}