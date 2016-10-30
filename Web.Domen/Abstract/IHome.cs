using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IHome
    {
        IEnumerable<Paty> GetPatys { get; }  
        IEnumerable<PatyCategory> GetCategorys { get; }
        Paty GetPaty(int id);
        Task<Customer> GetCustomerAsync(OrderViewmodel model);
        Task<Order> RegOnPatyAsync(int id,int places,decimal descount,Customer customer);
    }
}