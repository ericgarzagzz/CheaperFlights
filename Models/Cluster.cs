using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheaperFlights.Models
{
    internal class Cluster
    {
        public string? Name { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string? DepartureInfo { get; set; }
        public string? ReturnInfo { get; set; }
        public string Length { get; set; }
    }
}
