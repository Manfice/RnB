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
        Paty GetPatyByRouteUrl(string route);
        Task<Customer> GetCustomerAsync(OrderViewmodel model);
        Task<Order> RegOnPatyAsync(int id,int places,decimal descount,Customer customer);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Order GetOrderBuId(int id);
        Task<Order> OrderExistAsync(int patyId, string email);
        void SeeCheck(string s);
        void AcceptPayAsync(PaymentAviso aviso, Order order);
        void DiscardOrderAsync(int id);
        IEnumerable<ImageData> GetPhotos { get; } 
        IEnumerable<PhotoAlbom> GetAlboms { get; }
        PhotoAlbom GetAlbomById(int id);
    }
}