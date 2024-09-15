using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Stock
{
    public class GenerateCodeListSKURequest
    {
        public List<string> Colors { get; set; } = new List<string> { };
        public string CodeSKUParent { get; set; } = string.Empty;
        public int Id { get; set; }

    }
}