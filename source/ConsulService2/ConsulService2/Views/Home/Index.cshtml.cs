using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace ConsulService2.Views.Home
{
    public class IndexModel : PageModel
    {
        public string Prediction { get; set; }

        public void OnGetMyButton()
        {
            // Обработка нажатия кнопки "Получить предсказание"
        }

    }
}
