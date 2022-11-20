set search_path to public;
drop schema if exists zack_bridger cascade;
create schema zack_bridger;
set search_path to zack_bridger;

--drop table if exists passenger_manifest;
--drop table if exists payment;
--drop table if exists flight_booking;
--drop table if exists flight_reservation;
--drop table if exists available_plane;
--drop table if exists flight_schedule_template;
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

create table flight_schedule_template (
	id 						serial primary key,	
	flight_number			int not null,
	segment_number			int not null,
	airline_id				int references airline(id) not null,
	departure_airport_id	int references airport(id) not null,
	arrival_airport_id		int references airport(id) not null,
	take_off_time			time not null,
	landing_time			time not null,
	plane_type_id			int references plane_type(id) not null,
	date_created			timestamp not null,
	date_retired			timestamp,
	check (landing_time > take_off_time),
	check (departure_airport_id <> arrival_airport_id),
	unique (flight_number, segment_number)
);

create table flight_schedule (
	id 						serial primary key,
	flight_number			int not null,
	segment_number			int not null,
	assigned_plane			int references available_plane(id) not null,
	departure_airport_id	int references airport(id) not null,
	arrival_airport_id		int references airport(id) not null,
	departure_date			timestamp not null,
	arrival_date			timestamp not null,
	departure_gate			varchar(5) not null,
	arrival_gate			varchar(5) not null,
	cancelled				bool not null,
	check (arrival_date > departure_date),
	check (departure_airport_id <> arrival_airport_id)
);

create table flight_seat_class (
	id					serial primary key,
	seat_id				int references seat_class(id) not null,
	flight_id			int references flight_schedule(id) not null,
	suggested_cost		numeric not null
);

create table flight_reservation (
	id 					serial primary key,
	passenger_id 		int references passenger(id) not null,
	flight_schedule_id 	int references flight_schedule(id) not null,
	class_id 			int references flight_seat_class(id) not null,
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

/* Procedures to populate all tables with data */

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
	insert into staff 
			values (1, 'System');
	for i in 2..amount loop 
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
	air_id 			int;
	start_date 		date;
	flight_id		int;
	is_cancelled	bool;
begin 
	for i in 1..amount loop
		for air_id in select id from airline loop
			begin				
				start_date := get_random_date(now()::date, date_range);
			
				call create_flight_schedule(start_date, air_id);
			
				select get_random_bool() into is_cancelled;
				if is_cancelled then
					select f.id into flight_id
						from flight_schedule f
						order by random() limit 1;
					update flight_schedule 
						set cancelled = is_cancelled 
						where id=flight_id;
				end if;
			exception when 
				sqlstate '50001' then
					null;
			 	when
				sqlstate '50002' then
					null;
			end;
		end loop;		
	end loop;
	commit;
end;
$$;

create or replace procedure pop_flight_reservation(amount integer)
	language plpgsql
	as 
$$
declare 
	passenger_id			int;
	flight_schedule_id		int;
	class_id				int;
	start_date 				timestamp;
	price					numeric;
begin 
	for passenger_id in select id from passenger loop
		begin
			select fs1.id into flight_schedule_id 
				from flight_schedule fs1
				order by random() limit 1;
				
			select fc.id, fc.suggested_cost into class_id, price
				from flight_seat_class fc
				inner join flight_schedule fs3
					on (fs3.id = fc.flight_id)
				where fc.flight_id = flight_schedule_id
				order by random() limit 1;
			
			select fs2.departure_date into start_date
				from flight_schedule fs2
				where fs2.id = flight_schedule_id;
				
			insert into flight_reservation (passenger_id, flight_schedule_id, class_id, reservation_date, seat_cost)
				values (passenger_id, flight_schedule_id, class_id, get_random_datetime(start_date, '-30 days'), price);
			exception 
				when sqlstate '50003' then
					null;
		end;
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
	staff_id		int;
	start_date 		timestamp;
begin 
	for flight_res_id in 
		select fr.id from flight_reservation fr
			inner join flight_schedule fs2
				on (fr.flight_schedule_id = fs2.id)
			where fs2.cancelled is not true
	loop
		begin
			select id into staff_id 
				from staff
				where id != 1
				order by random() limit 1;
		
			select fs2.departure_date into start_date
				from flight_reservation fr 
				inner join flight_schedule fs2 
					on (fr.flight_schedule_id = fs2.id)
				where fr.id = flight_res_id;
			
			insert into flight_booking (flight_reservation_id, book_date, staff_id)
				values (flight_res_id, get_random_datetime(start_date,'-2 hour'), staff_id);
			exception when
				sqlstate '50004' then
					null;
		end;
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
	flight_past_cursor cursor for select * from flight_schedule f where f.arrival_date <= now() and not f.cancelled;
	flight_future_cursor cursor for select * from flight_schedule f where f.arrival_date > now() and not f.cancelled;
	is_delayed	bool;
begin
    for flight in flight_past_cursor loop
	    
	    is_delayed := get_random_bool();	    
	   	if is_delayed then 	   		
        	insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        		values (flight.id, get_random_datetime(flight.departure_date, '2 hours'), get_random_datetime(flight.arrival_date, '2 hours'), 
       				flight.departure_airport_id, flight.arrival_airport_id);
       	else
       		insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        		values (flight.id, flight.departure_date, flight.arrival_date, 
       				flight.departure_airport_id, flight.arrival_airport_id);
       	end if;
   	end loop;
   	commit;
   	for flight in flight_future_cursor loop
	   	is_delayed := get_random_bool();
	   	if is_delayed then	   	
        	insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        		values (flight.id, get_random_datetime(flight.departure_date, '10 hours'), null, flight.departure_airport_id, null);
       	else
       		insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        		values (flight.id,flight.departure_date, null, flight.departure_airport_id, null);
       	end if;
    end loop;
   	commit;
end;    
$$;

create or replace procedure pop_flight_schedule_template(amount integer)
	language plpgsql
	as 
$$
declare 
	start_date				timestamp;
	airline_id				int;	
	departure_airport_id 	int;
	arrival_airport_id		int;
	flight_num				int;
	segment_num				int;
	start_time				int;
	end_time				int;
	plane_type_id			int;
begin
	for airline_id in select id from airline loop
		for i in 1..amount loop
			begin
				start_date := get_random_datetime(now()::timestamp, '365 days');
		
				select get_random_number(1,3) into segment_num;
				select get_random_number(1,999) into flight_num;
				
				select id into arrival_airport_id
					from airport 
					order by random() limit 1;
				
				select get_random_number(0,3) into end_time;
						
				for s in 1..segment_num loop
					start_time := end_time;
					select get_random_number(start_time+1,start_time+3) into end_time;
					
						
					departure_airport_id := arrival_airport_id;
					select id into arrival_airport_id
						from airport 
						where id <> departure_airport_id
						order by random() limit 1;
							
					select p.id into plane_type_id
						from plane_type p
						order by random() limit 1;
							
					insert into flight_schedule_template (flight_number, segment_number, airline_id, departure_airport_id, arrival_airport_id, take_off_time, landing_time, plane_type_id, date_created, date_retired)
						values (flight_num, s, airline_id, departure_airport_id, arrival_airport_id, (start_time || ':00')::time, (end_time || ':00')::time, plane_type_id, start_date, null);
				end loop;
				exception when others then null;
			end;			
		end loop;							
	end loop;
	commit;
end;
$$;

create or replace procedure pop_flight_seat_class()
	language plpgsql
	as 
$$
declare
	flight 	int;
	seat	int;
begin
	for flight in select id from flight_schedule loop		
		for seat in select id from seat_class loop			
			insert into flight_seat_class (seat_id, flight_id, suggested_cost)
				values (seat, flight, get_random_number(100,300));			
		end loop;	
	end loop;	
end;
$$;

create or replace procedure create_flight_schedule(departure_date date, air_id integer)
	language plpgsql
	as 
$$
declare 
	plane_id					int;
	schedule_template_cursor	cursor for select * from flight_schedule_template fst where fst.airline_id = air_id;
	depart_datetime				timestamp;
	arrival_datetime			timestamp;
begin
	
	for schedule_template in schedule_template_cursor loop
		
		depart_datetime := departure_date + schedule_template.take_off_time;
		arrival_datetime := departure_date + schedule_template.landing_time;
	
		select ap.id into plane_id
			from plane_type pt
			inner join plane p 
				on (pt.id = p.plane_type_id)
			inner join available_plane ap 
				on (ap.plane_id = p.id)
			left join flight_schedule fs2 
				on (ap.id = fs2.assigned_plane)
			where pt.id = schedule_template.plane_type_id and 
				ap.in_maintenence = false and 
				p.airline_id = air_id and 
				((fs2.departure_date < depart_datetime and fs2.arrival_date < arrival_datetime) or
				(fs2.departure_date is null and fs2.arrival_date is null))			
			order by random() limit 1;
		
		if plane_id is not null then		
			insert into flight_schedule(flight_number, segment_number, assigned_plane, departure_airport_id, arrival_airport_id, departure_date, arrival_date, departure_gate, arrival_gate, cancelled)
				values(schedule_template.flight_number, schedule_template.segment_number, 
					plane_id, schedule_template.departure_airport_id, schedule_template.arrival_airport_id, 
					depart_datetime, arrival_datetime, get_random_gate(), get_random_gate(), false);
		end if;
	end loop;
end;
$$;	

/* Functions */

create or replace function get_random_number(start_value integer, max_value integer)
	returns integer
	language plpgsql
	as
$$
begin
	return floor(random() * (max_value) + start_value)::int;
end;
$$;

create or replace function get_random_date(start_date date, time_frame interval) 
	returns date 
	language plpgsql 
	as
$$
begin 
	return start_date + (random() * (start_date+time_frame - start_date));
end;
$$;

create or replace function get_random_datetime(start_date timestamp, time_frame interval) 
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
	select upper(chr(int4(random()*25)+65)) into letter;
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

/* Triggers */

create or replace function check_overbooked()
	returns trigger
	language plpgsql
	as
$$
declare 
	overbook_capacity	float;
	current_capacity	int;
	p_type_id 			int;
begin 
	--get what the overbooking total should be for a flight reservation
	select (ptsc.capacity * 1.1) into overbook_capacity
		from flight_schedule fp
		inner join available_plane ap	
			on (ap.id = fp.assigned_plane)
		inner join plane p 
			on (p.id = ap.plane_id)
		inner join plane_type pt 
			on (pt.id = p.plane_type_id)
		inner join plane_type_seat_class ptsc 
			on (pt.id = ptsc.plane_type_id)
		where fp.id = new.flight_schedule_id and ptsc.seat_class_id = 3;
	
	--get the plane type id for the flight plan
	select p.plane_type_id into p_type_id
		from flight_reservation fr 
		inner join flight_schedule fp
			on (fr.flight_schedule_id = fp.id)
		inner join available_plane ap	
			on (ap.id = fp.assigned_plane)
		inner join plane p 
			on (p.id = ap.plane_id);
		
	--Get the current capacity of the flight reservation
	select count(*) into current_capacity
		from flight_reservation fr
		inner join seat_class sc 
			on (fr.class_id = sc.id)
		inner join plane_type_seat_class ptsc 
			on (sc.id = ptsc.seat_class_id)
		where fr.flight_schedule_id = new.flight_schedule_id 
			and ptsc.seat_class_id = 3 
			and ptsc.plane_type_id = p_type_id;
	if overbook_capacity - current_capacity = 0 then
		raise exception 'Flight is already overbooked' using errcode = 50003;
	else
		return new;	
	end if;
end;
$$;

create trigger flight_reservation_before_insert
	before insert 
	on flight_reservation
	for each row
	execute function check_overbooked();

create or replace function create_reservation_payment()
	returns trigger
	language plpgsql
	as
$$
begin
	insert into payment (staff_id, flight_reservation_id, payment_date, amount)
		values (1, new.id, new.reservation_date, new.seat_cost);
	return new;
end;
$$;

create trigger flight_reservation_after_insert
	after insert 
	on flight_reservation
	for each row
	execute function create_reservation_payment();
	
create or replace function check_plane_capacity()
	returns trigger 
	language plpgsql
	as
$$
declare 
	class_cap 		int;
	fp_id			int;
	p_type_id 		int;
	s_class_id  	int;
	current_cap 	int;
begin 
	
	--get the flight plan id from the booking
	select flight_schedule_id into fp_id
		from flight_reservation fr 
		inner join flight_schedule fp 
			on (fr.flight_schedule_id = fp.id)
		where fr.id = new.flight_reservation_id;
	
	--get the seat class id
	select fr.class_id into s_class_id
		from flight_reservation fr 
		inner join seat_class sc
			on (fr.class_id = sc.id)
		where fr.id = new.flight_reservation_id;
		
	--get the class capacity
	select ptsc.capacity into class_cap
		from flight_schedule fp
		inner join available_plane ap	
			on (ap.id = fp.assigned_plane)
		inner join plane p 
			on (p.id = ap.plane_id)
		inner join plane_type pt 
			on (pt.id = p.plane_type_id)
		inner join plane_type_seat_class ptsc 
			on (pt.id = ptsc.plane_type_id)
		where fp.id = fp_id and ptsc.seat_class_id = s_class_id;
	
	--get the current capacity on the plan for the class
	select count(*) into current_cap
		from flight_booking fb 
		inner join flight_reservation fr 
			on (fb.flight_reservation_id = fr.id) 
		inner join flight_schedule fp 
			on (fr.flight_schedule_id = fp.id)
		where fp.id = fp_id and fr.class_id = s_class_id;
	
	if s_class_id < 3 then
		return new;
	else 
		if current_cap < class_cap then
			return new;
		else
			raise exception 'Flight class full' using errcode = 50004;
		end if;
	end if;	
end;
$$;

create trigger flight_booking_before_insert
	before insert 
	on flight_booking
	for each row
	execute function check_plane_capacity();

create or replace function create_booking_refund_payment()
	returns trigger
	language plpgsql
	as 
$$
declare 
	class_cap 		int;
	fp_id			int;
	p_type_id 		int;
	s_class_id  	int;
	current_cap 	int;
begin 
	
	--get the flight plan id from the booking
	select flight_schedule_id into fp_id
		from flight_reservation fr 
		inner join flight_schedule fp 
			on (fr.flight_schedule_id = fp.id)
		where fr.id = new.flight_reservation_id;
	
	--get the seat class id
	select fr.class_id into s_class_id
		from flight_reservation fr 
		inner join seat_class sc
			on (fr.class_id = sc.id)
		where fr.id = new.flight_reservation_id;
		
	--get the class capacity
	select ptsc.capacity into class_cap
		from flight_schedule fp
		inner join available_plane ap	
			on (ap.id = fp.assigned_plane)
		inner join plane p 
			on (p.id = ap.plane_id)
		inner join plane_type pt 
			on (pt.id = p.plane_type_id)
		inner join plane_type_seat_class ptsc 
			on (pt.id = ptsc.plane_type_id)
		where fp.id = fp_id and ptsc.seat_class_id = s_class_id;
	
	--get the current capacity on the plan for the class
	select count(*) into current_cap
		from flight_booking fb 
		inner join flight_reservation fr 
			on (fb.flight_reservation_id = fr.id) 
		inner join flight_schedule fp 
			on (fr.flight_schedule_id = fp.id)
		where fp.id = fp_id and fr.class_id = s_class_id;
	
	if s_class_id < 3 then
		return new;
	else 
		if current_cap < class_cap then
			return new;
		else
			insert into payment (staff_id, flight_reservation_id, payment_date, amount)
				values (new.staff_id, new.flight_reservation_id, new.book_date, -200);
			return new;
		end if;
	end if;		
end;
$$;

create trigger flight_booking_after_insert
	after insert 
	on flight_booking
	for each row
	execute function create_booking_refund_payment();
			
create or replace function check_plane_availability()
	returns trigger
	language plpgsql
	as 
$$
declare 
	maintenence 	bool;
	flight_cursor 	cursor for
					select * 
						from flight_schedule f 
						where f.assigned_plane = new.assigned_plane;
	route_retired 	timestamp;
begin
	select ap.in_maintenence into maintenence
		from available_plane ap
		where plane_id = new.assigned_plane;
	if maintenence is false then 		
		for flight in flight_cursor loop 			
			if new.departure_date between flight.departure_date and flight.arrival_date then 
				raise exception 'Plane % already scheduled', new.assigned_plane using errcode = 50001;
			else 				
				select fr.date_retired into route_retired
					from flight_schedule_template fr 
					where fr.flight_number = new.flight_number and fr.segment_number = new.segment_number;
				if route_retired is null then
					return new;
				else
					raise exception 'Flight route retired' using errcode = 50002;
				end if;
			end if;			
		end loop;
	else 
		raise exception 'Plane % in maintenence', new.assigned_plane using errcode = 50001;
	end if;
	return new;
end;
$$;

create trigger flight_schedule_insert
	before insert
	on flight_schedule
	for each row 
	execute function check_plane_availability();

/* Populate all data */

create or replace procedure pop_all()
	language plpgsql
	as 
$$
begin 
	call pop_airline(3);
	call pop_staff(5);
	call pop_airport(10);
	call pop_plane_type(3);
	call pop_plane(10);
	call pop_available_plane();
	call pop_passenger(1000);
	call pop_seat_class(3);
	call pop_plane_type_seat_class(0);
	call pop_flight_schedule_template(10);
	call pop_flight_schedule(20, '-180 days');
	call pop_flight_schedule(10, '30 days');
	call pop_flight_seat_class(); 
	call pop_flight_reservation(0);
	call pop_flight_booking(0);
	call pop_passenger_manifest(0);
	call pop_flight_log();
end;
$$;
call pop_all();

select * from flight_schedule fs2;
select count(*) from flight_schedule fs3; 
select * from flight_log fl;
select * from flight_reservation fr; 
select * from flight_seat_class fsc; 

