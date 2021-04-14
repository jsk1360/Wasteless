using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasteless.Data;
using Wasteless.Dtos;

namespace Wasteless.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly WasteService _wasteService;

        public MenuController(WasteService wasteService, UserService userService)
        {
            _wasteService = wasteService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MenuDto>>> GetMenus(int location)
        {
            if (_userService.IsAdmin || location == _userService.LocationId)
                return (await _wasteService.GetMenus(location)).OrderBy(m => m.Name).ToList();

            return Forbid();
        }
    }
}