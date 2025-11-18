using System;

namespace VistaraAirlines.Models
{
    public class CancelModel
    {
        public int CancelId { get; set; }
        public int BookingId { get; set; }
        public string PassengerName { get; set; }
        public string FlightNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal RefundAmount { get; set; }
        public string Remarks { get; set; }
    }
}