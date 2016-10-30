using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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

        public async Task<Order> RegOnPatyAsync(int id,int places,decimal discount, Customer customer)
        {
            var paty = await _context.Paties.FindAsync(id);
            var pls = GetTickets(places, paty.Seets);
            paty.Seets = pls[1];
            var order = new Order
            {
                Customer = customer,
                Paty = paty,
                Place = places,
                PlaceNumbers = pls[0],
                OrderDate = DateTime.Now,
                TotalCost = paty.Price*places*discount
            };
            _context.Orders.Add(order);
            paty.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;

        }

        private string[] GetTickets(int quantity, string pls)
        {
            var places = pls.Split(',');
            var result = new[] {"",""};
            result[0] = string.Join(",",places.Where((s, i) => i<=quantity-1));
            result[1] = string.Join(",", places.Where((s, i) => i > quantity - 1));

            return result;
        }
    }
}