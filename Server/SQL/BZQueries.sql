



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


		 
		

		
	
