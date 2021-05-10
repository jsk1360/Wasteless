using System;

namespace Wasteless.Models
{
    public class Menu
    {
        public int? MenuSID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DateId { get; set; }
        public int LocationId { get; set; }
        public double? TotalWasteKgAvg { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }

    public class MenuOption
    {
        public string Name { get; set; }
        public int LocationId { get; set; }
    }
}