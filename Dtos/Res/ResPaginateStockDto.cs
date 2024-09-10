using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Res
{
    public class ResPaginateStockDto<T>
    {
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public List<T>? Data { get; set; }
    }
}