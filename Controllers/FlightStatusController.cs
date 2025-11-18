using System;

using System.Collections.Generic;

using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using VistaraAirlines.Models;


namespace VistaraAirlines.Controllers
{
    public class FlightStatusController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // 🟢 Display all flight statuses
        public ActionResult Index()
        {
            List<FlightStatusModel> list = new List<FlightStatusModel>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT fs.FlightStatusId, f.FlightNumber, fs.Status, fs.UpdatedOn 
                             FROM FlightStatus fs 
                             JOIN Flights f ON fs.FlightId = f.FlightId";
                SqlCommand cmd = new SqlCommand(q, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new FlightStatusModel
                    {
                        FlightStatusId = Convert.ToInt32(dr["FlightStatusId"]),
                        FlightNumber = dr["FlightNumber"].ToString(),
                        Status = dr["Status"].ToString(),
                        UpdatedOn = Convert.ToDateTime(dr["UpdatedOn"])
                    });
                }
            }
            return View(list);
        }

        // 🟢 Add new flight status
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FlightStatusModel s)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"INSERT INTO FlightStatus (FlightId, Status) VALUES (@FlightId, @Status)";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@FlightId", s.FlightId);
                cmd.Parameters.AddWithValue("@Status", s.Status);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // ✏ Edit status
        public ActionResult Edit(int id)
        {
            FlightStatusModel s = new FlightStatusModel();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT fs.FlightStatusId, fs.FlightId, f.FlightNumber, fs.Status 
                             FROM FlightStatus fs JOIN Flights f ON fs.FlightId=f.FlightId 
                             WHERE fs.FlightStatusId=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    s.FlightStatusId = Convert.ToInt32(dr["FlightStatusId"]);
                    s.FlightId = Convert.ToInt32(dr["FlightId"]);
                    s.FlightNumber = dr["FlightNumber"].ToString();
                    s.Status = dr["Status"].ToString();
                }
            }
            return View(s);
        }

        [HttpPost]
        public ActionResult Edit(FlightStatusModel s)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE FlightStatus 
                             SET Status=@Status, UpdatedOn=GETDATE() 
                             WHERE FlightStatusId=@FlightStatusId";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@Status", s.Status);
                cmd.Parameters.AddWithValue("@FlightStatusId", s.FlightStatusId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // ❌ Delete status
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM FlightStatus WHERE FlightStatusId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
        
    }
}