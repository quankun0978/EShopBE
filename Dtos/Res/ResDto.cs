using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Res
{
    public class ResDto<T>
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}