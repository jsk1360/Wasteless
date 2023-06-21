using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RepoDb;
using Wasteless.Dtos;
using Wasteless.Helpers;
using Wasteless.Models;

namespace Wasteless.Data
{
    public class WasteService
    {
        private readonly string _connectionString;

        public WasteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Wasteless");
        }

        public async Task<IEnumerable<WasteDto>> GetWeekWaste(DateTime date, int locationId)
        {
            var monday = date.StartOfWeek(DayOfWeek.Monday);
            var friday = monday.AddDays(4);
            var week = monday.DatesTo(friday).ToArray();
            var weekSids = week.Select(x => Convert.ToInt32(x.ToString("yyyyMMdd"))).ToArray();

            await using var connection = new SqlConnection(_connectionString);

            var waste = (await connection.QueryAsync<Waste>(d =>
                weekSids.Contains(d.DateId) && d.LocationId == locationId)).ToList();

            var location = (await connection.QueryAsync<Location>(locationId)).First();
            var menus = (await connection.QueryAsync<FactMenuItem>(d =>
                weekSids.Contains(d.DateId) && d.LocationId == locationId)).ToList();

            var wastes = new List<WasteDto>();
            foreach (var dateSid in weekSids)
            {
                var menu = menus.Where(x =>
                    x.LocationId == locationId && x.DateId == dateSid).ToList();
                
                foreach (var menuItem in menu)
                {
                    var menuItemName = await connection.QueryAsync<MenuItem>(i => i.Id == menuItem.MenuItemId);
                    menuItem.Name = menuItemName.FirstOrDefault()?.Name ?? "";
                }

                // var limit = GetWasteLimit(menu);

                var wasteDbDto = waste.Where(w => w.DateId == dateSid).ToList();

                if (!wasteDbDto.Any()) continue;
                
                var wasteDto = new WasteDto(location.Name, location.City, menu, wasteDbDto);
                wastes.Add(wasteDto);
            }

            return wastes;
        }

        public async Task<WasteDto?> GetWaste(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var wastes = (await connection.QueryAsync<Waste>(id)).ToList();

            var waste = wastes.First();
            var date = waste.DateId.ToDate();
            if (date == null) return null;

            // var limit = GetWasteLimit(menu);
            var wasteDto = await WasteToDto(wastes, null);

            return wasteDto;
        }

        // private static WasteLimit? GetWasteLimit(MenuItem? menu)
        // {
        //     return menu?.TotalWasteKgAvg != null
        //         ? new WasteLimit {Limit = menu.TotalWasteKgAvg.Value * 0.15, LocationId = menu.LocationId}
        //         : null;
        // }

        public async Task<IEnumerable<LocationDto>> GetLocations()
        {
            await using var connection = new SqlConnection(_connectionString);
            var locations =
                (await connection.QueryAllAsync<Location>())
                .Select(x => new LocationDto(x)).ToArray();
            return locations;
        }

        // public async Task<IEnumerable<MenuDto>> GetMenus(int locationId)
        // {
        //     await using var connection = new SqlConnection(_connectionString);
        //
        //     var menus =
        //         (await connection.QueryAllAsync<MenuItem>())
        //         .Select(x => x.ToDto())
        //         .Where(x => x.LocationId == locationId)
        //         .Distinct();
        //
        //     return menus;
        // }

        public async Task<WasteDto> UpdateOrCreateWaste(WasteForm form)
        {
            var dateId = Convert.ToInt32(form.Date.ToString("yyyyMMdd"));

            await using var connection = new SqlConnection(_connectionString);
            var existingWaste = (await connection.QueryAsync<Waste>(d =>
                d.DateId == dateId && d.LocationId == form.LocationId)).ToList();
            
            foreach (var waste in existingWaste)
            {
                var menuItemWaste = form.MenuItemWaste?.FirstOrDefault(x => x.Item.Id == waste.MenuItemId);
                await connection.UpdateAsync(ClassMappedNameCache.Get<Waste>(), new
                {
                    WasteSID = waste.WasteSID,
                    ModifiedTime = DateTime.UtcNow,
                    form.MealTotal,
                    MealCount = form.MealTotal - form.SpecialMealCount.GetValueOrDefault(0),
                    form.SpecialMealCount,
                    LineWasteKg = menuItemWaste?.LineWasteKg,
                    form.MealCountReserved,
                    form.Comment
                });
            }

            var updatedWaste = (await connection.QueryAsync<Waste>(d =>
                d.DateId == dateId && d.LocationId == form.LocationId)).ToList();

            return await WasteToDto(updatedWaste, null);
        }

        private async Task<WasteDto> WasteToDto(IReadOnlyCollection<Waste> wastes, WasteLimit? limit = null)
        {
            var waste = wastes.First();
            await using var connection = new SqlConnection(_connectionString);
            var location = (await connection.QueryAsync<Location>(waste.LocationId)).First();
            var menu = (await connection.QueryAsync<FactMenuItem>(d =>
                d.DateId == waste.DateId && d.LocationId == waste.LocationId)).ToList();

            var wasteDto = new WasteDto(location.Name, location.City, menu, wastes, limit);

            return wasteDto;
        }
    }
}