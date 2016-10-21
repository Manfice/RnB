using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Models.Viewmodels
{
    public class CatDetVm
    {
        public PatyCategory RootCategory { get; set; }
        public IEnumerable<PatyCategory> ChildCategories { get; set; }
    }
}