using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBotLuisQna.Table;
using CoreBotLuisQna.Table.Luis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebBotLuisQna.Controllers
{
    [Authorize]
    public class QnAController : Controller
    {

        private ITableWrapper _tableWrapper;
        public QnAController(ITableWrapper tableWrapper)
        {
            _tableWrapper = tableWrapper;
        }

        private async Task<LuisEntity> SetLuisEntity(string id)
        {
            var result  = await _tableWrapper.Get<LuisEntity>(LuisEntity.STR_PARTITION_KEY, id);
            ViewBag.LuisEntity = result;

            return result;
        }

        public async Task<IActionResult> Index(string id)
        {
            

            var resut = await _tableWrapper.List<QnAKBEntity>(id);

            await SetLuisEntity(id);
            return View(resut);
        }

        public async Task<ActionResult> Create(string id)
        {
            await SetLuisEntity(id);
            return View(new QnAKBEntity(id, String.Empty));
        }
        [HttpPost]
        public async Task<ActionResult> Create(QnAKBEntity entity)
        {
            entity.RowKey = entity.QnAKBID;
            var result = await _tableWrapper.Insert<QnAKBEntity>(entity);

            return RedirectToAction("Index", new { id = entity.PartitionKey });
        }

        public async Task<ActionResult> Edit(string id, string luisID)
        {
            
            var result = await _tableWrapper.Get<QnAKBEntity>(luisID, id);
            await SetLuisEntity(luisID);
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(QnAKBEntity entity)
        {
            entity.RowKey = entity.QnAKBID;

            await _tableWrapper.Delete<QnAKBEntity>(entity);
            await _tableWrapper.Insert<QnAKBEntity>(entity);

            return RedirectToAction("Index", new { id = entity.PartitionKey });
        }

        public async Task<ActionResult> Delete(string id, string luisID)
        {
            var result = await _tableWrapper.Get<QnAKBEntity>(luisID, id);
            await SetLuisEntity(luisID);
            return View(result);
        }

        public async Task<ActionResult> DeleteConfirm(string id, string luisID)
        {
            var result = await _tableWrapper.Get<QnAKBEntity>(luisID, id);

            await _tableWrapper.Delete(result);

            return RedirectToAction("Index", new { id = luisID });

        }
    }
}