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
            var menus = (await connection.QueryAsync<Menu>(d =>
                weekSids.Contains(d.DateId) && d.LocationId == locationId)).ToList();

            var wastes = new List<WasteDto>();
            foreach (var dateSid in weekSids)
            {
                var menu = menus.FirstOrDefault(x =>
                    x.LocationId == locationId && x.DateId == dateSid);

                var limit = GetWasteLimit(menu);

                var wasteDbDto = waste.FirstOrDefault(w => w.DateId == dateSid) ??
                                 new Waste
                                 {
                                     DateId = dateSid,
                                     LocationId = locationId
                                 };

                var wasteDto = new WasteDto(location.Name, location.City, menu, wasteDbDto, limit);

                wastes.Add(wasteDto);
            }

            return wastes;
        }

        public async Task<WasteDto?> GetWaste(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            var waste = (await connection.QueryAsync<Waste>(id)).FirstOrDefault();

            if (waste == null) return null;

            var date = waste.DateId.ToDate();
            if (date == null) return null;

            var dateSid = Convert.ToInt32(date.Value.ToString("yyyyMMdd"));

            var menu = (await connection.QueryAsync<Menu>(d =>
                d.DateId == dateSid && d.LocationId == waste.LocationId)).FirstOrDefault();

            var limit = GetWasteLimit(menu);
            var wasteDto = await WasteToDto(waste, limit);

            return wasteDto;
        }

        private static WasteLimit? GetWasteLimit(Menu? menu)
        {
            return menu?.TotalWasteKgAvg != null
                ? new WasteLimit {Limit = menu.TotalWasteKgAvg.Value * 0.15, LocationId = menu.LocationId}
                : null;
        }

        public async Task<IEnumerable<LocationDto>> GetLocations()
        {
            await using var connection = new SqlConnection(_connectionString);
            var locations =
                (await connection.QueryAllAsync<Location>())
                .Select(x => new LocationDto(x)).ToArray();
            return locations;
        }

        public async Task<IEnumerable<MenuDto>> GetMenus(int locationId)
        {
            await using var connection = new SqlConnection(_connectionString);

            var menus =
                (await connection.QueryAllAsync<Menu>())
                .Select(x => x.ToDto())
                .Where(x => x.LocationId == locationId)
                .Distinct();

            return menus;
        }

        public async Task<WasteDto> UpdateOrCreateWaste(WasteForm form)
        {
            var startOfTheWeek = form.Date.StartOfWeek(DayOfWeek.Monday);

            await using var connection = new SqlConnection(_connectionString);
            var existingWaste = (await connection.QueryAsync<Waste>(form.Id)).FirstOrDefault();
            if (existingWaste != null)
            {
                var existingMenu = (await connection.QueryAsync<Menu>(menu =>
                        menu.DateId == existingWaste.DateId && menu.LocationId == existingWaste.LocationId))
                    .FirstOrDefault();

                if (existingMenu == null)
                {
                    existingMenu = new Menu
                    {
                        MenuSID = null,
                        Name = form.Menu,
                        DateId = Convert.ToInt32(form.Date.ToString("yyyyMMdd")),
                        LocationId = form.LocationId,
                        ModifiedTime = DateTime.Now
                    };
                    await connection.InsertAsync(existingMenu);
                }
                else if (!string.Equals(existingMenu.Name, form.Menu, StringComparison.CurrentCultureIgnoreCase))
                {
                    existingMenu.Name = form.Menu;
                    existingMenu.ModifiedTime = DateTime.Now;
                    await connection.UpdateAsync(existingMenu);
                }

                await connection.UpdateAsync(ClassMappedNameCache.Get<Waste>(), new
                {
                    WasteSID = form.Id,
                    ModifiedTime = DateTime.Now,
                    form.MealTotal,
                    MealCount = form.MealTotal - form.SpecialMealCount.GetValueOrDefault(0),
                    form.SpecialMealCount,
                    form.LineWasteKg,
                    form.ProductionWasteKg,
                    form.PlateWasteKg,
                    form.MealCountReserved,
                    form.Comment
                });

                return await WasteToDto(existingWaste, GetWasteLimit(existingMenu));
            }

            var id = await connection.InsertAsync(ClassMappedNameCache.Get<Waste>(), new
            {
                DateSID = Convert.ToInt32(form.Date.ToString("yyyyMMdd")),
                LocationSID = form.LocationId,
                form.MealTotal,
                MealCount = form.MealTotal - form.SpecialMealCount.GetValueOrDefault(0),
                form.SpecialMealCount,
                form.MealCountReserved,
                ModifiedTime = DateTime.Now,
                form.LineWasteKg,
                form.PlateWasteKg,
                form.ProductionWasteKg,
                form.Comment
            });

            var newMenu = new Menu
            {
                MenuSID = 0,
                Name = form.Menu,
                LocationId = form.LocationId,
                DateId = Convert.ToInt32(form.Date.ToString("yyyyMMdd")),
                ModifiedTime = DateTime.Now
            };

            await connection.MergeAsync(newMenu, m => new {m.LocationId, m.DateId});

            var newWaste = (await connection.QueryAsync<Waste>(id)).First();

            return await WasteToDto(newWaste, GetWasteLimit(newMenu));
        }

        private async Task<WasteDto> WasteToDto(Waste waste, WasteLimit? limit = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            var location = (await connection.QueryAsync<Location>(waste.LocationId)).First();
            var menu = (await connection.QueryAsync<Menu>(d =>
                d.DateId == waste.DateId && d.LocationId == waste.LocationId)).FirstOrDefault();

            var wasteDto = new WasteDto(location.Name, location.City, menu, waste, limit);

            return wasteDto;
        }
    }
}