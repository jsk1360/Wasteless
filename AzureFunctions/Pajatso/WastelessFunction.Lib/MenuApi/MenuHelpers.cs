using System.Collections.Generic;
using System.Linq;

namespace WastelessFunction.Functions.MenuApi;

public static class MenuHelpers
{
    public static IEnumerable<Menu> ConvertToMenu(this IEnumerable<MenuQueryResult> results)
    {
        var grouped = results.GroupBy(r => new { r.MenuName });

        return grouped.Select(g =>
            new Menu(g.Key.MenuName, g.Select(r => new MenuItem(r.MenuItemId, r.MenuItemName, r.MenuItemType))));
    }
}