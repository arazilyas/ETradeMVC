using ETrade.Entity.Concretes;
using ETrade.UI.Models;
using ETrade.Uw;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ETrade.UI.Controllers
{
    public class AuthController : Controller
    {
        IUow _uow;
        UsersModel _model;
        public AuthController(UsersModel model, IUow uow)
        {
            _model = model;
            _uow = uow;
        }

        public IActionResult Register()
        {
            _model.User = new Users();
            _model.Counties = _uow._countyRep.List();
            return View(_model);
        }
        [HttpPost]
        public IActionResult Register(UsersModel? model)
        {
            model.User = _uow._usersRep.CreateUser(model.User);

            if (model.User.Error == true)
            {
                //_model.Counties = _uow._countyRep.List();
                //_model.Error = "Böyle bir kullanıcı mevcut !!!";

                model.Counties = _uow._countyRep.List();
                model.Error = $" {model.User.Mail} "+" Kullanıcısı mevcut!";
                return View(model);
                //return RedirectToAction("Error", "Home", new {Msg = $" {model.User.Mail}Kullanıcısı Mevcut !!"}); // Bu şekilde hata yeni pencerede görürüz. Yukarıdakş şekilde ise ekleme ekranında hata alırız.           
            }
            else
            {
                model.User.Role = "User";
                _uow._usersRep.Add(model.User);
                _uow.Commit();
                return RedirectToAction("Success", "Home", new { Msg = $"{model.User.Mail} adlı kullanıcı başarıyla kayıt edildi." }); // Sadece index yazarsam bu sayfada index arar. Başka bir controllerdaki indexe gitmek istersem virgülden sonra gideceğim kontrollerin adını yazarız.
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Mail, string Password)
        {
            var user = _uow._usersRep.Login(Mail, Password);
            if (user.Error == false)
            {
                HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}
