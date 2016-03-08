using System;
using System.Configuration;
using System.Data.Entity.Core;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcWithAspNetIdentityUsingStructureMap.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityDbContext _identityDbContext;
        public HomeController(UserManager<IdentityUser> userManager, IdentityDbContext context)
        {
            _userManager = userManager;
            _identityDbContext = context;
        }        
        public ActionResult Index()
        {
            try {                
                _identityDbContext.Database.CreateIfNotExists();
            } catch (ProviderIncompatibleException)
            {                
                ModelState.AddModelError(string.Empty, "Could not create the database, make sure you have the right connection string on web.config. Use Visual Studio's SQL Object Explorer to check what is your LocalDb instance name and update the connection string in web.config accordingly");
                return View("~/Views/Home/Index.cshtml", Enumerable.Empty<IdentityUser>());
            }
            return View("~/Views/Home/Index.cshtml", _identityDbContext.Users.ToList());
        }

        [HttpPost]
        public ActionResult CreateUser(string username, string password)
        {
            var user = new IdentityUser { UserName = username };            
            var createUserResult = _userManager.Create(user, password);
            if (createUserResult.Succeeded)
                return RedirectToAction("Index");

            createUserResult.Errors.ToList().ForEach(error => ModelState.AddModelError(string.Empty, error));

            return Index();
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            var user = _userManager.FindById(id);
            var deleteUserResult = _userManager.Delete(user);
            if (deleteUserResult.Succeeded)
                return RedirectToAction("Index");

            deleteUserResult.Errors.ToList().ForEach(error => ModelState.AddModelError(string.Empty, error));

            return Index();
        }
    }
}