using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using VistaraAirlines.Models;

namespace VistaraAirlines.Controllers
{
    public class FlightsController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // -------------------------------
        // INDEX : Show all flights
        // -------------------------------
        public ActionResult Index()
        {
            List<FlightModel> list = new List<FlightModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Flights", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new FlightModel
                    {
                        FlightId = Convert.ToInt32(dr["FlightId"]),
                        FlightNumber = dr["FlightNumber"].ToString(),
                        Source = dr["Source"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        DepartureTime = Convert.ToDateTime(dr["DepartureTime"]),
                        ArrivalTime = Convert.ToDateTime(dr["ArrivalTime"]),
                        Fare = Convert.ToDecimal(dr["Fare"]),
                        TotalSeats = Convert.ToInt32(dr["TotalSeats"]),
                        AvailableSeats = Convert.ToInt32(dr["AvailableSeats"]),
                        FlightStatus = dr["FlightStatus"].ToString()
                    });
                }
            }

            return View(list);
        }

        // -------------------------------
        // CREATE : GET
        // -------------------------------
        public ActionResult Create()
        {
            return View();
        }

        // -------------------------------
        // CREATE : POST
        // -------------------------------
        [HttpPost]
        public ActionResult Create(FlightModel model)
        {
            model.DepartureTime = Convert.ToDateTime(Request.Form["DepartureTime"]);
            model.ArrivalTime = Convert.ToDateTime(Request.Form["ArrivalTime"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Flights
                    (FlightNumber, Source, Destination, DepartureTime, ArrivalTime, Fare, TotalSeats, AvailableSeats, FlightStatus)
                    VALUES (@fno, @src, @dst, @dt, @at, @fare, @ts, @as, @status)", con);

                cmd.Parameters.AddWithValue("@fno", model.FlightNumber);
                cmd.Parameters.AddWithValue("@src", model.Source);
                cmd.Parameters.AddWithValue("@dst", model.Destination);
                cmd.Parameters.AddWithValue("@dt", model.DepartureTime);
                cmd.Parameters.AddWithValue("@at", model.ArrivalTime);
                cmd.Parameters.AddWithValue("@fare", model.Fare);
                cmd.Parameters.AddWithValue("@ts", model.TotalSeats);
                cmd.Parameters.AddWithValue("@as", model.TotalSeats); // initial available seats
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(model.FlightStatus) ? "Scheduled" : model.FlightStatus);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // -------------------------------
        // EDIT : GET
        // -------------------------------
        public ActionResult Edit(int id)
        {
            FlightModel model = new FlightModel();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Flights WHERE FlightId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    model.FlightId = Convert.ToInt32(dr["FlightId"]);
                    model.FlightNumber = dr["FlightNumber"].ToString();
                    model.Source = dr["Source"].ToString();
                    model.Destination = dr["Destination"].ToString();
                    model.DepartureTime = Convert.ToDateTime(dr["DepartureTime"]);
                    model.ArrivalTime = Convert.ToDateTime(dr["ArrivalTime"]);
                    model.Fare = Convert.ToDecimal(dr["Fare"]);
                    model.TotalSeats = Convert.ToInt32(dr["TotalSeats"]);
                    model.AvailableSeats = Convert.ToInt32(dr["AvailableSeats"]);
                    model.FlightStatus = dr["FlightStatus"].ToString();
                }
            }
            return View(model);
        }

        // -------------------------------
        // EDIT : POST
        // -------------------------------
        [HttpPost]
        public ActionResult Edit(FlightModel model)
        {
            model.DepartureTime = Convert.ToDateTime(Request.Form["DepartureTime"]);
            model.ArrivalTime = Convert.ToDateTime(Request.Form["ArrivalTime"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Flights SET 
                    FlightNumber=@fno, Source=@src, Destination=@dst,
                    DepartureTime=@dt, ArrivalTime=@at,
                    Fare=@fare, TotalSeats=@ts, AvailableSeats=@as, FlightStatus=@status
                    WHERE FlightId=@id", con);

                cmd.Parameters.AddWithValue("@fno", model.FlightNumber);
                cmd.Parameters.AddWithValue("@src", model.Source);
                cmd.Parameters.AddWithValue("@dst", model.Destination);
                cmd.Parameters.AddWithValue("@dt", model.DepartureTime);
                cmd.Parameters.AddWithValue("@at", model.ArrivalTime);
                cmd.Parameters.AddWithValue("@fare", model.Fare);
                cmd.Parameters.AddWithValue("@ts", model.TotalSeats);
                cmd.Parameters.AddWithValue("@as", model.AvailableSeats);
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(model.FlightStatus) ? "Scheduled" : model.FlightStatus);
                cmd.Parameters.AddWithValue("@id", model.FlightId);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // -------------------------------
        // DELETE : GET
        // -------------------------------
        public ActionResult Delete(int id)
        {
            FlightModel model = new FlightModel();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Flights WHERE FlightId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    model.FlightId = Convert.ToInt32(dr["FlightId"]);
                    model.FlightNumber = dr["FlightNumber"].ToString();
                }
            }
            return View(model);
        }

        // -------------------------------
        // DELETE : POST
        // -------------------------------
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Flights WHERE FlightId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // -------------------------------
        // MANAGE STATUS
        // -------------------------------
        public ActionResult ManageStatus()
        {
            List<FlightModel> list = new List<FlightModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT FlightId, FlightNumber, Source, Destination, FlightStatus FROM Flights", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new FlightModel
                    {
                        FlightId = Convert.ToInt32(dr["FlightId"]),
                        FlightNumber = dr["FlightNumber"].ToString(),
                        Source = dr["Source"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        FlightStatus = dr["FlightStatus"].ToString()
                    });
                }
            }

            return View(list);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Flights SET FlightStatus=@st WHERE FlightId=@id", con);
                cmd.Parameters.AddWithValue("@st", status);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ManageStatus");
        }
        public ActionResult FlightStatus()
        {
            List<FlightModel> list = new List<FlightModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT FlightNumber, Source, Destination, DepartureTime, ArrivalTime, FlightStatus FROM Flights";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new FlightModel
                    {
                        FlightNumber = dr["FlightNumber"].ToString(),
                        Source = dr["Source"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        DepartureTime = Convert.ToDateTime(dr["DepartureTime"]),
                        ArrivalTime = Convert.ToDateTime(dr["ArrivalTime"]),
                        FlightStatus = dr["FlightStatus"].ToString()
                    });
                }
            }

            return View(list);
        }


    }
}
