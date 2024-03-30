using Microsoft.AspNetCore.Mvc.Rendering;

namespace Indotalent.Infrastructures.TimeZones
{
    public interface ITimeZoneService
    {
        ICollection<SelectListItem> GetAllTimeZones();
    }
}
