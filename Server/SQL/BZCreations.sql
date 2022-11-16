set search_path to public;
drop schema if exists zack_bridger cascade;
create schema zack_bridger;
set search_path to zack_bridger;

drop table if exists passenger_manifest;
drop table if exists payment;
drop table if exists flight_booking;
drop table if exists flight_reservation;
drop table if exists available_plane;
drop table if exists flight_route;
drop table if exists passenger;
drop table if exists plane_type_seat_class;
drop table if exists seat_class;
drop table if exists flight_log;
drop table if exists flight_schedule;
drop table if exists flight_schedule_log;
drop table if exists airport;
drop table if exists plane;
drop table if exists plane_type;
drop table if exists airline;
drop table if exists staff;

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
	date_retired			timestamp,
	check (depart_airport_id <> arrival_airport_id)
);

create table flight_schedule (
	id 						serial primary key,
	flight_route_id			int references flight_route(id) not null,
	available_plane_id		int references available_plane(id) not null,
	departure_date			timestamp not null,
	arrival_date			timestamp not null,
	departure_gate			varchar(5) not null,
	arrival_gate			varchar(5) not null,
	cancelled				bool not null
);

create table flight_schedule_log (
	id 					serial primary key,
	available_plane_id	int references available_plane(id) not null,
	departure_date		timestamp not null,
	arrival_date		timestamp not null,
	departure_gate		varchar(5) not null,
	arrival_gate		varchar(5) not null,
	flight_schedule_id	int references flight_schedule(id) not null
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
	arrival_date			timestamp check (arrival_date > depart_date)
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
		for flight_plan_id in select id from flight_plan loop
			for class_id in select id from seat_class loop
				insert into flight_reservation (passenger_id, flight_plan_id, class_id, reservation_date, seat_cost)
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

create or replace procedure pop_available_plane(amount integer)
	language plpgsql
	as 
$$
declare 
	plane_id	int;
begin 
	for i in 1..amount loop
		for plane_id in select id from plane loop
			insert into available_plane (plane_id, in_maintenence)
				values (plane_id, get_random_bool());
		end loop;		
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
	curs cursor for select flight_booking.id as id, flight_plan.departure_date as dep_date from flight_booking 
		inner join flight_reservation
			on (flight_reservation.id = flight_booking.flight_reservation_id)
		inner join flight_plan
			on (flight_plan.id = flight_reservation.flight_plan_id);
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
begin
    for flight in flight_past_cursor loop
        insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        values (flight.id, get_random_date(flight.depart_date, '10 hours'), get_random_date(flight.arrival, '10 hours'), 
       		flight.departure_airport_id, flight.arrival_airport_id);
   	end loop;
   	commit;
   	for flight in flight_future_cursor loop
        insert into flight_log (flight_schedule_id, depart_date, arrival_date, depart_airport_id, arrival_airport_id)
        values (flight.id, get_random_date(flight.depart_date, '10 hours'), null, 
       		flight.departure_airport_id, null);
    end loop;
   	commit;
end;    
$$;

create or replace procedure pop_flight_route(amount integer)
	language plpgsql
	as 
$$
declare 
	max_airports	int;
	start_date		timestamp;
	airline_id		int;	
begin
	select count(*) into max_airports from airport;
	for i in 1..amount loop
		for airline_id in select id from airline loop
			start_date := get_random_date(now()::timestamp, '365 days');
			insert into flight_route (depart_airport_id, arrival_airport_id, date_created, date_retired, airline_id)
				values (get_random_number(max_airports), get_random_number(max_airports), start_date, null, airline_id);			
		end loop;	
	end loop;
	commit;
	exception
		when check_violation then
			raise notice 'Depart and arrival same value';	
end;
$$;

create or replace function get_random_number(max_value integer)
	returns integer
	language plpgsql
	as
$$
begin
	return floor(random() * (max_value - 1) + 1)::int;
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
	return random() > 0.9;
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
	call pop_available_plane(5);
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
select * from flight_route fr 
call pop_all();

select * from passenger_manifest;
/*
id|flight_booking_id|boarding_date|staff_id|
--+-----------------+-------------+--------+
 1|                1|   2022-11-10|       2|
 2|                2|   2022-11-10|       2|
 3|                3|   2022-11-10|       2|
 4|                4|   2022-11-10|       2|
 5|                5|   2022-11-10|       2|
 6|                6|   2022-11-10|       2|
 7|                7|   2022-11-10|       2|
 8|                8|   2022-11-10|       2|
 9|                9|   2022-11-10|       2|
10|               10|   2022-11-10|       2|
11|               11|   2022-11-10|       2|
12|               12|   2022-11-10|       2|
13|               13|   2022-11-10|       2|
14|               14|   2022-11-10|       2|
15|               15|   2022-11-10|       2|
16|               16|   2022-11-10|       2|
17|               17|   2022-11-10|       2|
18|               18|   2022-11-10|       2|
19|               19|   2022-11-10|       2|
20|               20|   2022-11-10|       2|
21|               21|   2022-11-10|       2|
22|               22|   2022-11-10|       2|
23|               23|   2022-11-10|       2|
24|               24|   2022-11-10|       2|
25|               25|   2022-11-10|       2|
26|               26|   2022-11-10|       2|
27|               27|   2022-11-10|       2|
28|               28|   2022-11-10|       2|
29|               29|   2022-11-10|       2|
30|               30|   2022-11-10|       2|
 */

select * from payment;
/*
id|staff_id|flight_reservation_id|payment_date|amount|
--+--------+---------------------+------------+------+
 1|       1|                    1|  2022-11-11|    60|
 2|       1|                    2|  2022-11-11|    60|
 3|       1|                    3|  2022-11-11|    60|
 4|       1|                    4|  2022-11-11|    60|
 5|       1|                    5|  2022-11-11|    60|
 6|       1|                    6|  2022-11-11|    60|
 7|       1|                    7|  2022-11-11|    60|
 8|       1|                    8|  2022-11-11|    60|
 9|       1|                    9|  2022-11-11|    60|
10|       1|                   10|  2022-11-11|    60|
11|       1|                   11|  2022-11-11|    60|
12|       1|                   12|  2022-11-11|    60|
13|       1|                   13|  2022-11-11|    60|
14|       1|                   14|  2022-11-11|    60|
15|       1|                   15|  2022-11-11|    60|
16|       1|                   16|  2022-11-11|    60|
17|       1|                   17|  2022-11-11|    60|
18|       1|                   18|  2022-11-11|    60|
19|       1|                   19|  2022-11-11|    60|
20|       1|                   20|  2022-11-11|    60|
21|       1|                   21|  2022-11-11|    60|
22|       1|                   22|  2022-11-11|    60|
23|       1|                   23|  2022-11-11|    60|
24|       1|                   24|  2022-11-11|    60|
25|       1|                   25|  2022-11-11|    60|
26|       1|                   26|  2022-11-11|    60|
27|       1|                   27|  2022-11-11|    60|
28|       1|                   28|  2022-11-11|    60|
29|       1|                   29|  2022-11-11|    60|
30|       1|                   30|  2022-11-11|    60|
31|       1|                    1|  2022-11-12|  -200|
 */

select * from flight_booking;
/*
id|flight_reservation_id|book_date |staff_id|
--+---------------------+----------+--------+
 1|                    1|2022-11-10|       1|
 2|                    2|2022-11-10|       1|
 3|                    3|2022-11-10|       1|
 4|                    4|2022-11-10|       1|
 5|                    5|2022-11-10|       1|
 6|                    6|2022-11-10|       1|
 7|                    7|2022-11-10|       1|
 8|                    8|2022-11-10|       1|
 9|                    9|2022-11-10|       1|
10|                   10|2022-11-10|       1|
11|                   11|2022-11-10|       1|
12|                   12|2022-11-10|       1|
13|                   13|2022-11-10|       1|
14|                   14|2022-11-10|       1|
15|                   15|2022-11-10|       1|
16|                   16|2022-11-10|       1|
17|                   17|2022-11-10|       1|
18|                   18|2022-11-10|       1|
19|                   19|2022-11-10|       1|
20|                   20|2022-11-10|       1|
21|                   21|2022-11-10|       1|
22|                   22|2022-11-10|       1|
23|                   23|2022-11-10|       1|
24|                   24|2022-11-10|       1|
25|                   25|2022-11-10|       1|
26|                   26|2022-11-10|       1|
27|                   27|2022-11-10|       1|
28|                   28|2022-11-10|       1|
29|                   29|2022-11-10|       1|
30|                   30|2022-11-10|       1|
31|                   30|2022-10-12|       1|
34|                   34|2022-10-12|       1|
 */

select * from flight_reservation;
/*
id|passenger_id|flight_plan_id|class_id|reservation_date|seat_cost|
--+------------+--------------+--------+----------------+---------+
 1|           1|             1|       1|      2022-10-10|       60|
 2|           1|             1|       2|      2022-10-10|       60|
 3|           1|             1|       3|      2022-10-10|       60|
 4|           1|             2|       1|      2022-10-10|       60|
 5|           1|             2|       2|      2022-10-10|       60|
 6|           1|             2|       3|      2022-10-10|       60|
 7|           2|             1|       1|      2022-10-10|       60|
 8|           2|             1|       2|      2022-10-10|       60|
 9|           2|             1|       3|      2022-10-10|       60|
10|           2|             2|       1|      2022-10-10|       60|
11|           2|             2|       2|      2022-10-10|       60|
12|           2|             2|       3|      2022-10-10|       60|
13|           3|             1|       1|      2022-10-10|       60|
14|           3|             1|       2|      2022-10-10|       60|
15|           3|             1|       3|      2022-10-10|       60|
16|           3|             2|       1|      2022-10-10|       60|
17|           3|             2|       2|      2022-10-10|       60|
18|           3|             2|       3|      2022-10-10|       60|
19|           4|             1|       1|      2022-10-10|       60|
20|           4|             1|       2|      2022-10-10|       60|
21|           4|             1|       3|      2022-10-10|       60|
22|           4|             2|       1|      2022-10-10|       60|
23|           4|             2|       2|      2022-10-10|       60|
24|           4|             2|       3|      2022-10-10|       60|
25|           5|             1|       1|      2022-10-10|       60|
26|           5|             1|       2|      2022-10-10|       60|
27|           5|             1|       3|      2022-10-10|       60|
28|           5|             2|       1|      2022-10-10|       60|
29|           5|             2|       2|      2022-10-10|       60|
30|           5|             2|       3|      2022-10-10|       60|
 */

select * from maintenance;
/*
id|plane_id|start_date|end_date  |description        |
--+--------+----------+----------+-------------------+
 1|       2|2022-11-10|2022-11-11|General Maintenance|
 */

select * from current_flight;
/*
id|flight_plan_id|take_off_airport_id|landing_airport_id|take_off_time|landing_time|
--+--------------+-------------------+------------------+-------------+------------+
 1|             1|                  2|                 1|   2022-11-10|            |
 2|             2|                  3|                 1|   2022-11-10|            |
 */

select * from passenger;
/*
id|name      |
--+----------+
 1|Passenger1|
 2|Passenger2|
 3|Passenger3|
 4|Passenger4|
 5|Passenger5|
 */

select * from plane_type_seat_class;
/*
id|plane_type_id|seat_class_id|capacity|
--+-------------+-------------+--------+
 1|            1|            1|      40|
 2|            1|            2|      40|
 3|            1|            3|      40|
 4|            2|            1|      40|
 5|            2|            2|      40|
 6|            2|            3|      40|
 7|            3|            1|      40|
 8|            3|            2|      40|
 9|            3|            3|      40|
 */

select * from seat_class;
/*
id|name  |
--+------+
 1|Class1|
 2|Class2|
 3|Class3|
 */

select * from flight_plan;
/*
id|arrival_airport_id|departure_airport_id|plane_id|departure_date|arrival_date|departure_gate|arrival_gate|
--+------------------+--------------------+--------+--------------+------------+--------------+------------+
 1|                 1|                   2|       1|    2022-11-10|  2022-11-10|A1            |B2          |
 2|                 1|                   3|       2|    2022-11-10|  2022-11-11|C4            |A2          |
 */

select * from flight_plan_log;
/*
id|plane_id|departure_date|arrival_date|departure_gate|arrival_gate|new_plane_id|new_departure_date|new_arrival_date|new_departure_gate|new_arrival_gate|
--+--------+--------------+------------+--------------+------------+------------+------------------+----------------+------------------+----------------+
 1|       3|    2022-11-10|  2022-11-10|A1            |B2          |           1|                  |                |                  |                |
 2|       2|    2022-11-10|  2022-11-11|B6            |A2          |            |                  |                |C4                |                |
 */

select * from airport;
/*
id|name    |address |
--+--------+--------+
 1|Airport1|Address1|
 2|Airport2|Address2|
 3|Airport3|Address3|
 */

select * from plane;
/*
id|plane_type_id|airline_id|
--+-------------+----------+
 1|            1|         1|
 2|            1|         1|
 3|            1|         1|
 4|            1|         1|
 5|            1|         1|
 6|            2|         1|
 7|            2|         1|
 8|            2|         1|
 9|            2|         1|
10|            2|         1|
11|            3|         1|
12|            3|         1|
13|            3|         1|
14|            3|         1|
15|            3|         1|
16|            1|         2|
17|            1|         2|
18|            1|         2|
19|            1|         2|
20|            1|         2|
21|            2|         2|
22|            2|         2|
23|            2|         2|
24|            2|         2|
25|            2|         2|
26|            3|         2|
27|            3|         2|
28|            3|         2|
29|            3|         2|
30|            3|         2|
31|            1|         3|
32|            1|         3|
33|            1|         3|
34|            1|         3|
35|            1|         3|
36|            2|         3|
37|            2|         3|
38|            2|         3|
39|            2|         3|
40|            2|         3|
41|            3|         3|
42|            3|         3|
43|            3|         3|
44|            3|         3|
45|            3|         3|
 */

select * from plane_type;
/*
id|name |
--+-----+
 1|Type1|
 2|Type2|
 3|Type3|
 */

select * from airline;
/*
id|name    |
--+--------+
 1|Airline1|
 2|Airline2|
 3|Airline3|
 */

select * from staff;
/*
id|name  |
--+------+
 1|Staff1|
 2|Staff2|
 3|Staff3|
 4|Staff4|
 5|Staff5|
 */






