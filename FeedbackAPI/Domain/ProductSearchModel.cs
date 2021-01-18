namespace FeedbackAPI.Domain
{
    public class ProductSearchModel
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }

        public decimal? RatingMin { get; set; }

        public decimal? RatingMax { get; set; }

        public override string ToString()
        {
            return
                $"Id:[{Id}], ProductName:[{ProductName}], Brand:[{Brand}], RatingMin:[{RatingMin}], RatingMax[{RatingMax}]";
        }
    }

}
