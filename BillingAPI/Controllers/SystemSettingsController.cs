using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Settings;

namespace BillingAPI.Controllers
{
    //public class SystemSettingsController : Controller
    //{
    //    private readonly Lazy<ISettingsManager> _manager = new Lazy<ISettingsManager>(IocContainer.Get<ISettingsManager>);
    //    private ISettingsManager Manager => _manager.Value;

    //    public IActionResult Index()
    //    {
    //        var settings = Manager.GetAll<SystemSettings>();
    //        return View(settings);
    //    }

    //    public IActionResult Edit(int id)
    //    {
    //        if (id != 0)
    //        {
    //            var model = Manager.Get<SystemSettings>(s => s.Id == id);
    //            if (model == null)
    //                return new JsonResult("systemsetting not found");
    //            return View(model);
    //        }
    //        return View(new SystemSettings());
    //    }

    //    [HttpPost]
    //    public IActionResult Edit(SystemSettings setting)
    //    {
    //        try
    //        {
    //            var manager = new SettingsManager();
    //            manager.AddOrUpdate(setting);
    //        }
    //        catch (Exception e)
    //        {
    //            return new JsonResult($"Ошибка сохранения: {e.ToString()}");
    //        }
    //        return RedirectToAction("Index");
    //    }

    //    public IActionResult Delete(int id)
    //    {
    //        try
    //        {
    //            var manager = new SettingsManager();
    //            var toDelte = manager.Delete(id);
    //        }
    //        catch (Exception e)
    //        {
    //            return new JsonResult($"Ошибка удаления: {e.ToString()}");
    //        }
    //        return RedirectToAction("Index");
    //    }
    //}
}