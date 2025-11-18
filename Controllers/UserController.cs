using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace VistaraAirlines.Controllers
{
    public class UserController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // LOGIN - GET
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // LOGIN - POST
        [HttpPost]
        public ActionResult Login(string Username, string Password, string RoleType)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                // ---------------- SELECT QUERY BASED ON ROLE ----------------
                if (RoleType == "User")
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM UserLogin WHERE Username=@u AND Password=@p";
                }
                else if (RoleType == "Manager")
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM ManagerLogin WHERE Username=@u AND Password=@p";
                }
                else
                {
                    ViewBag.Error = "Please select a valid role.";
                    return View();
                }

                cmd.Parameters.AddWithValue("@u", Username);
                cmd.Parameters.AddWithValue("@p", Password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    // ----------------------- WHEN USER LOGINS -----------------------
                    if (RoleType == "User")
                    {
                        Session["User"] = Username;
                        return RedirectToAction("UserDashboard", "Home");
                    }

                    // ----------------------- WHEN MANAGER LOGINS -----------------------
                    if (RoleType == "Manager")
                    {
                        Session["ManagerUser"] = Username;
                        return RedirectToAction("ManagerDashboard", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Invalid username or password.";
                }
            }

            return View();
        }

        // LOGOUT
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
