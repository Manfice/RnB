using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IEvents
    {
        IEnumerable<PatyCategory> GetCategorys { get; }
        IEnumerable<Paty> GetPatys { get; }
        Task<PatyCategory> AddCategoryAsync(int p,int a,PatyCategory model, PatyImage image);
        Task<PatyImage> ImgToDeleteAsync(int id);
        Task<List<PatyImage>> GetImagesAsync(int id);
        Task<PatyCategory> DeleteCategoryAsync(int id);
        void DeleteImagesAsync(List<PatyImage> images);
    }
}