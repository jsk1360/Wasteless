using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WastelessFunction.Lib;
using WastelessFunction.Lib.Predictions;

namespace WastelessFunction.Functions.MenuApi;

public class MenuService
{
    private readonly string _connectionString;

    public MenuService(IOptions<ConnectionString> connectionStringOptions)
    {
        _connectionString = connectionStringOptions.Value.WasteDb;
    }

    public async Task<IEnumerable<Menu>> GetMenuForDay(string locationId, DateOnly date)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var dateSid = date.ToString("yyyyMMdd");

        var menuResults = await connection.QueryAsync<MenuQueryResult>(
            @" select menu.Menu as MenuName, item.Id as MenuItemId, item.Name as MenuItemName, item.Type as MenuItemType
                from dw.FacMenuItemNew menu
                join dw.DimMenuItemNew item on menu.MenuItemSID = item.Id
             where DateSID = @date and LocationSID = @locationId",
            new { date = dateSid, locationId });

        return menuResults.ConvertToMenu();
    }

    public async Task<IEnumerable<MenuItem>> GetAllMenuItems(string locationId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var menuResults = await connection.QueryAsync<MenuItem>(
            @"select distinct item.Id as Id, Name as Name, Type as Type
              from dw.FacMenuItemNew menu
              join dw.DimMenuItemNew item on menu.MenuItemSID = item.Id
              where LocationSID = @locationId", new { locationid = locationId });

        return menuResults;
    }
}