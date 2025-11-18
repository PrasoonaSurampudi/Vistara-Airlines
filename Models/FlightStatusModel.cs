using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace VistaraAirlines.Models
{
    public class FlightStatusModel
    {
        public int FlightStatusId { get; set; }
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}