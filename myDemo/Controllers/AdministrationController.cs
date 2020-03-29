using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myDemo.Models;
using myDemo.ViewModels;
using ORMEFCoreDA.Models;

namespace myDemo.Controllers
{
    //[Authorize(Policy = "EditRolePolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext db;


        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager
            , AppDbContext db)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.db = db;
        }

        public IActionResult Index() => View();

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleVM RoleObj)
        {
            if (ModelState.IsValid)
            {
                IdentityRole CreatedRole = new IdentityRole
                {
                    Name = RoleObj.RoleName
                };
                var result = await roleManager.CreateAsync(CreatedRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(RoleObj);
        }

        public IActionResult RolesList() => View(roleManager.Roles);

        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> Edit(string id)
        {
            var res = await roleManager.FindByIdAsync(id);
            if (res == null)
            {
                ViewBag.ErrorMsg = $"Resource with id {id} not found";
                return View("NotFound");
            }

            var Obj = new EditRoleVM
            {
                id = res.Id,
                RoleName = res.Name
            };
            var RoleUsers = db.UserRoles.Where(x => x.RoleId == id).ToList();
            foreach (var user in userManager.Users)
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, res.Name))
                {
                    Obj.RoleUsers.Add(user.UserName);
                }
            }

            return View(Obj);
        }

        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> Edit(EditRoleVM model)
        {
            var role = await roleManager.FindByIdAsync(model.id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string RoleId)
        {
            ViewBag.RoleId = RoleId;
            IdentityRole SelectedRole = await roleManager.FindByIdAsync(RoleId);

            if (SelectedRole == null)
            {
                ViewBag.ErrorMsg = $"Resource with id {RoleId} not found";
                return View("NotFound");
            }
            List<User2RoleVM> MangeUserRoleList = new List<User2RoleVM>();
            foreach (var user in userManager.Users)
            {
                var userRoleVM = new User2RoleVM { UserId = user.Id, UserName = user.UserName };

                _ = (await userManager.IsInRoleAsync(user, SelectedRole.Name)) == true ? userRoleVM.IsSelected = true : userRoleVM.IsSelected = false;

                MangeUserRoleList.Add(userRoleVM);
            }
            return View("ManageUserRoles", MangeUserRoleList);
        }


        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<User2RoleVM> User2RoleList, string RoleId)
        {
            IdentityRole SelectedRole = await roleManager.FindByIdAsync(RoleId);

            if (SelectedRole == null)
            {
                ViewBag.ErrorMsg = $"Resource with id {RoleId} not found";
                return View("NotFound");
            }

            try
            {
                foreach (var _ in User2RoleList)
                {
                    AppUser user = await userManager.FindByIdAsync(_.UserId);
                    if (_.IsSelected && !(await userManager.IsInRoleAsync(user, SelectedRole.Name)))
                    {
                        await userManager.AddToRoleAsync(user, SelectedRole.Name);
                    }
                    else if (!(_.IsSelected) && await userManager.IsInRoleAsync(user, SelectedRole.Name))
                    {
                        await userManager.RemoveFromRoleAsync(user, SelectedRole.Name);
                    }
                }
                return RedirectToAction("Edit", new { id = RoleId });
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        public IActionResult ListUsers() => View(userManager.Users.Where(x => x.IsDeleted == false));

        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserVM
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                city = user.city,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUser(EditUserVM model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.city = model.city;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.IsDeleted = true;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var Role = await roleManager.FindByIdAsync(id);

            if (Role == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await roleManager.DeleteAsync(Role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("RolesList");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            ViewBag.userId = userId;
            AppUser SelectedUser = await userManager.FindByIdAsync(userId);

            if (SelectedUser == null)
            {
                ViewBag.ErrorMsg = $"Resource with id {userId} not found";
                return View("NotFound");
            }
            List<UserRolesVM> UserRoleList = new List<UserRolesVM>();
            foreach (var Role in roleManager.Roles)
            {
                var userRoleVM = new UserRolesVM { RoleId = Role.Id, RoleName = Role.Name };

                _ = (await userManager.IsInRoleAsync(SelectedUser, Role.Name)) == true ? userRoleVM.IsSelected = true : userRoleVM.IsSelected = false;

                UserRoleList.Add(userRoleVM);
            }
            return View(UserRoleList);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(List<UserRolesVM> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }


        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUserClaims(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsVM { UserId = UserId } ;

            // Loop through each claim we have in our application
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim { ClaimType = claim.Type } ;

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);

        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUserClaims(UserClaimsVM model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }
            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected == true ? "true":"false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View(); 
    }
}



