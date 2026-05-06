using BookStoreAppSpring.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreAppSpring.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly BooksDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(BooksDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> userList = _dbContext.ApplicationUsers.ToList();

            var userRoles = _dbContext.UserRoles.ToList();

            foreach (var user in userList)
            {
                var userRole = userRoles.Find(ur => ur.UserId == user.Id);

                if (userRole != null)
                {
                    user.RoleName = _dbContext.Roles.Find(userRole.RoleId).Name;
                }
            }

            return View(userList);
        }

        public IActionResult LockUnlock(string id)
        {
            var userFromDB = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userFromDB == null)
            {
                return NotFound();
            }
            if (userFromDB.LockoutEnd != null && userFromDB.LockoutEnd > DateTime.Now)
            {
                // User is currently locked, so we will unlock them
                userFromDB.LockoutEnd = DateTime.Now;
            }
            else
            {
                // User is currently unlocked, so we will lock them for 100 years
                userFromDB.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult EditUserRole(string id)
        {
            var currentUserRole = _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == id);

            IEnumerable<SelectListItem> listOfRoles = _dbContext.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            });

            ViewBag.Roles = listOfRoles;

            ViewBag.UserInfo = _dbContext.ApplicationUsers.Find(id);

            return View(currentUserRole);
        }

        [HttpPost]
        public IActionResult EditUserRole(IdentityUserRole<string> updatedRole)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers.Find(updatedRole.UserId);

            string newRoleName = _dbContext.Roles.Find(updatedRole.RoleId).Name;

            string oldRoleId = _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == updatedRole.UserId).RoleId;

            string oldRoleName = _dbContext.Roles.Find(oldRoleId).Name;

            _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();

            return RedirectToAction("Index");
        }
    }
}
