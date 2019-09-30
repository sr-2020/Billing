using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Model;
using Microsoft.AspNetCore.Mvc;
using Settings;

namespace BillingAPI.Controllers
{
    public class SystemSettingsController : Controller
    {
        IUnitOfWork _unitOfWork;

        public SystemSettingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var settings = _unitOfWork.BillingContext.SystemSettings.ToList();
            return View(settings);
        }

        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var model = _unitOfWork.BillingContext.SystemSettings.Find(id);
                if (model == null)
                    return new JsonResult("systemsetting not found");
                return View(model);
            }
            return View(new SystemSettings());
        }

        [HttpPost]
        public IActionResult Edit(SystemSettings setting)
        {
            try
            {
                var manager = new SettingsManager(_unitOfWork);
                manager.AddOrUpdate(setting);
            }
            catch (Exception e)
            {
                return new JsonResult($"Ошибка сохранения: {e.ToString()}");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var manager = new SettingsManager(_unitOfWork);
                var toDelte = manager.Delete(id);
            }
            catch (Exception e)
            {
                return new JsonResult($"Ошибка удаления: {e.ToString()}");
            }
            return RedirectToAction("Index");
        }
    }
}