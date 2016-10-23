using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;

namespace Web.Domen.Repositorys
{
    public class DbHome : IHome
    {
        private readonly Context _context= new Context();

        public IEnumerable<PatyCategory> GetCategorys => _context.PatyCategories.ToList();

        public IEnumerable<Paty> GetPatys => _context.Paties.ToList();

        public async Task<Customer> GetCustomerAsync(OrderViewmodel model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>c.Email==model.Email);
            if (customer != null) return customer;
            customer = new Customer
            {
                Email = model.Email,
                Fio = model.Fio,
                Phone = model.Phone,
                Work = model.Workplace
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public Paty GetPaty(int id)
        {
            return _context.Paties.Find(id);
        }

        public async Task<Order> RegOnPatyAsync(int id, Customer customer)
        {
            var paty = await _context.Paties.FindAsync(id);
            paty.Guests.Add(customer);
            var order = new Order
            {
                Customer = customer,
                Paty = paty,
                Place = 1
            };
            await _context.SaveChangesAsync();

            return order;

        }
    }
}