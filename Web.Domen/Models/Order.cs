namespace Web.Domen.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int Place { get; set; }//кол-во мест
        public string PlaceNumbers { get; set; }
        public virtual Paty Paty { get; set; }
        public virtual Customer Customer { get; set; }
    }

    public class OrderViewmodel
    {
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Workplace { get; set; }
        public int Paty { get; set; }
        public int Place { get; set; }
    }
}