using EF_Lab.Models;
using Microsoft.AspNetCore.Mvc;

namespace EF_Lab.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EFController : ControllerBase
    {
        private readonly AppDBContext _context;
        public EFController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Customer> GetAllCustomers()
        {
            var query = _context.Customers;
            return query.ToList(); 
        }

        [HttpGet("sortbyname")]
        public IEnumerable<Customer> GetAllCustomersAndSortByName()
        {
            var query = _context.Customers;
            return query.OrderBy(c => c.FirstName).ToList();
        }

        [HttpGet("filterbyname")]
        public IEnumerable<Customer> GetAllCustomersAndFilterByName([FromQuery] string name)
        {
            var query = _context.Customers;
            return query.Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name)).ToList();
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }   

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Ok(customer);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, Customer updatedCustomer)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.Email = updatedCustomer.Email;
            customer.Phone = updatedCustomer.Phone;
            customer.DateOfBirth = updatedCustomer.DateOfBirth;
            customer.IsActive = updatedCustomer.IsActive;
            _context.SaveChanges();
            return Ok(customer);
        }

        [HttpPatch("{id}/Email")]
        public IActionResult PatchCustomer(int id, [FromBody] string email)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            
            customer.Email = email;
            
            _context.SaveChanges();
            return Ok(customer);
        }

    }
}
