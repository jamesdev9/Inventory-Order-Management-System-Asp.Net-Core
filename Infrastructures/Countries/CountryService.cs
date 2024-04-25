using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Indotalent.Infrastructures.Countries
{
    public class CountryService : ICountryService
    {
        public ICollection<SelectListItem> GetCountries()
        {
            List<SelectListItem> countries = new List<SelectListItem>();

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo region = new RegionInfo(ci.Name);
                string countryName = region.EnglishName;

                if (!countries.Any(c => c.Text == countryName))
                {

                    var countryItem = new SelectListItem
                    {
                        Value = countryName,
                        Text = countryName
                    };

                    countries.Add(countryItem);
                }
            }

            return countries;
        }
    }
}
