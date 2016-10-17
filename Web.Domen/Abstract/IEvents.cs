using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IEvents
    {
        IEnumerable<PatyCategory> GetCategorys { get; }
        Task<PatyCategory> AddCategoryAsync(int p,int a,PatyCategory model, PatyImage image);
        Task<PatyImage> ImgToDeleteAsync(int id);
    }
}