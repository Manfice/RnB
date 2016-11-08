using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface ICustomer
    {
        Customer GetCustomerByUserId(string id);
    }
}