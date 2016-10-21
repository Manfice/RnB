using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IHome
    {
        IEnumerable<Paty> GetPatys { get; }  
        IEnumerable<PatyCategory> GetCategorys { get; }
        Paty GetPaty(int id);
    }
}