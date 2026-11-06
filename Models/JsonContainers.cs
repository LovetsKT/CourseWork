using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couresework2.CoinCollectionApp.Models
{
    public class DictionaryData
    {
        public List<Country> Countries { get; set; } = new List<Country>();
        public List<Metal> Metals { get; set; } = new List<Metal>();
        public List<Condition> Conditions { get; set; } = new List<Condition>();
    }

    public class CollectionData
    {
        public List<Coin> Coins { get; set; } = new List<Coin>();
    }
}
