using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Products;
using Inventory.ViewModel.StockBatchs;
using Inventory.ViewModel.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Inventory.ViewModel.Stocks.StockDetailVM;

namespace Inventory.Repository.StockServices
{
    public interface IStockRepo
    {
        Task<PaginatedList<AllStockVM>> GetAllStocksAsync(int pageNum, int pageSize, string? searchTerm);
        Task<StockDetailVM> GetStockByIdAsync(int id);
        void AddStock(CreatedStockVM stock);
        void Update(CreatedStockVM stock);
        void Delete(int id);
        Task<CreatedStockVM> GetStockByStockIdAsync(int stockId);

        Task<List<ProductViewModel>> GetProductsAsync(string search);
        Task<List<StockBatchVM>> GetBatchesAsync(int productId);
        Task<bool> WithdrawMultipleAsync(List<WithdrawItemVM> items, string customerName, int? invoiceId, string remarks);
        string GetExpiryStatus(DateTime expiryDate);

    }
}

