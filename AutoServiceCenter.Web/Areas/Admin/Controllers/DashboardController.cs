using AutoServiceCenter.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceCenter.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class DashboardController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageRoles(int page = 1, string searchTerm = "")
        {
            int pageSize = 10;
            IQueryable<IdentityUser> usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                usersQuery = usersQuery.Where(u => u.Email.ToLower().Contains(searchTerm) || u.UserName.ToLower().Contains(searchTerm));
            }

            int totalItems = await usersQuery.CountAsync();
            List<IdentityUser> users = await usersQuery
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<UserRoleViewModel> userViewModels = new List<UserRoleViewModel>();
            foreach (IdentityUser user in users)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            UserRoleIndexViewModel model = new UserRoleIndexViewModel
            {
                Users = userViewModels,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                SearchTerm = searchTerm
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            IdentityUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound();
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            List<string> allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            EditUserRoleViewModel model = new EditUserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRoles = roles.ToList(),
                AvailableRoles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(EditUserRoleViewModel model)
        {
            IdentityUser? user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", model.UserId);
                return NotFound();
            }

            IList<string> currentRoles = await _userManager.GetRolesAsync(user);
            List<string> rolesToAdd = model.SelectedRoles?.Except(currentRoles).ToList() ?? new List<string>();
            List<string> rolesToRemove = currentRoles.Except(model.SelectedRoles ?? new List<string>()).ToList();

            if (rolesToAdd.Any() || rolesToRemove.Any())
            {
                IdentityResult addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    _logger.LogError("Failed to add roles to user {UserId}: {Errors}", user.Id, string.Join(", ", addResult.Errors));
                    ModelState.AddModelError(string.Empty, "Failed to add roles.");
                    model.CurrentRoles = currentRoles.ToList();
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View(model);
                }

                IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError("Failed to remove roles from user {UserId}: {Errors}", user.Id, string.Join(", ", removeResult.Errors));
                    ModelState.AddModelError(string.Empty, "Failed to remove roles.");
                    model.CurrentRoles = currentRoles.ToList();
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View(model);
                }

                _logger.LogInformation("Updated roles for user {UserId}. Added: {AddedRoles}, Removed: {RemovedRoles}", user.Id, string.Join(", ", rolesToAdd), string.Join(", ", rolesToRemove));
            }

            return RedirectToAction("ManageRoles");
        }
    }
}