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
    public class DbCustomer : ICustomer
    {
        private readonly Context _context = new Context();

        public Customer CreateCustomerByUser(Customer customer)
        {
            var dbCustomer = _context.Customers.FirstOrDefault(cus => cus.Email.Equals(customer.Email, StringComparison.CurrentCultureIgnoreCase));
            if (dbCustomer == null)
            {
                dbCustomer = customer;
                dbCustomer.ShowData = "00000000000N";
                dbCustomer.Company = new Company();
                dbCustomer.EmailNotice = true;
                dbCustomer.SmsNotice = true;
                dbCustomer.PhoneNotice = true;
                _context.Customers.Add(dbCustomer);
            }
            else
            {
                if (string.IsNullOrEmpty(dbCustomer.ShowData))
                {
                    dbCustomer.ShowData = "00000000000N";
                }
                dbCustomer.User = customer.User;
                if (dbCustomer.Company==null)
                {
                    dbCustomer.Company=new Company();
                }
            }
            _context.SaveChanges();
            return dbCustomer;
        }

        public Customer UpdateCustomer(Customer model)
        {
            var customer = _context.Customers.Find(model.Id) ?? new Customer();
            customer.ShowData = model.ShowData;
            customer.Fio = model.Fio;
            customer.Phone = model.Phone;
            customer.Email = model.Email;
            customer.City = model.City;
            customer.Birthday = model.Birthday;
            customer.WorkPlace = model.WorkPlace;
            customer.Facebook = model.Facebook;
            customer.Insta = model.Insta;
            customer.Vk = model.Vk;
            customer.Od = model.Od;
            customer.Twit = model.Twit;
            customer.AboutMe = model.AboutMe;
            customer.EmailNotice = model.EmailNotice;
            customer.SmsNotice = model.SmsNotice;
            customer.PhoneNotice = model.PhoneNotice;
            if (model.Avatar != null) customer.Avatar = model.Avatar;
            if (customer.Id == 0) customer.User = model.User;
            if (customer.Id == 0) _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer;
        }

        public Customer GetCustomerByUserId(string id)
        {
            return _context.Customers.SingleOrDefault(customer => customer.User == id);
        }

        public IEnumerable<Paty> GetSuggestedPaties(string id)
        {
            return _context.Paties.Where(paty => paty.PatyDate > DateTime.Today).ToList();
        }

        public IEnumerable<Paty> GetVisitedPaties(string id)
        {
            var orders = _context.Orders.Where(order => order.Customer.User == id).ToList();
            var paties = orders.Select(order => order.Paty).ToList();

            return paties;
        }

        public void SetAvatar(int id, CustImage ava)
        {
            var dbCustomer = _context.Customers.Find(id);
            if (dbCustomer.Avatar!=null)
            {
                var avatar = _context.CustImages.Find(dbCustomer.Avatar.Id);
                _context.CustImages.Remove(avatar);
            }
            dbCustomer.Avatar = ava;
            _context.SaveChanges();
        }

        public void SetCompanyAvatar(int id, CustImage ava)
        {
            var dbCompany = _context.Companies.Find(id);
            
            if (dbCompany.Image != null)
            {
                var avatar = _context.CustImages.Find(dbCompany.Image.Id);
                _context.CustImages.Remove(avatar);
            }
            dbCompany.Image = ava;
            _context.SaveChanges();
        }

        public void UpdateMyCompany(Company model)
        {
            var company = _context.Companies.Find(model.Id);
            company.Title = model.Title;
            company.Descr = model.Descr;
            _context.SaveChanges();
        }

        public IEnumerable<Order> GetMyOrders(string id)
        {
            var customer = _context.Customers.FirstOrDefault(customer1 => customer1.User == id)?.Id;
            return _context.Orders.Where(order => order.Customer.Id == customer).ToList();
        }

        public async Task<Customer> GetCustomerByUser(string userId)
        {
            var result = await _context.Customers.FirstOrDefaultAsync(c => c.User.Equals(userId));
            return result;
        }
    }
}