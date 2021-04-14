using Wasteless.Models;

namespace Wasteless.Dtos
{
    public class LocationDto
    {
        public LocationDto(Location db)
        {
            Id = db.Id;
            Name = db.Name;
            City = db.City;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }
}