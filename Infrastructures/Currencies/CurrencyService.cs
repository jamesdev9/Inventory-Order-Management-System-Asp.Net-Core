using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace Indotalent.Infrastructures.Currencies
{
    public class CurrencyService : ICurrencyService
    {
        public ICollection<SelectListItem> GetCurrencies()
        {
            List<SelectListItem> currencies = new List<SelectListItem>();

            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo region = new RegionInfo(ci.Name);
                string currencySymbol = region.CurrencySymbol;
                string currencyName = region.ISOCurrencySymbol;
                string currencyEnglishName = region.CurrencyEnglishName;

                if (!string.IsNullOrEmpty(currencySymbol) && !currencies.Any(c => c.Value == currencySymbol))
                {
                    currencies.Add(new SelectListItem
                    {
                        Value = currencySymbol,
                        Text = $"{currencyEnglishName} - {currencySymbol}"
                    });
                }
            }

            return currencies.OrderByDescending(x => x.Text).ToList();
        }
    }
}
