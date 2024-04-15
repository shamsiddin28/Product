using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.DTOs.Admins;
using Product.Service.Interfaces.Admins;
using Product.Service.Interfaces.Commons;

namespace Product.Web.Areas.Admins
{
    [Route("Admins")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminsController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IIdentityService _identityService;

        public AdminsController(IAdminService adminService, IIdentityService identityService)
        {
            this._adminService = adminService;
            this._identityService = identityService;
        }

        [HttpGet("GetByPhoneNumber/{phoneNumber}")]
        public async Task<IActionResult> GetByPhoneNumberAsync([FromRoute(Name = "phoneNumber")] string phoneNumber)
            => Ok(await this._adminService.GetByPhoneNumberAsync(phoneNumber));

        [HttpGet("GetAllAdmins")]
        public async Task<IActionResult> GetAllAsync()
            => Ok(await this._adminService.GetAllAsync());

        [HttpGet("GetByPropertyName/{propertyName}")]
        public async Task<IActionResult> GetByPropertyAsync([FromRoute(Name = "propertyName")] string propertyName)
            => Ok(await this._adminService.GetAllAsync(propertyName));

        [HttpGet("GetAdminById/{id:long}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._adminService.GetByIdAsync(id));

        [HttpPut("UpdateAdminById/{id}")]
        public async Task<IActionResult> PutAsync([FromRoute(Name = "id")] long id, [FromForm] AdminUpdateDto dto)
            => Ok(await this._adminService.UpdateAsync(id, dto));

        [HttpPut("UpdateImageById/{id}")]
        public async Task<IActionResult> PutVideoAsync([FromRoute(Name = "id")] long id, [FromForm] IFormFile formFile)
            => Ok(await this._adminService.UpdateImageAsync(id, formFile));

        [HttpDelete("DeleteAdminById/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._adminService.DeleteAsync(id));

        [HttpDelete("DeleteImageById/{id}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute(Name = "id")] long id)
            => Ok(await this._adminService.DeleteImageAsync(id));

        [HttpPut("UpdatePasswordById/{id}")]
        public async Task<IActionResult> PutVideoAsync([FromRoute(Name = "id")] long id, [FromForm] PasswordUpdateDto dto)
            => Ok(await this._adminService.UpdatePasswordAsync(id, dto));
    }
}
