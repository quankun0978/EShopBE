using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Res
{
    public class ResStockDto<T>
    {
        public T? Data { get; set; }
        public List<T>? Atributes { get; set; }
    }
}