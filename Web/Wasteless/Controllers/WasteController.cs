using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wasteless.Data;
using Wasteless.Dtos;

namespace Wasteless.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class WasteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WasteController> _logger;
        private readonly UserService _userService;
        private readonly WasteService _wasteService;

        public WasteController(ILogger<WasteController> logger, WasteService wasteService, UserService userService,
            IConfiguration configuration)
        {
            _logger = logger;
            _wasteService = wasteService;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("locations")]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            if (_userService.IsAdmin)
                return (await _wasteService.GetLocations()).ToList();

            var locationId = _userService.LocationId;

            if (locationId == null)
                return Forbid();

            return (await _wasteService.GetLocations()).Where(x => x.Id == locationId).ToList();
        }

        [HttpGet]
        public async Task<ActionResult<List<WasteDto>>> GetWaste(DateTime date, int location)
        {
            if (_userService.IsAdmin || location == _userService.LocationId)
                return (await _wasteService.GetWeekWaste(date, location)).ToList();

            return Forbid();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WasteDto>> GetWasteItem(int id)
        {
            var user = await _userService.GetUser();

            if (user == null) return Forbid();

            var waste = await _wasteService.GetWaste(id);
            if (waste == null) return NotFound();

            if (_userService.IsAdmin || waste.LocationId == _userService.LocationId) return waste;

            return Forbid();
        }

        [HttpPost]
        public async Task<ActionResult<WasteDto>> SaveWaste(WasteForm form)
        {
            if (!_userService.IsAdmin && form.LocationId != _userService.LocationId) return Forbid();

            var waste = await _wasteService.UpdateOrCreateWaste(form);
            return CreatedAtAction(nameof(GetWasteItem), new {id = waste.Id}, waste);
        }

        [HttpGet("report")]
        public ActionResult<string> GetReportUrl()
        {
            var reportUrl = _configuration["ReportUrl"];

            if (string.IsNullOrWhiteSpace(reportUrl)) return Problem("Raportin polkua ei ole asetettu");

            return reportUrl;
        }
    }
}