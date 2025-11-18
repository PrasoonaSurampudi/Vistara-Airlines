using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using VistaraAirlines.Models;

namespace VistaraAirlines.Controllers
{
    public class BookingController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // 🟢 INDEX - Show all bookings
        public ActionResult Index()
        {
            List<BookingModel> list = new List<BookingModel>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                // ✅ FIXED: Added b.FlightId in SELECT
                string q = @"SELECT b.BookingId, b.FlightId, f.FlightNumber, 
                                    p.Name AS PassengerName, 
                                    b.SeatsBooked, b.AmountPaid, 
                                    b.BookingDate, b.BookingStatus
                             FROM Bookings b
                             JOIN Flights f ON b.FlightId = f.FlightId
                             JOIN PassengerDetails p ON b.PassengerId = p.PassengerId";

                SqlCommand cmd = new SqlCommand(q, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BookingModel
                    {
                        BookingId = Convert.ToInt32(dr["BookingId"]),
                        FlightId = Convert.ToInt32(dr["FlightId"]),
                        PassengerName = dr["PassengerName"].ToString(),
                        SeatsBooked = Convert.ToInt32(dr["SeatsBooked"]),
                        AmountPaid = Convert.ToDecimal(dr["AmountPaid"]),
                        BookingDate = Convert.ToDateTime(dr["BookingDate"]),
                        BookingStatus = dr["BookingStatus"].ToString(),

                    });
                }
            }
            return View(list);
        }

        // 🟢 CREATE - GET
        public ActionResult Create()
        {
            ViewBag.Flights = GetFlightSelectList();
            return View();
        }

        // 🟢 CREATE - POST
        [HttpPost]
        public ActionResult Create(BookingModel b)
        {
            if (b.FlightId <= 0)
            {
                ModelState.AddModelError("", "Please select a valid flight.");
                ViewBag.Flights = GetFlightSelectList();
                return View(b);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Insert Passenger
                string pquery = @"INSERT INTO PassengerDetails (Name, Gender, ContactNumber, Email)
                                  OUTPUT INSERTED.PassengerId
                                  VALUES(@Name, 'Male', '9999999999', 'test@gmail.com')";
                SqlCommand cmdP = new SqlCommand(pquery, con);
                cmdP.Parameters.AddWithValue("@Name", b.PassengerName);
                int pid = (int)cmdP.ExecuteScalar();

                // Insert Booking
                string bquery = @"INSERT INTO Bookings (FlightId, PassengerId, SeatsBooked, AmountPaid, BookingStatus)
                                  VALUES (@FlightId, @PassengerId, @SeatsBooked, @AmountPaid, 'Confirmed')";
                SqlCommand cmd = new SqlCommand(bquery, con);
                cmd.Parameters.AddWithValue("@FlightId", b.FlightId);
                cmd.Parameters.AddWithValue("@PassengerId", pid);
                cmd.Parameters.AddWithValue("@SeatsBooked", b.SeatsBooked);
                cmd.Parameters.AddWithValue("@AmountPaid", b.AmountPaid);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // ✏ EDIT - GET
        public ActionResult Edit(int id)
        {
            BookingModel b = new BookingModel();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"SELECT b.BookingId, b.FlightId, b.SeatsBooked, b.AmountPaid, b.BookingStatus,
                                 p.Name AS PassengerName
                                 FROM Bookings b
                                 JOIN PassengerDetails p ON b.PassengerId=p.PassengerId
                                 WHERE BookingId=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    b.BookingId = Convert.ToInt32(dr["BookingId"]);
                    b.FlightId = Convert.ToInt32(dr["FlightId"]);
                    b.PassengerName = dr["PassengerName"].ToString();
                    b.SeatsBooked = Convert.ToInt32(dr["SeatsBooked"]);
                    b.AmountPaid = Convert.ToDecimal(dr["AmountPaid"]);
                    b.BookingStatus = dr["BookingStatus"].ToString();
                }
            }
            ViewBag.Flights = GetFlightSelectList(b.FlightId);
            return View(b);
        }

        // ✏ EDIT - POST
        [HttpPost]
        public ActionResult Edit(BookingModel b)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"UPDATE Bookings 
                             SET SeatsBooked=@SeatsBooked, AmountPaid=@AmountPaid, 
                                 BookingStatus=@BookingStatus, FlightId=@FlightId
                             WHERE BookingId=@BookingId";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@SeatsBooked", b.SeatsBooked);
                cmd.Parameters.AddWithValue("@AmountPaid", b.AmountPaid);
                cmd.Parameters.AddWithValue("@BookingStatus", b.BookingStatus);
                cmd.Parameters.AddWithValue("@FlightId", b.FlightId);
                cmd.Parameters.AddWithValue("@BookingId", b.BookingId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 🧩 Helper to get flight dropdown
        private SelectList GetFlightSelectList(int selectedId = 0)
        {
            var flights = new List<dynamic>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT FlightId, FlightNumber FROM Flights";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    flights.Add(new { FlightId = dr["FlightId"], FlightNumber = dr["FlightNumber"] });
                }
            }
            return new SelectList(flights, "FlightId", "FlightNumber", selectedId);
        }
        public ActionResult ManagerBookings()
        {
            List<BookingModel> list = new List<BookingModel>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT b.BookingId, b.FlightId, f.FlightNumber,
                        p.Name AS PassengerName, 
                        b.SeatsBooked, b.AmountPaid,
                        b.BookingDate, b.BookingStatus
                     FROM Bookings b
                     JOIN Flights f ON b.FlightId = f.FlightId
                     JOIN PassengerDetails p ON b.PassengerId = p.PassengerId
                     WHERE b.BookingStatus = 'Confirmed'";   // 🔥 Only confirmed bookings

                SqlCommand cmd = new SqlCommand(q, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BookingModel
                    {
                        BookingId = Convert.ToInt32(dr["BookingId"]),
                        FlightId = Convert.ToInt32(dr["FlightId"]),
                        PassengerName = dr["PassengerName"].ToString(),
                        SeatsBooked = Convert.ToInt32(dr["SeatsBooked"]),
                        AmountPaid = Convert.ToDecimal(dr["AmountPaid"]),
                        BookingDate = Convert.ToDateTime(dr["BookingDate"]),
                        BookingStatus = dr["BookingStatus"].ToString()
                    });
                }
            }
            return View(list);  // 🔥 This goes to ManagerBookings.cshtml
        }
        public ActionResult CancelledBookings()
        {
            List<BookingModel> list = new List<BookingModel>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT b.BookingId, b.FlightId, f.FlightNumber,
                        p.Name AS PassengerName, 
                        b.SeatsBooked, b.AmountPaid,
                        b.BookingDate, b.BookingStatus
                     FROM Bookings b
                     JOIN Flights f ON b.FlightId = f.FlightId
                     JOIN PassengerDetails p ON b.PassengerId = p.PassengerId
                     WHERE b.BookingStatus = 'Cancelled'";  // 🔥 Only cancelled bookings

                SqlCommand cmd = new SqlCommand(q, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BookingModel
                    {
                        BookingId = Convert.ToInt32(dr["BookingId"]),
                        FlightId = Convert.ToInt32(dr["FlightId"]),
                        PassengerName = dr["PassengerName"].ToString(),
                        SeatsBooked = Convert.ToInt32(dr["SeatsBooked"]),
                        AmountPaid = Convert.ToDecimal(dr["AmountPaid"]),
                        BookingDate = Convert.ToDateTime(dr["BookingDate"]),
                        BookingStatus = dr["BookingStatus"].ToString()
                    });
                }
            }
            return View(list);  // 🔥 This will go to CancelledBookings.cshtml
        }


    }
}