using AutoMapper;
using Inventory.Models;
using Inventory.Helper.Paging;
using Inventory.ViewModel.Customers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.CustomerServices
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(CustomerViewModel customerViewModel)
        {
            bool isExist = _context.Customers
                .Any(c => c.CustomerName == customerViewModel.CustomerName);
            if (isExist)
            {
                throw new InvalidOperationException("Customer with the same name already exists.");
            }
            var customerTypeId = _context.CustomerTypes
                .Where(c => c.CustomerTypeName.ToLower() == customerViewModel.CustomerTypeName.ToLower())
                .Select(c => c.CustomerTypeId)
                .FirstOrDefault();
            var customer = new Customer
            {
                CustomerName = customerViewModel.CustomerName,
                Address = customerViewModel.Address,
                City = customerViewModel.City,
                State = customerViewModel.State,
                ZipCode = customerViewModel.ZipCode,
                Phone = customerViewModel.Phone,
                Email = customerViewModel.Email,
                CustomerTypeId = customerTypeId,
                ContactPerson = customerViewModel.ContactPerson
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

        }

        public void Delete(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer != null)
            {
                try
                {
                    _context.Customers.Remove(customer);
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

        public Task<PaginatedList<CustomerViewModel>> GetAllAsync(int PageSize, int PageNumber , string? searchTerm)
        {
            var query = _context.Customers.AsNoTracking()
                .Include(c=>c.CustomerType)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    CustomerTypeName = c.CustomerType != null ? c.CustomerType.CustomerTypeName : "",
                    ContactPerson = c.ContactPerson,
                });
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.CustomerName.ToLower().Contains(searchTerm.ToLower()));
            }
            return PaginatedList<CustomerViewModel>.CreateAsync(query, PageNumber, PageSize);
        }

        public async Task<CustomerViewModel> GetByIdAsync(int id)
        {
            var customer =await _context.Customers
                .Where(c => c.CustomerId == id)
                .Include(c => c.CustomerType)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    ZipCode = c.ZipCode,
                    Phone = c.Phone,
                    Email = c.Email,
                    CustomerTypeName = c.CustomerType != null ? c.CustomerType.CustomerTypeName : "",
                    ContactPerson = c.ContactPerson
                })
                .FirstOrDefaultAsync();
            return customer;
        }

        public Task<List<string>> GetForDropdownAsync()
        {
            return _context.Customers
                .Select(c => c.CustomerName)
                .ToListAsync();
        }

        public void Update(CustomerViewModel customerViewModel)
        {
            var customer = _context.Customers.Find(customerViewModel.CustomerId);
            var customerTypeId = _context.CustomerTypes
               .Where(c => c.CustomerTypeName == customerViewModel.CustomerTypeName)
               .Select(c => c.CustomerTypeId)
               .FirstOrDefault();

            if (customer != null)
            {
                customer.CustomerName = customerViewModel.CustomerName;
                customer.Address = customerViewModel.Address;
                customer.City = customerViewModel.City;
                customer.State = customerViewModel.State;
                customer.ZipCode = customerViewModel.ZipCode;
                customer.Phone = customerViewModel.Phone;
                customer.Email = customerViewModel.Email;
                customer.CustomerTypeId = customerTypeId;
                customer.ContactPerson = customerViewModel.ContactPerson;
                _context.SaveChanges();
            }
        }
    }
}