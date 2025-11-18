using System.Web.Mvc;

namespace VistaraAirlines.Controllers
{
    public class HomeController : Controller
    {
        // DEFAULT ACTION → /Home/Index
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Index()
        {
            if (Session["User"] != null)
                return RedirectToAction("UserDashboard");

            if (Session["ManagerUser"] != null)
                return RedirectToAction("ManagerDashboard");

            return RedirectToAction("Login", "User");
        }

        // USER DASHBOARD VIEW
        public ActionResult UserDashboard()
        {
            if (Session["User"] == null)
                return RedirectToAction("Login", "User");

            return View();
        }

        // MANAGER DASHBOARD VIEW
        public ActionResult ManagerDashboard()
        {
            if (Session["ManagerUser"] == null)
                return RedirectToAction("Login", "Manager");

            return View();
        }
    }
}
