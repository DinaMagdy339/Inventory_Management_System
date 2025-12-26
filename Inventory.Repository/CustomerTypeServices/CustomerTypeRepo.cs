using Inventory.Helper.Paging;
using Inventory.Models;
using Inventory.ViewModel.Customers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.CustomerTypeServices
{
    public class CustomerTypeRepo : ICustomerTypeRepo
    {
        private readonly ApplicationDbContext _context;
        public CustomerTypeRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(CustomerTypeViewModel model)
        {
            bool name = _context.CustomerTypes.Any(c => c.CustomerTypeName.ToLower() == model.CustomerTypeName.ToLower());
            if (name)
            {
                throw new Exception("This Customer Type name already exists.");
            }
            var customerType = new CustomerType
            {
                CustomerTypeName = model.CustomerTypeName,
                Description = model.Description
            };
            _context.CustomerTypes.Add(customerType);
            _context.SaveChanges();

        }

        public void Delete(int id)
        {
            var customerType = _context.CustomerTypes.FirstOrDefault(c => c.CustomerTypeId == id);
            if (customerType != null)
            {
                try
                {

                    _context.CustomerTypes.Remove(customerType);
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

        public async Task<PaginatedList<CustomerTypeViewModel>> GetAllAsync(int pageSize, int pageNumber, string? searchTerm)
        {
            var customerTypes = _context.CustomerTypes.AsNoTracking()
                .Select(ct => new CustomerTypeViewModel
                {
                    CustomerTypeId = ct.CustomerTypeId,
                    CustomerTypeName = ct.CustomerTypeName,
                    Description = ct.Description
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                customerTypes = customerTypes.Where(s => s.CustomerTypeName.ToLower().Contains(searchTerm.ToLower()));
            }
            return await PaginatedList<CustomerTypeViewModel>.CreateAsync(customerTypes, pageNumber, pageSize);
        }
        public async Task<CustomerTypeViewModel> GetByIdAsync(int id)
        {
            var customerType = await _context.CustomerTypes.FirstOrDefaultAsync(c => c.CustomerTypeId == id);
            if (customerType != null)
            {
                return new CustomerTypeViewModel
                {
                    CustomerTypeId = customerType.CustomerTypeId,
                    CustomerTypeName = customerType.CustomerTypeName,
                    Description = customerType.Description
                };
            }
            return null;
        }

        public void Update(CustomerTypeViewModel model)
        {
            var customerType = _context.CustomerTypes.FirstOrDefault(c => c.CustomerTypeId == model.CustomerTypeId);
            if (customerType != null)
            {
                customerType.CustomerTypeName = model.CustomerTypeName;
                customerType.Description = model.Description;
                _context.SaveChanges();
            }
        }

        public async Task<List<string>> GetForDropdownAsync()
        {
            return await _context.CustomerTypes
                .AsNoTracking()
                .OrderBy(ct => ct.CustomerTypeName)
                .Select(ct => ct.CustomerTypeName)
                .ToListAsync() ?? new List<string>();
        }

    }
}
