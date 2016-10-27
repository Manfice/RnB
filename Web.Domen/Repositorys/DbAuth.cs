using System;
using System.Globalization;
using System.Threading.Tasks;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbAuth : IAuth
    {
        private readonly Context _context = new Context();
        public async Task<Customer> RegCustomer(CustomerViewModel model)
        {
            DateTime bD;
            var customer = new Customer
            {
                Fio = model.Title,
                Phone = model.Phone,
                Email = model.Email,
                Rate = 0,
                City = model.City,
                Work = model.Workplace,
                User = model.UserId
            };
            if (DateTime.TryParse(model.Birthday, out bD))
            {
                customer.Birthday = bD;
            }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}