//using Inventory.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Inventory.Utility
//{
//    public class RoleInventory : IRoleInventory
//    {
//        private RoleManager<IdentityRole> _roleManager;      
//        private UserManager<AppUser> _userManager;

//        public RoleInventory(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
//        {
//            _roleManager = roleManager;
//            _userManager = userManager;
//        }

//        public async Task AddRoleAsync(string appUserId, UserType userType)
//        {
//            var user = await _userManager.FindByIdAsync(appUserId);
//            if (user == null)
//                throw new Exception("User not found");

//            List<string> rolesToAssign = new();

//            switch (userType)
//            {
//                case UserType.SuperAdmin:
//                    rolesToAssign = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
//                    if (!rolesToAssign.Contains("SystemAdmin"))
//                        rolesToAssign.Add("SystemAdmin");
//                    break;

//                case UserType.Employee:
//                    rolesToAssign = new List<string> { "Sale", "Customer" };
//                    break;

//                case UserType.Worker:
//                    rolesToAssign = new List<string> { "ViewOnly" };
//                    break;

//                case UserType.Supplier:
//                    rolesToAssign = new List<string> { "Purchase" };
//                    break;

//                default:
//                    rolesToAssign = new List<string>();
//                    break;
//            }

//            await _userManager.AddToRolesAsync(user, rolesToAssign);
//        }
//        public async Task CreateNewRoleAsync()
//        {
//            Type t = typeof(TopMenu);
//            foreach (Type classObj in t.GetNestedTypes())
//            {
//                foreach (var objField in classObj.GetFields())
//                {
//                    if (objField.Name.Contains("RoleName"))
//                    {
//                        var roleName = (string)objField.GetValue(null);

//                        if (!await _roleManager.RoleExistsAsync(roleName))
//                        {
//                            await _roleManager.CreateAsync(new IdentityRole(roleName));
//                        }
//                    }
//                }
//            }
//        }

//    }
//}
