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
		from flight_plan fp
		inner join plane p 	
			on (p.id = fp.plane_id)
		inner join plane_type pt 
			on (pt.id = p.plane_type_id)
		inner join plane_type_seat_class ptsc 
			on (pt.id = ptsc.plane_type_id)
		where fp.id = new.flight_plan_id and ptsc.seat_class_id = 3;
	
	--get the plane type id for the flight plan
	select p.plane_type_id into p_type_id
		from flight_reservation fr 
		inner join flight_plan fp
			on (fr.flight_plan_id = fp.id)
		inner join plane p 
			on (fp.plane_id = p.id);
	
	--Get the current capacity of the flight reservation
	select count(*) into current_capacity
		from flight_reservation fr
		inner join seat_class sc 
			on (fr.class_id = sc.id)
		inner join plane_type_seat_class ptsc 
			on (sc.id = ptsc.seat_class_id)
		where fr.flight_plan_id = new.flight_plan_id 
			and ptsc.seat_class_id = 3 
			and ptsc.plane_type_id = p_type_id;
	if overbook_capacity - current_capacity = 0 then
		raise exception 'Flight is already overbooked';
		return null;
	else
		return new;	
	end if;
end;
$$;

create trigger CheckOverbooking
	before insert 
	on flight_reservation
	for each row
	execute function check_overbooked();

insert into flight_reservation (passenger_id, flight_plan_id, class_id, reservation_date, seat_cost)
	values (2, 1, 3, '10/11/2022 10:00', 60);

select * from flight_reservation;

/* covers adding a new reservation only if flight isn't over booked
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
31|           2|             1|       3|      2022-10-11|       60|
 */

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
	select flight_plan_id into fp_id
		from flight_reservation fr 
		inner join flight_plan fp 
			on (fr.flight_plan_id = fp.id)
		where fr.id = new.flight_reservation_id;
	
	--get the seat class id
	select fr.class_id into s_class_id
		from flight_reservation fr 
		inner join seat_class sc
			on (fr.class_id = sc.id)
		where fr.id = new.flight_reservation_id;
		
	--get the class capacity
	select ptsc.capacity into class_cap
		from flight_plan fp
		inner join plane p 	
			on (p.id = fp.plane_id)
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
		inner join flight_plan fp 
			on (fr.flight_plan_id = fp.id)
		where fp.id = fp_id and fr.class_id = s_class_id;
	
	if s_class_id < 3 then
		return new;
	else 
		if current_cap < class_cap then
			return new;
		else
			raise exception 'Flight class full';
			return null;
		end if;
	end if;	
end;
$$;

create trigger CheckPlaneCapacity
	before insert 
	on flight_booking
	for each row
	execute function check_plane_capacity();

insert into flight_booking (flight_reservation_id, book_date, staff_id)
	values (31, '10/12/2022 12:00', 1);

select * from flight_booking;

/* Adds reservation to booking if the flight isn't full
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
31|                   31|2022-10-12|       1|
 */

create or replace function calculate_seats_sold(plan_id integer)
	returns float 
	language plpgsql
	as
$$
declare 
	seats_sold	float;
	plane_cap 	float;
begin
	
	select count(fr.class_id) into seats_sold
		from flight_reservation fr
		where fr.flight_plan_id = plan_id;
	
	select sum(ptsc.capacity) into plane_cap
		from flight_plan fp 
		inner join plane p 
			on (fp.plane_id = p.id)
		inner join plane_type pt 
			on (p.plane_type_id = pt.id)
		inner join plane_type_seat_class ptsc  
			on (ptsc.plane_type_id = pt.id)
		where fp.id = plan_id;
	
	return (seats_sold/plane_cap) * 100;
end;
$$;

select calculate_seats_sold(1);
/*
calculate_seats_sold|
--------------------+
 0.13333333333333333|
 */
	
create or replace function calculate_overbooking_payment(plan_id integer)
	returns float 
	language plpgsql
	as
$$
declare 
	reservation_num		float;
	plane_cap 			float;
	overbook_total		float;
begin
	
	select count(*) into overbook_total
		from payment p 
		inner join flight_reservation fr 
			on (p.flight_reservation_id = fr.id)
		where fr.flight_plan_id = plan_id and amount = -200;
	
	select sum(ptsc.capacity) into plane_cap
		from flight_plan fp 
		inner join plane p 
			on (fp.plane_id = p.id)
		inner join plane_type pt 
			on (p.plane_type_id = pt.id)
		inner join plane_type_seat_class ptsc  
			on (ptsc.plane_type_id = pt.id)
		where fp.id = plan_id;
	
	select count(*) into reservation_num
		from flight_reservation fr 
		where fr.flight_plan_id = 1;
	if plane_cap - reservation_num > 0 then
		return 0;
	else
		return (1-(reservation_num/plane_cap)) * 100;
	end if;
end;
$$;	

select calculate_overbooking_payment(1);
/*
calculate_overbooking_payment|
-----------------------------+
        				  0.0|
 */


		 
		

		
	
