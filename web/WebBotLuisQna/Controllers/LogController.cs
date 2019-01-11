using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBotLuisQna.Table;
using CoreBotLuisQna.Table.Luis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBotLuisQna.Table.Luis;

namespace WebBotLuisQna.Controllers
{
    [Authorize]
    public class LogController : Controller
    {

        private ITableWrapper _tableWrapper;
        public LogController(ITableWrapper tableWrapper)
        {
            _tableWrapper = tableWrapper;
        }
        private async Task<LuisEntity> SetLuisEntity(string id)
        {
            var result = await _tableWrapper.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, id);
            ViewBag.LuisEntity = result;

            return result;
        }

        public async Task<IActionResult> Index(string id)
        {
            

            var resut = await _tableWrapper.List<LogEntity>(id);

            await SetLuisEntity(id);
            return View(resut);
        }

        public async Task<ActionResult> Details(string id, string luisID)
        {

            var result = await _tableWrapper.Get<LogEntity>(luisID, id);
            await SetLuisEntity(luisID);
            return View(result);
        }
    }
}