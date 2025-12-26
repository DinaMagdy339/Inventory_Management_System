using Inventory.Models;
using Inventory.Utility.HelperClass;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Inventory.Repository;
using Microsoft.EntityFrameworkCore;


//namespace Inventory.Utility
//{
//    public class DbInitializer : IDbInitializer
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private SuperAdmin _superAdmin;
//        private ApplicationDbContext _context;
//        private IRoleInventory _roleInventory;

//        public DbInitializer(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<SuperAdmin> superAdmin,
//            ApplicationDbContext context, IRoleInventory roleInventory)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _superAdmin = superAdmin.Value;
//            _context = context;
//            _roleInventory = roleInventory;
//        }

//        public async Task CreateSuperAdmin()
//        {
//            AppUser appUser = new AppUser
//            {
//                UserName = _superAdmin.Email,
//                Email = _superAdmin.Email,
//                EmailConfirmed = true
//            };

//            var result = await _userManager.CreateAsync(appUser, _superAdmin.Password);
//            if (!result.Succeeded) return;

//            UserProfile userProfile = new UserProfile
//            {
//                FirstName = "Admin",
//                LastName = "Admin",
//                Email = appUser.Email,
//                OldPassword = string.Empty,
//                AppUserId = appUser.Id,
//                UserType = UserType.SuperAdmin,
//                IsApproved = true
//            };
//            await _context.UserProfiles.AddAsync(userProfile);
//            await _context.SaveChangesAsync();

//            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
//            {
//                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
//            }

//            if (!await _userManager.IsInRoleAsync(appUser, "SuperAdmin"))
//            {
//                await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
//            }
//        }
//        public async Task AssignAllRolesToSuperAdmin()
//        {
//            var superAdmin = await _userManager.FindByEmailAsync(_superAdmin.Email);

//            if (superAdmin == null)
//                return;

//            var allRoles = await _roleManager.Roles
//                                             .Select(r => r.Name)
//                                             .ToListAsync();

//            foreach (var role in allRoles)
//            {
//                if (!await _userManager.IsInRoleAsync(superAdmin, role))
//                {
//                    await _userManager.AddToRoleAsync(superAdmin, role);
//                }
//            }
//        }

//    }
//}
