namespace WastelessFunction.Functions.MenuApi;

public record MenuQueryResult(string MenuName, int MenuItemId, string MenuItemName,
    MenuItemType MenuItemType);