using System;
using System.Collections.Generic;

namespace DirectFlights.Shared
{
    public partial class PlaneTypeSeatClass
    {
        public int Id { get; set; }
        public int PlaneTypeId { get; set; }
        public int SeatClassId { get; set; }
        public int Capacity { get; set; }

        public virtual PlaneType PlaneType { get; set; } = null!;
        public virtual SeatClass SeatClass { get; set; } = null!;
    }
}
