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
			on (ap.id = fp.available_plane_id)
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
			on (ap.id = fp.available_plane_id)
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
		raise exception 'Flight is already overbooked';
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

insert into flight_reservation (passenger_id, flight_schedule_id, class_id, reservation_date, seat_cost)
	values (2, 1, 3, '10/11/2022 10:00', 60);

select * from flight_reservation;

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
			on (ap.id = fp.available_plane_id)
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
			raise exception 'Flight class full';
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
		where fr.flight_schedule_id = plan_id;
	
	select sum(ptsc.capacity) into plane_cap
		from flight_schedule fp 
		inner join available_plane ap	
			on (ap.id = fp.available_plane_id)
		inner join plane p 
			on (p.id = ap.plane_id)
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
		where fr.flight_schedule_id = plan_id and amount = -200;
	
	select sum(ptsc.capacity) into plane_cap
		from flight_schedule fp 
		inner join available_plane ap	
			on (ap.id = fp.available_plane_id)
		inner join plane p 
			on (p.id = ap.plane_id)
		inner join plane_type pt 
			on (p.plane_type_id = pt.id)
		inner join plane_type_seat_class ptsc  
			on (ptsc.plane_type_id = pt.id)
		where fp.id = plan_id;
	
	select count(*) into reservation_num
		from flight_reservation fr 
		where fr.flight_schedule_id = 1;
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


		 
		

		
	
