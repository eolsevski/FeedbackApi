using System.Collections.Generic;
using CSharpFunctionalExtensions;
using FeedbackAPI.Domain;

namespace FeedbackAPI.DomainServices
{
    public interface IProductServices
    {
        public Result<List<Product>> GetProducts();
        Result<List<Product>> GetProducts(ProductSearchModel model);
        public Result<List<Product>> AddProduct(List<Product> product);
        Result<Product> UpdateProduct(string id, Product product);
        public Result<string> DeleteProduct(string id);
        Result<Product> AddFeedback(string id, KeyValuePair<string, int> feedback);
    }
}
