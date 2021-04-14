using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wasteless.Models;

namespace Wasteless.Data
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public int? LocationId
        {
            get
            {
                var locationId = _httpContextAccessor.HttpContext?.User.FindFirst("Location");
                return locationId != null ? Convert.ToInt32(locationId.Value) : null;
            }
        }

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

        public async Task<ApplicationUser?> GetUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
    }
}