using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectFlights.Shared
{
    public partial class AirlineTotal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Total { get; set; }
        public decimal? Refund { get; set; }
        public decimal? Profit { get; set; }
    }
}
