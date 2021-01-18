using FeedbackAPI.Domain;
using FeedbackAPI.DomainServices;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using NLog;


namespace FeedbackAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IProductServices _productServices;

        public ProductsController(IProductServices services)
        {
            _productServices = services;
        }
        
        [HttpGet("/api/products")]
        public ActionResult<List<Product>> GetProducts()
        {
            var rez = _productServices.GetProducts();
            if (rez.IsSuccess)
            {
                return Ok(rez.Value);
            }

            return NotFound(rez.Error);
        }

        [HttpGet("/api/products/search")]
        public ActionResult<List<Product>> Search([FromQuery] ProductSearchModel model)
        {
            _logger.Debug($"Looking for product by model [{model}]");
            //method is safe, because always return result, try catch inside ... 
            var productsResult = _productServices.GetProducts(model);

            if (productsResult.IsSuccess)
            {
                return Ok(productsResult.Value);
            }

            return NotFound(productsResult.Error);
        }

        [HttpPost("/api/products")]
        public ActionResult<List<Product>> AddProducts(List<Product> products)
        {
            _logger.Debug($"Adding [{products.Count}] products");
            var result = _productServices.AddProduct(products);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }

        [HttpPut("/api/products/{id}")]
        public ActionResult<Product> UpdateProduct(string id, Product product)
        {
            _logger.Debug($"Updating product [{id}]");

            var result = _productServices.UpdateProduct(id, product);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }

        [HttpDelete("/api/products/{id}")]
        public ActionResult<string> Deleteproduct(string id)
        {
            _logger.Debug($"Deleting product by id [{id}]");
            var rezult = _productServices.DeleteProduct(id);
            if (rezult.IsSuccess)
            {
                return Ok(rezult.Value);
            }

            return NotFound(rezult.Error);
        }

        [HttpPost("/api/products/feedback/{id}")]
        public ActionResult<Product> AddFeedback(string id, KeyValuePair<string,int> feedback)
        {
            _logger.Debug($"Add feedback to  [{id}]");

            var result = _productServices.AddFeedback(id,feedback);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error);
        }

        [HttpGet("/api/products/test")]
        public ActionResult<KeyValuePair<string,int>> Test()
        {
            _logger.Debug("Test entered");
            return Ok("working");
        }
    }
}