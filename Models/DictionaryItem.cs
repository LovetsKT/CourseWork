using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couresework2.CoinCollectionApp.Models
{
    public class DictionaryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    public class Country : DictionaryItem { }
    public class Metal : DictionaryItem { }

    public class Condition : DictionaryItem
    {
        public string Description { get; set; }
    }
}
