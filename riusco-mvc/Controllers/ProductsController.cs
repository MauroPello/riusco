using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using riusco_mvc.Data;
using riusco_mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace riusco_mvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ProductsController(MainDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByQuery([FromForm] string query)
        {
            var products = await _context.Products.ToListAsync();
            if (query == null)
                return products;
                
            var keywords = query.Split(' ').ToList();
            if (keywords.Count == 0)
                return products;

            var output = new List<ProductDTO>();
            foreach (var product in products)
            {
                foreach (var keyword in keywords)
                {
                    if (product.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        product.Description.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                    {
                        output.Add(product);
                        break;
                    }
                }
            }

            return output;
        }

        [HttpGet]
        public async Task<ActionResult<ProductDTO>> GetProduct(int value)
        {
            var product = await _context.Products.FindAsync(value);
            if (product == null)
                return NotFound();

            return product;
        }
        
        [HttpPut]
        public async Task<IActionResult> PutProduct(int value, [FromForm] Product product, [FromForm] string apiKey)
        {
            var oldProduct = await _context.Products.FindAsync(value);
            if (oldProduct == null || !oldProduct.IsAvailable)
                return NotFound();
            
            var user = await _context.Users.FindAsync(oldProduct.OwnerID);
            if (apiKey != _configuration["apiKey"] && apiKey != user.ApiKey)
                return BadRequest();
            
            _context.Entry(oldProduct).State = EntityState.Detached;
            if (product.Name != null)
                oldProduct.Name = product.Name;
            
            if (product.Description != null)
                oldProduct.Description = product.Description;
            
            var oldImage = oldProduct.Image;
            if (product.Image.FileName != "no_image")
                oldProduct.Image = UploadImage(product.Image);
            
            oldProduct.LastUpdate = DateTime.Now;
            _context.Entry(oldProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                if (product.Image.FileName != "no_image")
                    DeleteImage(oldImage);
            }
            catch (DbUpdateConcurrencyException)
            {
                DeleteImage(oldProduct.Image);
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct([FromForm] Product product, [FromForm] int userId, [FromForm] string apiKey)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || (apiKey != _configuration["apiKey"] && apiKey != user.ApiKey))
                return BadRequest();

            var newProduct = new ProductDTO(product.Name, product.Description, UploadImage(product.Image), DateTime.Now, true, user.UserID);
            await _context.Products.AddAsync(newProduct);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            
            return CreatedAtAction("GetProduct", new { value = newProduct.Name }, newProduct);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int value, [FromForm] string apiKey)
        {
            var product = await _context.Products.FindAsync(value);
            if (product == null)
                return NotFound();
            
            var user = await _context.Users.FindAsync(product.OwnerID);
            if (apiKey != _configuration["apiKey"] && apiKey != user.ApiKey)
                return BadRequest();
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            DeleteImage(product.Image);

            return Ok();
        }

        
        private string UploadImage(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            var imageName = Guid.NewGuid() + extension;
            if (extension == "" || (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".svg"))
                return "default_product_picture.png";

            using var fileImage = Image.Load(image.OpenReadStream());
            if (fileImage.Height > 2000)
                fileImage.Mutate(x => x.Resize(0, 2000));
            if (fileImage.Width > 2000)
                fileImage.Mutate(x => x.Resize(2000, 0));
                        
            fileImage.Save(Path.Combine(_environment.WebRootPath, "images", "products", imageName));

            return imageName;
        }

        private void DeleteImage(string imageName)
        {
            if (imageName != "default_product_picture.png")
                System.IO.File.Delete(Path.Combine(_environment.WebRootPath, "images", "products", imageName));
        }
    }
}