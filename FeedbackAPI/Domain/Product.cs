using System.Collections.Generic;

namespace FeedbackAPI.Domain
{
    public class Product
    {
        private decimal _rating;
        
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public decimal Rating
        {
            get
            {
                _rating = 0;
                if (FeedBacks?.Values == null)
                    return 0;
                foreach (var rating in FeedBacks.Values)
                {
                    _rating += rating;
                }

                if (FeedBacks?.Values?.Count > 0)
                {
                    _rating /= FeedBacks.Values.Count;
                }
                return _rating;
            }
        }

        public Dictionary<string, int> FeedBacks { get; set; }
    }
}
