using System;

namespace VistaraAirlines.Models
{
    public class FlightModel
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string FlightStatus { get; set; }
    }
}
