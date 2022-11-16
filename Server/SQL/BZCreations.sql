set search_path to public;
drop schema if exists zack_bridger cascade;
create schema zack_bridger;
set search_path to zack_bridger;

--drop table if exists passenger_manifest;
--drop table if exists payment;
--drop table if exists flight_booking;
--drop table if exists flight_reservation;
--drop table if exists available_plane;
--drop table if exists flight_route;
--drop table if exists passenger;
--drop table if exists plane_type_seat_class;
--drop table if exists seat_class;
--drop table if exists flight_log;
--drop table if exists flight_schedule;
--drop table if exists flight_schedule_log;
--drop table if exists airport;
--drop table if exists plane;
--drop table if exists plane_type;
--drop table if exists airline;
--drop table if exists staff;

create table airline (
	id 		serial primary key,
	name	text unique not null
);

create table staff (
	id 		serial primary key,
	name 	text not null
);

create table airport (
	id 			serial primary key,
	name 		text not null,
	address 	text not null
);

create table plane_type (
	id 		serial primary key,
	name 	text  unique not null
);

create table plane (
	id 				serial primary key,
	plane_type_id	int references plane_type(id) not null,
	airline_id		int references airline(id) not null
);

create table available_plane (
	id 				serial primary key,
	plane_id		int references plane(id) not null,
	in_maintenence 	bool not null
);

create table passenger (
	id 		serial primary key,
	name 	text not null
);

create table seat_class (
	id 		serial primary key,
	name 	text unique not null
);

create table plane_type_seat_class (
	id 				serial primary key,
	plane_type_id	int references plane_type(id) not null,
	seat_class_id	int references seat_class(id) not null,
	capacity		int not null
);

create table flight_route (
	id 						serial primary key,	
	airline_id				int references airline(id) not null,
	depart_airport_id		int references airport(id) not null,
	arrival_airport_id		int references airport(id) not null,
	date_created			timestamp not null,
	date_retired			timestamp
);

create table flight_schedule (
	id 						serial primary key,
	flight_route_id			int references flight_route(id) not null,
	available_plane_id		int references available_plane(id) not null,
	departure_date			timestamp not null,
	arrival_date			timestamp not null,
	departure_gate			varchar(5) not null,
	arrival_gate			varchar(5) not null,
	cancelled				bool not null,
	check (arrival_date > departure_date)
);

create table flight_schedule_log (
	id 					serial primary key,
	available_plane_id	int references available_plane(id) not null,
	departure_date		timestamp not null,
	arrival_date		timestamp not null,
	departure_gate		varchar(5) not null,
	arrival_gate		varchar(5) not null,
	flight_schedule_id	int references flight_schedule(id) not null,
	check (arrival_date > departure_date)
);

create table flight_reservation (
	id 					serial primary key,
	passenger_id 		int references passenger(id) not null,
	flight_schedule_id 	int references flight_schedule(id) not null,
	class_id 			int references seat_class(id) not null,
	reservation_date	date not null,
	seat_cost			numeric not null
);

create table flight_booking (
	id 						serial primary key,
	flight_reservation_id	int references flight_reservation(id) not null,
	book_date				timestamp not null,
	staff_id				int references staff(id) not null
);

create table payment (
	id 						serial primary key,
	staff_id				int references staff(id) not null,
	flight_reservation_id 	int references flight_reservation(id) not null,
	payment_date			timestamp not null,
	amount 					numeric not null
);

create table passenger_manifest (
	id 					serial primary key,
	flight_booking_id	int references flight_booking(id) not null,
	boarding_date		timestamp not null,
	staff_id			int references staff(id) not null
);

create table flight_log (
	id 						serial primary key,
	flight_schedule_id		int references flight_schedule(id) not null,
	depart_airport_id		int references airport(id) not null,
	arrival_airport_id		int references airport(id),
	depart_date				timestamp not null,
	arrival_date			timestamp
);

create or replace procedure pop_airline(amount integer)
	language plpgsql
	as 
$$
begin 
	for i in 1..amount loop 
		insert into airline 
			values (i, 'Airline' || i);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_staff(amount integer)
	language plpgsql
	as 
$$
begin 
	for i in 1..amount loop 
		insert into staff 
			values (i, 'Staff' || i);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_airport(amount integer)
	language plpgsql
	as 
$$
begin 
	for i in 1..amount loop 
		insert into airport  
			values (i, 'Airport' || i, 'Address' || i);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_plane_type_seat_class(amount integer)
	language plpgsql
	as 
$$
declare 
	plane_type_id	int;
	seat_class_id	int;
begin 
	for plane_type_id in select id from plane_type loop
		for seat_class_id in select id from seat_class loop
			insert into plane_type_seat_class (plane_type_id, seat_class_id, capacity)
				values (plane_type_id, seat_class_id, 40);
		end loop;
	end loop;
	commit;
end;
$$;

create or replace procedure pop_plane_type(amount integer)
	language plpgsql
	as 
$$
begin 
	for i in 1..amount loop 
		insert into plane_type 
			values (i, 'Type' || i);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_plane(amount integer)
	language plpgsql
	as 
$$
declare 
	airline_id 		int;
	plane_type_id	int;
begin 
	for airline_id in select id from airline loop
		for plane_type_id in select id from plane_type loop
			for i in 1..amount loop
				insert into plane (plane_type_id, airline_id)
					values (plane_type_id, airline_id);
			end loop;
		end loop;
	end loop;
	commit;
end;
$$;

create or replace procedure pop_passenger(amount integer)
	language plpgsql
	as 
$$
begin 
	for i in 1..amount loop 
		insert into passenger 
			values (i, 'Passenger' || i);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_seat_class(amount integer)
	language plpgsql
	as 
$$
begin 
	insert into seat_class (name) 
	values ('First Class'),
		('Business Class'),
		('Couch Class');
	commit;
end;
$$;

create or replace procedure pop_flight_schedule(amount integer, date_range interval)
	language plpgsql
	as 
$$
declare 
	route_max	int;
	plane_max	int;
	air_id 	int;
	start_date 	timestamp;
begin 
	for i in 1..amount loop
		for air_id in select id from airline loop
			select count(*) into plane_max
				from available_plane ap
				inner join plane p 
					on (ap.plane_id = p.id)
				where in_maintenence = false;
			
			select count(*) into route_max
				from flight_route fr
				where fr.airline_id = air_id;
			
			start_date := get_random_date(now()::timestamp, date_range);
			
			insert into flight_schedule (flight_route_id, available_plane_id, departure_date, arrival_date, departure_gate, arrival_gate, cancelled)
			values (get_random_number(route_max), get_random_number(plane_max), 
					start_date, get_random_date(start_date, '10 hours'),
					get_random_gate(), get_random_gate(), get_random_bool());
		end loop;
	end loop;
	commit;
end;
$$;


create or replace procedure pop_flight_schedule_log(amount integer)
	language plpgsql
	as 
$$
begin 
	insert into flight_schedule_log
		values (1, 3, '11/10/2022 8:30', '11/10/2022 11:30', 'A1', 'B2', 1),
		(2, 2, '11/10/2022 18:30', '11/11/2022 6:30', 'B6', 'A2', 2);
	commit;
end;
$$;

create or replace procedure pop_flight_reservation(amount integer)
	language plpgsql
	as 
$$
declare 
	passenger_id	int;
	flight_plan_id	int;
	class_id		int;
begin 
	for passenger_id in select id from passenger loop
		for flight_plan_id in select id from flight_schedule loop
			for class_id in select id from seat_class loop
				insert into flight_reservation (passenger_id, flight_schedule_id, class_id, reservation_date, seat_cost)
					values (passenger_id, flight_plan_id, class_id, '10/10/2022 10:00', 60);
			end loop;
		end loop;
	end loop;
	commit;
end;
$$;

create or replace procedure pop_flight_booking(amount integer)
	language plpgsql
	as 
$$
declare 
	flight_res_id 	int;
begin 
	for flight_res_id in select id from flight_reservation loop
		insert into flight_booking (flight_reservation_id, book_date, staff_id)
			values (flight_res_id, '11/10/2022 3:06', 1);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_available_plane()
	language plpgsql
	as 
$$
declare 
	plane_id	int;
begin 
	for plane_id in select id from plane loop
		insert into available_plane (plane_id, in_maintenence)
			values (plane_id, get_random_bool());
	end loop;		
	commit;
end;
$$;

create or replace procedure pop_payment(amount integer)
	language plpgsql
	as 
$$
declare 
	flight_res_id 	int;
begin 
	for flight_res_id in select id from flight_reservation loop
		insert into payment (staff_id, flight_reservation_id, payment_date, amount)
			values (1, flight_res_id, '11/11/2022', 60);
	end loop;
	insert into payment (staff_id, flight_reservation_id, payment_date, amount)
			values (1, 1, '11/12/2022', -200);
	commit;
end;
$$;

create or replace procedure pop_passenger_manifest(amount integer)
	language plpgsql
	as 
$$
declare 
	flight_booking_id	int;
	curs cursor for select flight_booking.id as id, flight_schedule.departure_date as dep_date from flight_booking 
		inner join flight_reservation
			on (flight_reservation.id = flight_booking.flight_reservation_id)
		inner join flight_schedule
			on (flight_schedule.id = flight_reservation.flight_schedule_id);
begin 
	for flight in curs loop
		insert into passenger_manifest (flight_booking_id, boarding_date, staff_id)
			values (flight.id, flight.dep_date, 2);
	end loop;
	commit;
end;
$$;

create or replace procedure pop_flight_log()
    language plpgsql
    as
$$
declare
	flight_past_cursor cursor for select * from flight_schedule f inner join flight_route fr on (f.flight_route_id=fr.id) where f.arrival_date <= now() and not f.cancelled;
	flight_future_cursor cursor for select * from flight_schedule f inner join flight_route fr on (f.flight_route_id=fr.id) where f.arrival_date > now() and not f.cancelled;
begin
    for flight in flight_past_cursor loop
        insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        values (flight.id, get_random_date(flight.departure_date, '10 hours'), get_random_date(flight.arrival_date, '10 hours'), 
       		flight.depart_airport_id, flight.arrival_airport_id);
   	end loop;
   	commit;
   	for flight in flight_future_cursor loop
        insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        values (flight.id, get_random_date(flight.departure_date, '10 hours'), null, 
       		flight.depart_airport_id, null);
    end loop;
   	commit;
end;    
$$;

create or replace procedure pop_flight_route(amount integer)
	language plpgsql
	as 
$$
declare 
	max_airports		int;
	start_date			timestamp;
	airline_id			int;	
	depart_airport_id 	int;
	arrival_airport_id	int;
begin
	select count(*) into max_airports from airport;
	for i in 1..amount loop
		for airline_id in select id from airline loop
			for depart_airport_id in select id from airport loop
				for arrival_airport_id in select id from airport loop					
					start_date := get_random_date(now()::timestamp, '365 days');
					if depart_airport_id <> arrival_airport_id then
						insert into flight_route (depart_airport_id, arrival_airport_id, date_created, date_retired, airline_id)
							values (depart_airport_id, arrival_airport_id, start_date, null, airline_id);		
					end if;					
				end loop;				
			end loop;			
		end loop;	
	end loop;
	commit;
end;
$$;

create or replace function get_random_number(max_value integer)
	returns integer
	language plpgsql
	as
$$
begin
	return floor(random() * (max_value) + 1)::int;
end;
$$;

create or replace function get_random_date(start_date timestamp, time_frame interval) 
	returns timestamp 
	language plpgsql 
	as
$$
begin 
	return start_date + (random() * (start_date+time_frame - start_date));
end;
$$;

create or replace function get_random_gate()
	returns text
	language plpgsql
	as
$$
declare 
	letter 	text;
	num 	int;
begin
	select upper(chr(int4(random()*26)+65)) into letter;
	select floor(random() * 9 + 1)::int into num;
	return letter || num;
end;
$$;

create or replace function get_random_bool()
	returns bool 
	language plpgsql 
	as 
$$
begin 
	return random() >= 0.9;
end;
$$;

create or replace procedure pop_all()
	language plpgsql
	as 
$$
begin 
	call pop_airline(3);
	call pop_staff(5);
	call pop_airport(3);
	call pop_plane_type(3);
	call pop_plane(5);
	call pop_available_plane();
	call pop_passenger(5);
	call pop_seat_class(3);
	call pop_plane_type_seat_class(0);
	call pop_flight_route(10);
	call pop_flight_schedule(20, '-180 days');
	call pop_flight_schedule(10, '30 days');
	call pop_flight_schedule_log(0);
	call pop_flight_reservation(0);
	call pop_flight_booking(0);
	call pop_payment(0);
	call pop_passenger_manifest(0);
	call pop_flight_log();
end;
$$;


call pop_all();




