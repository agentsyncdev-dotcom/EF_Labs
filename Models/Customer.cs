namespace EF_Lab.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public List<Order> Orders { get; set; }
    }
}
