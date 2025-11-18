using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using VistaraAirlines.Models;

namespace VistaraAirlines.Controllers
{
    public class ManagerController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // GET: Manager/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Manager/Login
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT Username FROM ManagerLogin WHERE Username=@u AND Password=@p", con);

                cmd.Parameters.AddWithValue("@u", model.Username);
                cmd.Parameters.AddWithValue("@p", model.Password);

                con.Open();

                var username = cmd.ExecuteScalar();  // Faster than SqlDataReader

                if (username != null)
                {
                    // Set manager session
                    Session["ManagerUser"] = username.ToString();

                    // Redirect to Manager Dashboard
                    return RedirectToAction("ManagerDashboard", "Home");
                }
                else
                {
                    ViewBag.Message = "Invalid Manager username or password!";
                    return View();
                }
            }
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
