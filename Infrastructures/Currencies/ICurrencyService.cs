using Microsoft.AspNetCore.Mvc.Rendering;

namespace Indotalent.Infrastructures.Currencies
{
    public interface ICurrencyService
    {
        ICollection<SelectListItem> GetCurrencies();
    }
}
