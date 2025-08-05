using AutoServiceCenter.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceCenter.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageRoles(int page = 1, string searchTerm = "")
        {
            int pageSize = 10;
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                usersQuery = usersQuery.Where(u => u.Email.ToLower().Contains(searchTerm) || u.UserName.ToLower().Contains(searchTerm));
            }

            var totalItems = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserRoleViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            var model = new UserRoleIndexViewModel
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
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserRoleViewModel
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
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", model.UserId);
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.SelectedRoles?.Except(currentRoles).ToList() ?? new List<string>();
            var rolesToRemove = currentRoles.Except(model.SelectedRoles ?? new List<string>()).ToList();

            if (rolesToAdd.Any() || rolesToRemove.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    _logger.LogError("Failed to add roles to user {UserId}: {Errors}", user.Id, string.Join(", ", addResult.Errors));
                    ModelState.AddModelError(string.Empty, "Failed to add roles.");
                    model.CurrentRoles = currentRoles.ToList();
                    model.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View(model);
                }

                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
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