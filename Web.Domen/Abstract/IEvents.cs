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
        Task<PatyActionResult> AddPatyAsync(int a, Paty model, PatyImage image);
        //Task<PatyActionResult> GetPatyByIdAsync(int id);
        Task<PatyActionResult> DeletePatyAsync(int id);
        PatyCategory GetCategoryById(int id);
        PatyCategory GetCategoryByRoute(string routeUrl);
        PatyCategory SavePatyCategory(int parent,PatyCategory model, PatyImage image);
        Task<Paty> GetPatyByIdAsync(int id);
        bool CheckPatyUrlTitle(string url, int id = 0);
        bool CheckPatyCategoryUrlTitle(string url, int id = 0);
    }
}