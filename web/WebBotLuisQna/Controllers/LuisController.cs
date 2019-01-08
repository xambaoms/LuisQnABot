using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBotLuisQna.Table;
using CoreBotLuisQna.Table.Luis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace WebBotLuisQna.Controllers
{
    [Authorize]
    public class LuisController : Controller
    {
        private ITableWrapper _tableWrapper;
        public LuisController(ITableWrapper tableWrapper)
        {
            _tableWrapper = tableWrapper;
        }

        public async Task<IActionResult> Index()
        {
          

            var resut = await _tableWrapper.List<LuisEntity>(LuisEntity.STR_PARTITION_KEY);

            return View(resut);
        }

        public ActionResult Create()
        {
            return View(new LuisEntity());
        }
        [HttpPost]
        public async Task<ActionResult> Create(LuisEntity entity)
        {
            entity.PartitionKey = LuisEntity.STR_PARTITION_KEY;
            entity.RowKey = entity.AppId;
            var result = await _tableWrapper.Insert<LuisEntity>(entity);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            var result = await _tableWrapper.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, id);

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(LuisEntity entity)
        {
            entity.PartitionKey = LuisEntity.STR_PARTITION_KEY;
            entity.RowKey = entity.AppId;

            await _tableWrapper.Delete<LuisEntity>(entity);
            await _tableWrapper.Insert<LuisEntity>(entity);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            var result = await _tableWrapper.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, id);

            return View(result);
        }

        public async Task<ActionResult> DeleteConfirm(string id)
        {
            var result = await _tableWrapper.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, id);

            await _tableWrapper.Delete(result);

            return RedirectToAction("Index");
        }
    }

}