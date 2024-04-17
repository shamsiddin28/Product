using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.DTOs.Admins;
using Product.Service.Interfaces.Admins;
using Product.Service.Interfaces.Commons;
using Product.Service.ViewModels.AdminViewModels;

namespace Product.Web.Controllers.Admins
{
    [Authorize(Roles = "superadmin")]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IIdentityService _identityService;

        public AdminsController(IAdminService adminService, IIdentityService identityService)
        {
            this._adminService = adminService;
            this._identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchValue)
        {
            List<AdminViewModel> admins;
            if (!string.IsNullOrEmpty(searchValue))
            {
                ViewBag.AdminSearch = searchValue;
                admins = await _adminService.GetAllAsync(searchValue);
            }
            else
            {
                admins = await _adminService.GetAllAsync();
            }

            return View(admins);
        }

        public async Task<ViewResult> Profile()
        {
            var admin = await _adminService.GetByTokenAsync();
            if (admin is not null)
                return View(admin);
            return View();
        }

        public async Task<ViewResult> Update(long id)
        {
            try
            {
                var admin = await _adminService.GetByIdAsync(id);

                var adminUpdate = new AdminUpdateDto()
                {
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Address = admin.Address,
                    BirthDate = admin.BirthDate,
                    PhoneNumber = admin.PhoneNumber,
                    ImagePath = admin.ImagePath,

                };

                return View("Update", adminUpdate);

            }
            catch (Exception ex)
            {
                TempData["InfoMessage"] = $"{ex.Message}";
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(long id, AdminUpdateDto dto)
        {

            try
            {
                var product = await _adminService.UpdateAsync(id, dto);
                if (product)
                {
                    TempData["SuccessMessage"] = "Admin Updated Successfully !";
                    return RedirectToAction("Profile", "Admins");
                }

                return await Update(id);
            }
            catch (Exception ex)
            {
                TempData["InfoMessage"] = $"{ex.Message}";

                throw new Exception(ex.Message);
            }
        }

        public async Task<ViewResult> Remove(long id)
        {
            try
            {
                var admin = await _adminService.GetByIdAsync(id);
                if (admin is not null)
                {
                    return View("Remove", admin);
                }
                return View("Remove", id);
            }
            catch (Exception ex)
            {
                TempData["InfoMessage"] = $"{ex.Message}";
                throw new Exception(ex.Message);
            }
        }

        [HttpPost, ActionName("Remove")]
        public async Task<IActionResult> RemoveConfirmed(long id)
        {
            try
            {
                var product = await _adminService.DeleteAsync(id);
                if (product)
                {
                    TempData["SuccessMessage"] = "Admin Removed Successfully !";
                    return RedirectToAction("Index", "Admins");
                }
                return View("Remove", id);
            }
            catch (Exception ex)
            {
                TempData["InfoMessage"] = $"{ex.Message}";

                throw new Exception(ex.Message);
            }
        }

        //#region GetAll
        //[HttpGet]
        //public async Task<IActionResult> Index(string search)
        //{
        //    List<AdminViewModel> admins;
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        ViewBag.AdminSearch = search;
        //        admins = await _adminService.GetAllAsync(search);
        //    }
        //    else
        //    {
        //        admins = await _adminService.GetAllAsync();
        //    }

        //    return View(admins);
        //}
        //#endregion

        //#region GetPhoneNumber
        //[HttpGet("phoneNumber")]
        //public async Task<IActionResult> GetByPhoneNumberAsync(string phoneNumber)
        //{
        //    var admin = await _adminService.GetByPhoneNumberAsync(phoneNumber);
        //    ViewBag.HomeTitle = "Profile";
        //    var adminView = new AdminViewModel()
        //    {
        //        Id = admin.Id,
        //        FirstName = admin.FirstName,
        //        LastName = admin.LastName,
        //        ImagePath = admin.ImagePath,
        //        PhoneNumber = admin.PhoneNumber,
        //        BirthDate = admin.BirthDate,
        //        Address = admin.Address,
        //        CreatedAt = admin.CreatedAt
        //    };

        //    return View("Profile", adminView);
        //}
        //#endregion

        //#region Update
        //[HttpGet("update")]
        //public async Task<ViewResult> UpdateAsync(int adminId)
        //{
        //    var admin = await _adminService.GetByIdAsync(adminId);
        //    var adminUpdate = new AdminUpdateDto()
        //    {
        //        ImagePath = admin.ImagePath,
        //        FirstName = admin.FirstName,
        //        LastName = admin.LastName,
        //        PhoneNumber = admin.PhoneNumber,
        //        BirthDate = admin.BirthDate,
        //        Address = admin.Address,
        //    };

        //    return View("../Admins/Update", adminUpdate);
        //}

        //[HttpPost("update")]
        //public async Task<IActionResult> UpdateAsync([FromForm] AdminUpdateDto adminUpdateDto, int adminId)
        //{
        //    var admin = await _adminService.UpdateAsync(adminId, adminUpdateDto);
        //    if (admin) return await UpdateAsync(adminId);
        //    else return await UpdateAsync(adminId);
        //}

        //[HttpPost("updateImage")]
        //public async Task<IActionResult> UpdateImageAsync(int adminId, [FromForm] IFormFile formFile)
        //{
        //    var updateImage = await _adminService.UpdateImageAsync(adminId, formFile);
        //    return await UpdateAsync(adminId);
        //}

        //[HttpPost("passwordUpdate")]
        //public async Task<IActionResult> PasswordUpdateAsync(int id, PasswordUpdateDto dto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _adminService.UpdatePasswordAsync(id, dto);
        //        if (result) return await UpdateAsync(id);
        //        else return await UpdateAsync(id);
        //    }
        //    else return await UpdateAsync(id);
        //}
        //#endregion

        //#region DeleteImage
        //[HttpGet("delete")]
        //public async Task<ViewResult> DeleteAsync(int adminId)
        //{
        //    var admin = await _adminService.GetByIdAsync(adminId);
        //    if (admin != null) return View("Delete", admin);
        //    else return View("admins");
        //}

        //[HttpPost("delete")]
        //public async Task<IActionResult> DeleteAdminAsync(int adminId)
        //{
        //    var admin = await _adminService.DeleteAsync(adminId);
        //    if (admin) return RedirectToAction("index", "admins", new { area = "" });
        //    else return View();
        //}

        //[HttpPost("deleteImage")]
        //public async Task<IActionResult> DeleteImageAsync(int adminId)
        //{
        //    var image = await _adminService.DeleteImageAsync(adminId);
        //    return await UpdateAsync(adminId);
        //}
        //#endregion

    }
}
