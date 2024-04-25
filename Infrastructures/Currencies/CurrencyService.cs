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
                string currencyEnglishName = region.CurrencyEnglishName;

                var value = currencySymbol;
                var text = $"{currencyEnglishName} - {currencySymbol}";

                if (!string.IsNullOrEmpty(currencySymbol) && !currencies.Any(c => c.Text == text))
                {
                    currencies.Add(new SelectListItem
                    {
                        Value = value,
                        Text = text
                    });
                }
            }

            return currencies.OrderByDescending(x => x.Text).ToList();
        }
    }
}
