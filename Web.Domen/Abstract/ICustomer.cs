using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Abstract
{
    public interface ICustomer
    {
        Customer GetCustomerByUserId(string id);
        IEnumerable<Paty> GetVisitedPaties(string id);
        IEnumerable<Paty> GetSuggestedPaties(string id);
        Customer CreateCustomerByUser(Customer customer);
        Customer UpdateCustomer(Customer model);
        void SetAvatar(int id, CustImage ava);
        void SetCompanyAvatar(int id, CustImage ava);
        void UpdateMyCompany(Company model);
        IEnumerable<Order> GetMyOrders(string id);
    }
}