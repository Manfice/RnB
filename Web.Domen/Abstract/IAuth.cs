using System.Threading.Tasks;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface IAuth
    {
        Task<Customer> RegCustomer(CustomerViewModel model);
        bool CheckCustomerExist(string id);
    }
}