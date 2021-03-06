﻿using System;
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

        public IEnumerable<ImageData> GetPhotos => _context.Photos.Include(data => data.Region.Albom.Category);
        public IEnumerable<PhotoAlbom> GetAlboms => _context.Alboms.ToList();


        public void AcceptPayAsync(PaymentAviso aviso, Order order)
        {
            if (aviso!=null)
            {
                _context.Avisos.Add(aviso);
            }
            order.Aviso = aviso;
            order.Customer.Rate += order.Paty.AddRate;
            _context.SaveChanges();
        }

        public void DiscardOrderAsync(int id)
        {
            var order = _context.Orders.Find(id);
            if (order!=null)
            {
                _context.Orders.Remove(order);
            }
            _context.SaveChanges();
        }

        public async Task<Customer> GetCustomerAsync(OrderViewmodel model)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c =>c.Email==model.Email);
            if (customer != null)
            {
                if (string.IsNullOrEmpty(customer.Fio) && !string.IsNullOrEmpty(model.Fio))
                        customer.Fio = model.Fio;
                    if (string.IsNullOrEmpty(customer.Phone) && !string.IsNullOrEmpty(model.Phone))
                        customer.Phone = model.Phone;
                
                await _context.SaveChangesAsync();
                return customer;
            }
            customer = new Customer
            {
                Email = model.Email,
                Fio = model.Fio,
                Phone = model.Phone,
                Work = model.Workplace,
                ShowData = "00000000000N",
                EmailNotice = true,
                PhoneNotice = true,
                SmsNotice = true
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(customer => customer.Email == email);
        }

        public Order GetOrderBuId(int id)
        {
            return _context.Orders.Where(order => order.Id==id).Include(order => order.Customer).Include(order => order.Paty).FirstOrDefault();
        }

        public Paty GetPaty(int id)
        {
            return _context.Paties.Find(id);
        }

        public async Task<Order> OrderExistAsync(int patyId, string email)
        {
            return
                await
                    _context.Orders.Include(order => order.Customer)
                        .FirstOrDefaultAsync(order => order.Paty.Id==patyId && order.Customer.Email == email);
        }

        public async Task<Order> RegOnPatyAsync(int id,int places,decimal discount, Customer customer)
        {
            var paty = await _context.Paties.FindAsync(id);
            var pls = GetTickets(places, paty.Seets);
            var cmr = _context.Customers.Find(customer.Id);
            paty.Seets = pls[1];
            var order = new Order
            {
                Customer = cmr,
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
        public void SeeCheck(string s)
        {
            var ord = _context.Orders.Find(35);
            ord.AvisoLog = s;
            _context.SaveChangesAsync();
        }
        private static string[] GetTickets(int quantity, string pls)
        {
            var places = pls.Split(',');
            var result = new[] {"",""};
            result[0] = string.Join(",",places.Where((s, i) => i<=quantity-1));
            result[1] = string.Join(",", places.Where((s, i) => i > quantity - 1));

            return result;
        }

        public PhotoAlbom GetAlbomById(int id)
        {
            return _context.Alboms.Find(id);
        }

        public Paty GetPatyByRouteUrl(string route)
        {
            return _context.Paties.FirstOrDefault(paty => paty.RouteTitle.Contains(route));
        }

        public Customer GetCustomerByEmail(string email)
        {
            return _context.Customers.FirstOrDefault(customer => customer.Email.Equals(email,StringComparison.CurrentCultureIgnoreCase));
        }

        public void AddAlbomView(int id)
        {
            var alb = _context.Alboms.Find(id);
            if (alb!=null)
            {
                alb.Viewed ++;
                _context.SaveChanges();
            }
        }

        public void AddPhotoView(int id)
        {
            var photo = _context.Photos.Find(id);
            if (photo != null)
            {
                photo.Viewed++;
                _context.SaveChanges();
            }
        }

        public async Task LikePhotoInGalary(int id)
        {
            var dbPhoto = await _context.Photos.FindAsync(id);
            if (dbPhoto==null)
            {
                return;
            }
            dbPhoto.Likes += 1;
            await _context.SaveChangesAsync();
        }
    }
}