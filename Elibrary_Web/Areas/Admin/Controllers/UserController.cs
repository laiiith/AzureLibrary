using Elibrary.Data;
using Elibrary.Utility;
using ELibrary.DataAccess.Repository.IRepository;
using ELibrary.Models;
using ELibrary.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Elibrary_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u=>u.UserId == userId).RoleId;
            RoleManagmentVM roleManagmentVM = new()
            {
                ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u=>u.Id == userId),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                RoleList = _db.Roles.Select(i=> new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            roleManagmentVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            return View(roleManagmentVM);
        }
        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u=>u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
            var oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            if(!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u=> u.Id == roleManagmentVM.ApplicationUser.Id);
                if(roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser , oldRole).GetAwaiter().GetResult(); 
                _userManager.AddToRoleAsync(applicationUser , roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult(); 

            }

            return RedirectToAction(nameof(Index));   
        }
        #region API CALLS
       [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(y => y.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var item in objUserList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == item.Id).RoleId;
                item.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (item.Company == null)
                {
                    item.Company = new() { Name = "It's Not A Company !!" };
                }
            }
            return Json(new { data = objUserList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = " Locking/Unlocking Successfully" });
        }


        #endregion

    }
}
