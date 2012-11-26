using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using stockchart_lod.Models;

namespace stockchart_lod.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StockData(string unit, stockchart_lod.Models.Filter filter)
        {
            var result = ChartDataRepository.BoeingStockData();

            if (unit != null)
            {
                result =
                    from s in result
                    group s by new { s.Date.Year, s.Date.Month } into g
                    select new StockDataPoint
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Open = g.Max(s => s.Open),
                        High = g.Max(s => s.High),
                        Low = g.Min(s => s.Low),
                        Close = g.Max(s => s.Close),
                        Volume = g.Sum(s => s.Volume)
                    };
            }

            if (filter.Filters != null)
            {
                var selectFrom = DateTime.Parse(filter.Filters[0].Value.ToString());
                var selectTo = DateTime.Parse(filter.Filters[1].Value.ToString());

                result =
                    from s in result
                    where s.Date >= selectFrom && s.Date < selectTo
                    select s;
            }

            return Json(result);
        }
    }
}
