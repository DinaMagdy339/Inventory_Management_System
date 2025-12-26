using Inventory.Helper.Paging;
using Inventory.ViewModel.Bills;
using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Repository.BillTypeServices
{
    public class BillTypeRepo : IBillTypeRepo
    {
        private ApplicationDbContext _context;
        public BillTypeRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(CreateBillTypeVM model)
        {
            bool name = _context.BillTypes.Any(b => b.BillTypeName.ToLower() == model.BillTypeName.ToLower());

            if (name)
      
            {
                throw new Exception("This Bill Type name already exists.");
            }

            var billType = new BillType
            {
                BillTypeId = model.BillTypeId,
                BillTypeName = model.BillTypeName,
                Description = model.Description
            };
            _context.BillTypes.Add(billType);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var billType = _context.BillTypes.FirstOrDefault(b => b.BillTypeId == id);
            if (billType != null)
            {
                try
                {

                    _context.BillTypes.Remove(billType);
                    _context.SaveChanges();
                }

                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                    {
                        throw new InvalidOperationException(
                            "لا يمكن الحذف لأنه مرتبط ببيانات أخرى"
                        );
                    }

                    throw;
                }



            }
        }
        public async Task<PaginatedList<AllBillTypeViewModel>> GetAllAsync(int pageSize, int pageNumber,string? searchTerm)
        {
            var billTypes = _context.BillTypes.AsNoTracking()
                .Select(b => new AllBillTypeViewModel
                {
                    BillTypeId = b.BillTypeId,
                    BillTypeName = b.BillTypeName,
                    Description = b.Description
                });
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();

                billTypes = billTypes.Where(x => x.BillTypeName.ToLower().Contains(searchTerm));
            }


            return await PaginatedList<AllBillTypeViewModel>.CreateAsync(billTypes, pageNumber, pageSize);
        }

        public async Task<BillTypeViewModel> GetByIdAsync(int id)
        {
            var billType =await _context.BillTypes.AsNoTracking().FirstOrDefaultAsync(b => b.BillTypeId == id);
            var billTypeVM = new BillTypeViewModel()
            {
                BillTypeId = billType.BillTypeId,
                BillTypeName = billType.BillTypeName,
                Description = billType.Description,
            };
                
            return billTypeVM;
        }

        public void Update(BillTypeViewModel model)
        {

            var billType = _context.BillTypes.Where(b => b.BillTypeId == model.BillTypeId).FirstOrDefault();
            if (billType != null)
            {
                billType.BillTypeName = model.BillTypeName;
                billType.Description = model.Description;
                _context.SaveChanges();

            }

        }

        
    }
}
