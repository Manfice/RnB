using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Viewmodels
{
    public class PatyCat
    {
         
    }

    public class CategoryViewModel
    {
        public PatyCategory Category { get; set; }
        public IEnumerable<PatyCategory> Categories { get; set; }
    }
}