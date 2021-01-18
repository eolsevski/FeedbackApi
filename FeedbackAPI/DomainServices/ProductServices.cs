using CSharpFunctionalExtensions;
using FeedbackAPI.Domain;
using FeedbackAPI.Infrastructure;
using FeedbackAPI.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FeedbackAPI.DomainServices
{
    public class ProductServices : IProductServices
    {
        private static List<Product> _products;

        private List<Product> Products
        {
            get => _cacheManager.GetFromCache<List<Product>>(CacheKeys.Products) ?? _products;
            set
            {
                _cacheManager.SetCache(CacheKeys.Products, value);
                _products = value;
            }
        }

        private readonly ICacheManager _cacheManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProductServices(ICacheManager cacheManager)
        {
            _products = new List<Product>();
            _cacheManager = cacheManager;
        }
        public Result<List<Product>> GetProducts()
        {
            try
            {
                _logger.Trace($"Found [{Products.Count}] products");
                return Products;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<List<Product>>(e.Message);
            }
        }

        public Result<List<Product>> GetProducts(ProductSearchModel model)
        {
            try
            {
                _logger.Trace($"Searching products by model [{model}]");
                IEnumerable<Product> result = Products;

                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    result = result.Where(p => p.Id == model.Id);
                }
                if (!string.IsNullOrWhiteSpace(model.ProductName))
                {
                    result = result.Where(p => p.ProductName == model.ProductName);
                }
                if (!string.IsNullOrWhiteSpace(model.Brand))
                {
                    result = result.Where(p => p.Brand == model.Brand);
                }
                if (model.RatingMin.HasValue)
                {
                    result = result.Where(p => p.Rating >= model.RatingMin);
                }
                if (model.RatingMax.HasValue)
                {
                    result = result.Where(p => p.Rating <= model.RatingMax);
                }
                _logger.Trace($"Success, found [{result.Count()}] entries");
                return Result.Success(result.ToList());
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<List<Product>>(e.Message);
            }
        }

        public Result<List<Product>> AddProduct(List<Product> products)
        {
            try
            {
                if (products is null) return Result.Failure<List<Product>>("Input is null");
                _logger.Trace($"Add [{products.Count}] products");
                //using tmp for triggering setter in Products
                var temp = Products ?? new List<Product>();

                temp.AddRange(products);

                Products = temp;
                _logger.Trace($"Succes");
                return Result.Success(GetProducts().Value);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<List<Product>>(e.Message);
            }
        }

        public Result<Product> UpdateProduct(string id, Product product)
        {
            try
            {
                _logger.Trace($"Updating product by id [{id}]");
                if (product is null || string.IsNullOrWhiteSpace(id)) return Result.Failure<Product>("Input is null");

                var index = ProductIndexFounder(id);
                
                if (index.IsFailure) return Result.Failure<Product>(index.Error);
                
                Products[index.Value] = product;
                
                _logger.Trace("Success");
                return Result.Success(GetProducts(new ProductSearchModel(){Id = id}).Value.First());
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<Product>(e.Message);
            }
        }
        public Result<Product> AddFeedback(string id, KeyValuePair<string, int> feedback)
        {
            try
            {
                _logger.Trace($"Adding feedback to product by id [{id}]");
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(feedback.Key))
                    return Result.Failure<Product>("Input is null");

                var index = ProductIndexFounder(id);
                
                if (index.IsFailure) return Result.Failure<Product>(index.Error);

                var wantedProduct = Products[index.Value];

                wantedProduct.FeedBacks ??= new Dictionary<string, int>();
                
                wantedProduct.FeedBacks.Add(feedback.Key, feedback.Value);
                
                Products[index.Value] = wantedProduct;
                _logger.Trace("Success");
                return Result.Success(GetProducts(new ProductSearchModel() { Id = id }).Value.First());
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<Product>(e.Message);
            }
        }

        public Result<string> DeleteProduct(string id)
        {
            try
            {
                _logger.Trace($"Deleting product by id [{id}]");
                if (string.IsNullOrWhiteSpace(id)) return Result.Failure<string>("Input is null");

                var index = ProductIndexFounder(id);

                if (index.IsFailure) return Result.Failure<string>(index.Error);
                
                Products.RemoveAt(index.Value);
                _logger.Trace("Success");
                return Result.Success(id);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return Result.Failure<string>(e.Message);
            }
        }

        private Result<int> ProductIndexFounder(string id)
        {
            _logger.Trace($"Searching product by id [{id}]");
            var allPtoducts = Products;
            var wanted = allPtoducts.Where(p => p.Id.Equals(id)).ToList();

            if (!wanted.Any())
            {
                _logger.Error($"Not found product by id [{id}]");
                return Result.Failure<int>($"Not found product by id [{id}]");
            }

            if (wanted.Count() > 1)
            {
                _logger.Error($"Found too many [{wanted.Count()}] results with id [{id}]");
                return Result.Failure<int>($"Found too many [{wanted.Count()}] results with id [{id}]");
            }

            return allPtoducts.IndexOf(wanted.First());
        }
    }
}
