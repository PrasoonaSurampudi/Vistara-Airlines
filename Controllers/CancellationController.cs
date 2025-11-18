using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using VistaraAirlines.Models;

namespace VistaraAirlines.Controllers
{
    public class CancellationController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["VistaraDb"].ConnectionString;

        // THIS IS THE ONLY PAGE YOU WILL SEE
        public ActionResult Cancel()
        {
            List<CancelModel> list = new List<CancelModel>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"SELECT b.BookingId, p.Name AS PassengerName, 
                                    f.FlightNumber, b.BookingDate, b.BookingStatus
                             FROM Bookings b
                             JOIN PassengerDetails p ON b.PassengerId=p.PassengerId
                             JOIN Flights f ON b.FlightId=f.FlightId";

                SqlCommand cmd = new SqlCommand(q, con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new CancelModel
                    {
                        BookingId = Convert.ToInt32(dr["BookingId"]),
                        PassengerName = dr["PassengerName"].ToString(),
                        FlightNumber = dr["FlightNumber"].ToString(),
                        BookingDate = Convert.ToDateTime(dr["BookingDate"]),
                        Remarks = dr["BookingStatus"].ToString()
                    });
                }
            }

            return View(list); // loads Cancel.cshtml
        }


        // CANCEL POST FOR A SINGLE BOOKING
        [HttpPost]
        public ActionResult CancelBooking(int bookingId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlTransaction t = con.BeginTransaction();

                try
                {
                    // Insert Cancel Record
                    SqlCommand cmd = new SqlCommand(
                        @"INSERT INTO Cancels (BookingId, RefundAmount, Remarks)
                          VALUES (@BookingId, 0, 'Cancelled by system')", con, t);

                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    cmd.ExecuteNonQuery();

                    // Update Status
                    SqlCommand cmd2 = new SqlCommand(
                        "UPDATE Bookings SET BookingStatus='Cancelled' WHERE BookingId=@id", con, t);

                    cmd2.Parameters.AddWithValue("@id", bookingId);
                    cmd2.ExecuteNonQuery();

                    t.Commit();
                }
                catch
                {
                    t.Rollback();
                }
            }

            return RedirectToAction("Cancel");
        }
    }
}
