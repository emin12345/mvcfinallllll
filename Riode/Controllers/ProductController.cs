using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Contexts;
using Riode.Models;
using Riode.ViewModels;

namespace Riode.Controllers
{
    public class ProductController : Controller
    {
        private readonly RiodeDbContext _context;
        public ProductController(RiodeDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ProductDetail(int id)
        {
            var products = await _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            var reviews = await _context.Reviews
				.Where(r => !r.IsDeleted)
				.Include(r => r.User)
				.ToListAsync();

            if (products == null)
            {
				return NotFound();
			}
            var productDetailViewModel = new ProductDetailViewModel
            {
                Id = products.Id,
				Name = products.Name,
				Description = products.Description,
				Price = products.Price,
				OldPrice = products.OldPrice,
				Rating = products.Rating,
				Discount = products.Discount,
				Features = products.Features,
				Material = products.Material,
				CategoryId = products.CategoryId,
				CategoryName = products.Category.Name,
				BrandId = products.BrandId,
				BrandName = products.Brand.Name,
				RecommendedUse = products.RecommendedUse,
				Images = products.Images,
                Reviews = reviews
			};

            var reviewFormViewModel = new ReviewFormViewModel
            {
                ProductId = products.Id
            };

            var productDetailPageViewModel = new ProductDetailPageViewModel
            {
                ProductDetail = productDetailViewModel,
                ReviewForm = reviewFormViewModel
            };

            return View(productDetailPageViewModel);
        }
        [HttpPost]
		[Authorize]
		public async Task<IActionResult> AddReview(int productId, string userId, ProductDetailPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var review = new Review
            {
                ProductId = productId,
                Rating = model.ReviewForm.Rating,
                Comment = model.ReviewForm.Comment,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
				IsDeleted = false,
                User = _context.Users.FirstOrDefault(u => u.UserName == userId)
			};

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductDetail", new { id = productId });
        }
    }
}
