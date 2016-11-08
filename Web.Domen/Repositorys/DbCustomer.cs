using System;
using System.Linq;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbCustomer : ICustomer
    {
        private readonly Context _context = new Context();
        public Customer GetCustomerByUserId(string id)
        {
            return _context.Customers.SingleOrDefault(customer => customer.User == id);
        }
    }
}