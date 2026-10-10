using System.Diagnostics;
using EF_Lab.Data;
using EF_Lab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF_Lab.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ProductsController(AppDBContext context)
        {
            _context = context;
        }

        // Accepts a list of products and compares individual inserts vs batch insert timings.
        [HttpPost("measure-bulk-insert")]
        public async Task<IActionResult> MeasureBulkInsert([FromBody] List<Product> products)
        {
            if (products == null || products.Count == 0)
                return BadRequest("Provide a non-empty list of products in the request body.");

            // Ensure a clean slate for measurement
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Make copies so both tests use equivalent data
            var individualList = products.Select(p => new Product
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Category = p.Category,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();

            var batchList = products.Select(p => new Product
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Category = p.Category,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();

            var sw = new Stopwatch();

            // Individual inserts (SaveChanges per entity)
            sw.Start();
            foreach (var prod in individualList)
            {
                _context.Products.Add(prod);
                await _context.SaveChangesAsync();
            }
            sw.Stop();
            var individualElapsedMs = sw.Elapsed.TotalMilliseconds;
            var individualCount = await _context.Products.CountAsync();

            // Clear DB before batch insert
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Batch insert using AddRange + single SaveChanges
            // Disable AutoDetectChanges for large batches to improve performance
            var previousAutoDetect = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            sw.Restart();
            await _context.Products.AddRangeAsync(batchList);
            await _context.SaveChangesAsync();
            sw.Stop();

            // Restore tracker behavior for safety
            _context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect;
            var batchElapsedMs = sw.Elapsed.TotalMilliseconds;
            var batchCount = await _context.Products.CountAsync();

            // Clear DB before optimized bulk insert
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Optimized bulk-insert (async) using chunking inside a transaction with AutoDetectChanges disabled
            var chunkSize = Math.Min(2000, products.Count);
            var optimizedElapsedMs = 0.0;
            var optimizedCount = 0;

            previousAutoDetect = _context.ChangeTracker.AutoDetectChangesEnabled;
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            sw.Restart();
            await using (var tx = await _context.Database.BeginTransactionAsync())
            {
                for (int i = 0; i < products.Count; i += chunkSize)
                {
                    var chunk = products.Skip(i).Take(chunkSize).Select(p => new Product
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        Category = p.Category,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity
                    }).ToList();

                    await _context.Products.AddRangeAsync(chunk);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    optimizedCount += chunk.Count;
                }

                await tx.CommitAsync();
            }
            sw.Stop();
            optimizedElapsedMs = sw.Elapsed.TotalMilliseconds;

            // Restore tracker behavior
            _context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetect;

            var optimizedTotalCount = await _context.Products.CountAsync();

            return Ok(new
            {
                individual = new { count = individualCount, elapsedMs = individualElapsedMs },
                batch = new { count = batchCount, elapsedMs = batchElapsedMs },
                bulkInsertOptimizedAsync = new { count = optimizedTotalCount, elapsedMs = optimizedElapsedMs }
            });
        }
    }
}
