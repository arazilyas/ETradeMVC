using ETrade.Dto;
using ETrade.Entity.Concretes;
using ETrade.Uw;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ETrade.UI.Controllers
{
    public class BasketMasterController : Controller
    {
        IUow _uow;
        BasketMaster _basketMaster;
        public BasketMasterController(IUow uow, BasketMaster basketMaster)
        {
            _uow = uow;
            _basketMaster = basketMaster;
        }

        public IActionResult Create()
        {
            try
            {
                var user = JsonConvert.DeserializeObject<UserDTO>(HttpContext.Session.GetString("User"));
                var incompletedMaster = _uow._basketMasterRep.Set().FirstOrDefault(x => x.Completed == false && x.EntityId == user.Id);
                if (incompletedMaster != null)
                {
                    return RedirectToAction("Add", "BasketDetail", new { id = incompletedMaster.Id });
                }
                else
                {
                    _basketMaster.OrderDate = DateTime.Now;
                    _basketMaster.EntityId = user.Id;
                    _uow._basketMasterRep.Add(_basketMaster);
                    _uow.Commit();
                    return RedirectToAction("Add", "BasketDetail", new { id = _basketMaster.Id });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Auth");
            }
            
        }
    }
}
