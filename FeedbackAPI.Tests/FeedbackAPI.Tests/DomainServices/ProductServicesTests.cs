using FeedbackAPI.Domain;
using FeedbackAPI.DomainServices;
using FeedbackAPI.Infrastructure;
using FeedbackAPI.Settings;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Xunit;

namespace FeedbackAPI.Tests.DomainServices
{
    public class ProductServicesTests
    {
        static readonly IOptions<AppSettings> options = Options.Create<AppSettings>(new AppSettings() { CacheSeconds = 10 });

        readonly List<Product> _products = CreateProducts();

        [Fact]
        [Trait("Common", "GetProducts")]
        public void GetProductsTestPositive()
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            var sut = services.GetProducts();

            sut.Value.Should().BeEmpty();
            sut.Value.Should().NotBeNull();
        }

        [Fact]
        [Trait("Common", "AddProducts")]
        public void AddProductsTestPositive()
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            var sut = services.AddProduct(_products);

            sut.Value.Should().NotBeNullOrEmpty();
            sut.Value.Count.Should().Be(_products.Count);
        }
        [Theory]
        [Trait("Common", "GetProducts")]
        [InlineData("001")]
        [InlineData("002")]
        [InlineData("003")]
        public void GetByIdTestPositive(string id)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            services.AddProduct(_products);
            var searchModel = new ProductSearchModel() { Id = id };
            var sut = services.GetProducts(searchModel);

            sut.Value.Should().NotBeNullOrEmpty();
            sut.Value.Count.Should().Be(1);
            sut.Value.First().Id.Should().Be(_products.First(p => p.Id == id).Id);
        }
        [Theory]
        [Trait("Common", "GetProducts")]
        [InlineData("BMW", 1)]
        [InlineData("Volkswagen", 1)]
        [InlineData("Toyota", 2)]
        public void GetByBrandTestPositive(string brand, int rez)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            services.AddProduct(_products);
            var searchModel = new ProductSearchModel() { Brand = brand };
            var sut = services.GetProducts(searchModel);

            sut.Value.Should().NotBeNullOrEmpty();
            sut.Value.Count.Should().Be(rez);
            sut.Value.First().Id.Should().Be(_products.First(p => p.Brand == brand).Id);
        }
        [Theory]
        [Trait("Common", "GetProducts")]
        [InlineData("335")]
        [InlineData("Golf")]
        [InlineData("Previa")]
        public void GetByProductNameTestPositive(string name)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            services.AddProduct(_products);

            var searchModel = new ProductSearchModel() { ProductName = name };
            var sut = services.GetProducts(searchModel);

            sut.Value.Should().NotBeNullOrEmpty();
            sut.Value.Count.Should().Be(1);
            sut.Value.First().Id.Should().Be(_products.First(p => p.ProductName == name).Id);
        }
        [Theory]
        [Trait("Common", "GetProducts")]
        [InlineData(2, 5, 3)]
        public void GetByRatingTestPositive(decimal ratingmin, decimal ratingmax, int result)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            services.AddProduct(_products);

            var searchModel = new ProductSearchModel() { RatingMin = ratingmin, RatingMax = ratingmax };
            var sut = services.GetProducts(searchModel);

            sut.Value.Count.Should().Be(result);
        }
        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("001", 3.5)]
        [InlineData("002", 2.5)]
        [InlineData("003", 3)]
        [InlineData("004", 0)]
        public void RatingTestPositive(string id, decimal rating)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);

            services.AddProduct(_products);

            var searchModel = new ProductSearchModel() { Id = id };
            var sut = services.GetProducts(searchModel);

            sut.Value.First().Rating.Should().Be(rating);
        }

        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("001","999")]
        [InlineData("002","999")]
        [InlineData("003","999")]
        public void UpdateProductTestPositive(string id, string productName)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);
            services.AddProduct(_products);
            var product = new Product()
            {
                Id = id,
                Brand = "BMW",
                ProductName = productName,
                FeedBacks = new Dictionary<string, int>()
                {
                    {"good", 3},
                    {"very good", 4}
                }
            };
            var sut = services.UpdateProduct(id,product);

            sut.IsSuccess.Should().Be(true);
            sut.Value.ProductName.Should().Be(productName);
        }
        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("005", "999")]
        public void UpdateProductTestNegative(string id, string productName)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);
            services.AddProduct(_products);
            var product = new Product()
            {
                Id = id,
                Brand = "BMW",
                ProductName = productName,
                FeedBacks = new Dictionary<string, int>()
                {
                    {"good", 3},
                    {"very good", 4}
                }
            };
            var sut = services.UpdateProduct(id, product);

            sut.Should().NotBeNull();
            sut.IsSuccess.Should().Be(false);
            sut.Error.Should().NotBeNullOrWhiteSpace();
        }
        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("001", "999",10)]
       
        public void AddFeedbackTestPositive(string id, string key, int value)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);
            services.AddProduct(_products);
           
            var sut = services.AddFeedback(id, new KeyValuePair<string,int> (key,value));
            
            sut.Should().NotBeNull();
            sut.IsSuccess.Should().Be(true);
            sut.Value.FeedBacks.Last().Value.Should().Be(value);
            sut.Value.FeedBacks.Last().Key.Should().Be(key);
        }
        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("001")]
        [InlineData("002")]
        [InlineData("003")]
        [InlineData("004")]
        public void DeleteproductTestPositive(string id)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);
            services.AddProduct(_products);

            var sut = services.DeleteProduct(id);

            sut.Should().NotBeNull();
            sut.IsSuccess.Should().Be(true);
            sut.Value.Count().Should().Be(_products.Count-1);
        }
        [Theory]
        [Trait("Common", "RatingCount")]
        [InlineData("005")]
        
        public void DeleteproductTestNegative(string id)
        {
            var cache = new MemoryCache(new Mock<MemoryCacheOptions>().Object);
            var cacheManager = new CacheManager(cache, options);
            var services = new ProductServices(cacheManager);
            services.AddProduct(_products);

            var sut = services.DeleteProduct(id);

            sut.Should().NotBeNull();
            sut.IsSuccess.Should().Be(false);
            sut.Error.Should().NotBeNullOrWhiteSpace();
        }
        private static List<Product> CreateProducts()
        {
            var product = new Product()
            {
                Id = "001",
                Brand = "BMW",
                ProductName = "335",
                FeedBacks = new Dictionary<string, int>()
                {
                    {"good", 3},
                    {"very good", 4}
                }
            };
            var product1 = new Product()
            {
                Id = "002",
                Brand = "Volkswagen",
                ProductName = "Golf",
                FeedBacks = new Dictionary<string, int>()
                {
                    {"good", 3},
                    {"very bad", 2}
                }
            };
            var product2 = new Product()
            {
                Id = "003",
                Brand = "Toyota",
                ProductName = "Previa",
                FeedBacks = new Dictionary<string, int>()
                {
                    {"very good", 4},
                    {"very bad", 2}
                }
            };
            var product3 = new Product()
            {
                Id = "004",
                Brand = "Toyota",
                ProductName = "Prius",
                FeedBacks = new Dictionary<string, int>()
                {
                }
            };

            return new List<Product>() { product, product1, product2, product3 };
        }
    }
}
