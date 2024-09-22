using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopBE.Dtos.Stock
{
    // dữ liệu truyền lên khi cập nhật danh sách hàng hóa
    public class UpdateStockBody
    {
        public UpdateStockRequest ListSKUsUpdate { get; set; }
        public IEnumerable<string>? ListSKUsDelele { get; set; }
    }
}