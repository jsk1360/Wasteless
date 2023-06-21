using System;

namespace Wasteless.Models
{
    public class FactMenuItem
    {
        public int MenuSID { get; set; }
        public string Menu { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DateId { get; set; }
        public int LocationId { get; set; }
        public int MenuItemId { get; set; }
        public double? TotalWasteKgAvg { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public int OrigMenuItemSID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class MenuOption
    {
        public string Name { get; set; }
        public int LocationId { get; set; }
    }
}