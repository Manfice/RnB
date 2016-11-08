using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IPhoto
    {
        IEnumerable<PhotoAlbom> GetAlboms { get; }
        Paty GetPatyById(int id);
        void SaveAlbom(PhotoAlbom albom);
        PhotoAlbom GetAlbomById(int id);
        void DeleteAlbom(int id);
        IEnumerable<string> GetChildElements(int albomId);
        void BulkAddPhotos(List<ImageData> data);
        ImageData Photo(int id);
        ImageData ShowOnTitlePage(int id);
        void DeletePhoto(int id);
        ImageGalary SaveRegion(ImageGalary model);
        ImageGalary GetGalaryById(int id);
        void DeleteRegion(int id);
        IEnumerable<string> GetPhotoPath(int id);
    }
}