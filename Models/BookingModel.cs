using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VistaraAirlines.Models;

namespace VistaraAirlines.Models
{
    public class BookingModel
    {
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public string PassengerName { get; set; }
        public int SeatsBooked { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
    }
}