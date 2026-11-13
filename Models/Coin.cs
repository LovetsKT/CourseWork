using Couresework2.CoinCollectionApp.Models.CoinCollectionApp.Models;

namespace Couresework2.CoinCollectionApp.Models
{
    namespace CoinCollectionApp.Models
    {
        public enum StorageType
        {
            Album,
            Capsule,
            Holder,
            Safe,
            None
        }
    }

    public class Coin
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public decimal FaceValue { get; set; }
        public string Currency { get; set; }
        public int Year { get; set; }
        public string MintMark { get; set; }
        public Guid CountryId { get; set; }
        public Guid MetalId { get; set; }
        public Guid ConditionId { get; set; }

        public double? Weight { get; set; }
        public double? Diameter { get; set; }
        public string EdgeType { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal MarketValue { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Note { get; set; }

        //подумаю над этим полем
        //public CoinStatus Status { get; set; } = CoinStatus.InCollection;
        public StorageType Storage { get; set; } = StorageType.None;

        public List<string> ImagePaths { get; set; } = new List<string>();
    }
}
