using Microsoft.AspNetCore.Mvc.Rendering;

namespace Indotalent.Infrastructures.Countries
{
    public interface ICountryService
    {
        ICollection<SelectListItem> GetCountries();
    }
}
