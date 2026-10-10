using EF_Lab.Data;
using EF_Lab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
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
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
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
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            
            customer.Email = email;
            
            _context.SaveChanges();
            return Ok(customer);
        }

        // Insert customers and measure query performance WITH and WITHOUT AsNoTracking
        [HttpGet("measure-astracking")]
        public async Task<IActionResult> MeasureAsNoTracking([FromQuery] int insertCount = 5000)
        {
            // Insert records
            if (insertCount > 0)
            {
                var list = new List<Customer>(insertCount);
                for (int i = 0; i < insertCount; i++)
                {
                    list.Add(new Customer
                    {
                        FirstName = $"First_{DateTime.UtcNow.Ticks % int.MaxValue}_{i}",
                        LastName = $"Last_{i}",
                        Email = $"user{i}@example.com",
                        Phone = 1000000000 + i,
                        DateOfBirth = DateTime.UtcNow.AddYears(-30).AddDays(i % 365),
                        IsActive = true
                    });
                }

                await _context.Customers.AddRangeAsync(list);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }

            // Warm up
            await _context.Customers.Take(1).ToListAsync();
            _context.ChangeTracker.Clear();

            var sw = new Stopwatch();

            // Query WITHOUT AsNoTracking (entities will be tracked)
            sw.Start();
            var tracked = await _context.Customers.ToListAsync();
            sw.Stop();
            var withoutTrackingMs = sw.Elapsed.TotalMilliseconds;
            var trackedCount = tracked.Count;

            // Clear tracker so second query isn't affected
            _context.ChangeTracker.Clear();

            // Query WITH AsNoTracking (read-only)
            sw.Restart();
            var notTracked = await _context.Customers.AsNoTracking().ToListAsync();
            sw.Stop();
            var withAsNoTrackingMs = sw.Elapsed.TotalMilliseconds;
            var notTrackedCount = notTracked.Count;

            return Ok(new
            {
                seeded = insertCount,
                withoutAsNoTracking = new { count = trackedCount, elapsedMs = withoutTrackingMs },
                withAsNoTracking = new { count = notTrackedCount, elapsedMs = withAsNoTrackingMs }
            });
        }

    }
}
