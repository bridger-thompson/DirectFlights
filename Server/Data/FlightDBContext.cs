using System;
using System.Collections.Generic;
using DirectFlights.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DirectFlights.Server.Data
{
    public partial class FlightDBContext : DbContext
    {
        public FlightDBContext()
        {
        }

        public FlightDBContext(DbContextOptions<FlightDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airline> Airlines { get; set; } = null!;
        public virtual DbSet<Airport> Airports { get; set; } = null!;
        public virtual DbSet<AvailablePlane> AvailablePlanes { get; set; } = null!;
        public virtual DbSet<FlightBooking> FlightBookings { get; set; } = null!;
        public virtual DbSet<FlightDetail> FlightDetails { get; set; } = null!;
        public virtual DbSet<FlightLog> FlightLogs { get; set; } = null!;
        public virtual DbSet<FlightReservation> FlightReservations { get; set; } = null!;
        public virtual DbSet<FlightSchedule> FlightSchedules { get; set; } = null!;
        public virtual DbSet<FlightScheduleTemplate> FlightScheduleTemplates { get; set; } = null!;
        public virtual DbSet<FlightScheduleTemplateOption> FlightScheduleTemplateOptions { get; set; } = null!;
        public virtual DbSet<FlightSeatClass> FlightSeatClasses { get; set; } = null!;
        public virtual DbSet<FlightTotal> FlightTotals { get; set; } = null!;
        public virtual DbSet<Passenger> Passengers { get; set; } = null!;
        public virtual DbSet<PassengerManifest> PassengerManifests { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Plane> Planes { get; set; } = null!;
        public virtual DbSet<PlaneType> PlaneTypes { get; set; } = null!;
        public virtual DbSet<PlaneTypeSeatClass> PlaneTypeSeatClasses { get; set; } = null!;
        public virtual DbSet<SeatClass> SeatClasses { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pg_catalog", "azure")
                .HasPostgresExtension("pg_cron");

            modelBuilder.Entity<Airline>(entity =>
            {
                entity.ToTable("airline", "zack_bridger");

                entity.HasIndex(e => e.Name, "airline_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.ToTable("airport", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<AvailablePlane>(entity =>
            {
                entity.ToTable("available_plane", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.InMaintenence).HasColumnName("in_maintenence");

                entity.Property(e => e.PlaneId).HasColumnName("plane_id");

                entity.HasOne(d => d.Plane)
                    .WithMany(p => p.AvailablePlanes)
                    .HasForeignKey(d => d.PlaneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("available_plane_plane_id_fkey");
            });

            modelBuilder.Entity<FlightBooking>(entity =>
            {
                entity.ToTable("flight_booking", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BookDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("book_date");

                entity.Property(e => e.FlightReservationId).HasColumnName("flight_reservation_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.FlightReservation)
                    .WithMany(p => p.FlightBookings)
                    .HasForeignKey(d => d.FlightReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_booking_flight_reservation_id_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.FlightBookings)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_booking_staff_id_fkey");
            });

            modelBuilder.Entity<FlightDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("flight_details", "zack_bridger");

                entity.Property(e => e.Airline).HasColumnName("airline");

                entity.Property(e => e.ArrivalDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("arrival_date");

                entity.Property(e => e.DepartureDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("departure_date");

                entity.Property(e => e.FromAirport).HasColumnName("from_airport");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SeatId).HasColumnName("seat_id");

                entity.Property(e => e.ToAirport).HasColumnName("to_airport");
            });

            modelBuilder.Entity<FlightLog>(entity =>
            {
                entity.ToTable("flight_log", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ArrivalAirportId).HasColumnName("arrival_airport_id");

                entity.Property(e => e.ArrivalDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("arrival_date");

                entity.Property(e => e.DepartAirportId).HasColumnName("depart_airport_id");

                entity.Property(e => e.DepartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("depart_date");

                entity.Property(e => e.FlightScheduleId).HasColumnName("flight_schedule_id");

                entity.HasOne(d => d.ArrivalAirport)
                    .WithMany(p => p.FlightLogArrivalAirports)
                    .HasForeignKey(d => d.ArrivalAirportId)
                    .HasConstraintName("flight_log_arrival_airport_id_fkey");

                entity.HasOne(d => d.DepartAirport)
                    .WithMany(p => p.FlightLogDepartAirports)
                    .HasForeignKey(d => d.DepartAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_log_depart_airport_id_fkey");

                entity.HasOne(d => d.FlightSchedule)
                    .WithMany(p => p.FlightLogs)
                    .HasForeignKey(d => d.FlightScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_log_flight_schedule_id_fkey");
            });

            modelBuilder.Entity<FlightReservation>(entity =>
            {
                entity.ToTable("flight_reservation", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.FlightScheduleId).HasColumnName("flight_schedule_id");

                entity.Property(e => e.PassengerId).HasColumnName("passenger_id");

                entity.Property(e => e.ReservationDate).HasColumnName("reservation_date");

                entity.Property(e => e.SeatCost).HasColumnName("seat_cost");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.FlightReservations)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_reservation_class_id_fkey");

                entity.HasOne(d => d.FlightSchedule)
                    .WithMany(p => p.FlightReservations)
                    .HasForeignKey(d => d.FlightScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_reservation_flight_schedule_id_fkey");

                entity.HasOne(d => d.Passenger)
                    .WithMany(p => p.FlightReservations)
                    .HasForeignKey(d => d.PassengerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_reservation_passenger_id_fkey");
            });

            modelBuilder.Entity<FlightSchedule>(entity =>
            {
                entity.ToTable("flight_schedule", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ArrivalAirportId).HasColumnName("arrival_airport_id");

                entity.Property(e => e.ArrivalDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("arrival_date");

                entity.Property(e => e.ArrivalGate)
                    .HasMaxLength(5)
                    .HasColumnName("arrival_gate");

                entity.Property(e => e.AssignedPlane).HasColumnName("assigned_plane");

                entity.Property(e => e.Cancelled).HasColumnName("cancelled");

                entity.Property(e => e.DepartureAirportId).HasColumnName("departure_airport_id");

                entity.Property(e => e.DepartureDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("departure_date");

                entity.Property(e => e.DepartureGate)
                    .HasMaxLength(5)
                    .HasColumnName("departure_gate");

                entity.Property(e => e.FlightNumber).HasColumnName("flight_number");

                entity.Property(e => e.SegmentNumber).HasColumnName("segment_number");

                entity.HasOne(d => d.ArrivalAirport)
                    .WithMany(p => p.FlightScheduleArrivalAirports)
                    .HasForeignKey(d => d.ArrivalAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_arrival_airport_id_fkey");

                entity.HasOne(d => d.AssignedPlaneNavigation)
                    .WithMany(p => p.FlightSchedules)
                    .HasForeignKey(d => d.AssignedPlane)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_assigned_plane_fkey");

                entity.HasOne(d => d.DepartureAirport)
                    .WithMany(p => p.FlightScheduleDepartureAirports)
                    .HasForeignKey(d => d.DepartureAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_departure_airport_id_fkey");
            });

            modelBuilder.Entity<FlightScheduleTemplate>(entity =>
            {
                entity.ToTable("flight_schedule_template", "zack_bridger");

                entity.HasIndex(e => new { e.FlightNumber, e.SegmentNumber }, "flight_schedule_template_flight_number_segment_number_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AirlineId).HasColumnName("airline_id");

                entity.Property(e => e.ArrivalAirportId).HasColumnName("arrival_airport_id");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_created");

                entity.Property(e => e.DateRetired)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_retired");

                entity.Property(e => e.DepartureAirportId).HasColumnName("departure_airport_id");

                entity.Property(e => e.FlightNumber).HasColumnName("flight_number");

                entity.Property(e => e.LandingTime).HasColumnName("landing_time");

                entity.Property(e => e.PlaneTypeId).HasColumnName("plane_type_id");

                entity.Property(e => e.SegmentNumber).HasColumnName("segment_number");

                entity.Property(e => e.TakeOffTime).HasColumnName("take_off_time");

                entity.HasOne(d => d.Airline)
                    .WithMany(p => p.FlightScheduleTemplates)
                    .HasForeignKey(d => d.AirlineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_template_airline_id_fkey");

                entity.HasOne(d => d.ArrivalAirport)
                    .WithMany(p => p.FlightScheduleTemplateArrivalAirports)
                    .HasForeignKey(d => d.ArrivalAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_template_arrival_airport_id_fkey");

                entity.HasOne(d => d.DepartureAirport)
                    .WithMany(p => p.FlightScheduleTemplateDepartureAirports)
                    .HasForeignKey(d => d.DepartureAirportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_template_departure_airport_id_fkey");

                entity.HasOne(d => d.PlaneType)
                    .WithMany(p => p.FlightScheduleTemplates)
                    .HasForeignKey(d => d.PlaneTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_schedule_template_plane_type_id_fkey");
            });

            modelBuilder.Entity<FlightScheduleTemplateOption>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("flight_schedule_template_options", "zack_bridger");

                entity.Property(e => e.Airline).HasColumnName("airline");

                entity.Property(e => e.ArrivalAirport).HasColumnName("arrival_airport");

                entity.Property(e => e.DepartureAirport).HasColumnName("departure_airport");

                entity.Property(e => e.PlaneType).HasColumnName("plane_type");
            });

            modelBuilder.Entity<FlightSeatClass>(entity =>
            {
                entity.ToTable("flight_seat_class", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FlightId).HasColumnName("flight_id");

                entity.Property(e => e.SeatId).HasColumnName("seat_id");

                entity.Property(e => e.SuggestedCost).HasColumnName("suggested_cost");

                entity.HasOne(d => d.Flight)
                    .WithMany(p => p.FlightSeatClasses)
                    .HasForeignKey(d => d.FlightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_seat_class_flight_id_fkey");

                entity.HasOne(d => d.Seat)
                    .WithMany(p => p.FlightSeatClasses)
                    .HasForeignKey(d => d.SeatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("flight_seat_class_seat_id_fkey");
            });

            modelBuilder.Entity<FlightTotal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("flight_total", "zack_bridger");

                entity.Property(e => e.DepartureDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("departure_date");

                entity.Property(e => e.FlightNumber).HasColumnName("flight_number");

                entity.Property(e => e.Profit).HasColumnName("profit");

                entity.Property(e => e.Refund).HasColumnName("refund");

                entity.Property(e => e.Total).HasColumnName("total");
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.ToTable("passenger", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<PassengerManifest>(entity =>
            {
                entity.ToTable("passenger_manifest", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoardingDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("boarding_date");

                entity.Property(e => e.FlightBookingId).HasColumnName("flight_booking_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.FlightBooking)
                    .WithMany(p => p.PassengerManifests)
                    .HasForeignKey(d => d.FlightBookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("passenger_manifest_flight_booking_id_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.PassengerManifests)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("passenger_manifest_staff_id_fkey");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.FlightReservationId).HasColumnName("flight_reservation_id");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("payment_date");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.FlightReservation)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.FlightReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("payment_flight_reservation_id_fkey");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("payment_staff_id_fkey");
            });

            modelBuilder.Entity<Plane>(entity =>
            {
                entity.ToTable("plane", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AirlineId).HasColumnName("airline_id");

                entity.Property(e => e.PlaneTypeId).HasColumnName("plane_type_id");

                entity.HasOne(d => d.Airline)
                    .WithMany(p => p.Planes)
                    .HasForeignKey(d => d.AirlineId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("plane_airline_id_fkey");

                entity.HasOne(d => d.PlaneType)
                    .WithMany(p => p.Planes)
                    .HasForeignKey(d => d.PlaneTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("plane_plane_type_id_fkey");
            });

            modelBuilder.Entity<PlaneType>(entity =>
            {
                entity.ToTable("plane_type", "zack_bridger");

                entity.HasIndex(e => e.Name, "plane_type_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<PlaneTypeSeatClass>(entity =>
            {
                entity.ToTable("plane_type_seat_class", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.PlaneTypeId).HasColumnName("plane_type_id");

                entity.Property(e => e.SeatClassId).HasColumnName("seat_class_id");

                entity.HasOne(d => d.PlaneType)
                    .WithMany(p => p.PlaneTypeSeatClasses)
                    .HasForeignKey(d => d.PlaneTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("plane_type_seat_class_plane_type_id_fkey");

                entity.HasOne(d => d.SeatClass)
                    .WithMany(p => p.PlaneTypeSeatClasses)
                    .HasForeignKey(d => d.SeatClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("plane_type_seat_class_seat_class_id_fkey");
            });

            modelBuilder.Entity<SeatClass>(entity =>
            {
                entity.ToTable("seat_class", "zack_bridger");

                entity.HasIndex(e => e.Name, "seat_class_name_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("staff", "zack_bridger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.HasSequence("jobid_seq", "cron");

            modelBuilder.HasSequence("runid_seq", "cron");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
