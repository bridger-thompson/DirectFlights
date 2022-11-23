create or replace view flight_details 
as
select a.name as airline, a1.name as from_airport, fs1.departure_date, a2.name as to_airport, fs1.arrival_date, sc.name as seat_name, fsc.suggested_cost as seat_cost
	from flight_schedule fs1
	left join airport a1 
		on (fs1.departure_airport_id = a1.id)
	left join airport a2
		on (fs1.arrival_airport_id = a2.id)
	left join flight_seat_class fsc 
		on (fs1.id = fsc.flight_id)
	left join seat_class sc 
		on (fsc.seat_id = sc.id)
	left join available_plane ap 
		on (fs1.assigned_plane = ap.id)
	left join plane p
		on (ap.plane_id = p.id)
	left join airline a 
		on (p.airline_id = a.id);

select * from flight_details fd
	inner join airline a 
		on (a.name = fd.airline)
	where a.id = 10;

create or replace view flight_schedule_template_options
as
select distinct a1.name as departure_airport, a2.name as arrival_airport, a3.name as airline, pt.name as plane_type
	from flight_schedule_template fst 
	right join airport a1
		on (fst.departure_airport_id = a1.id)
	right join airport a2 
		on (fst.arrival_airport_id = a2.id)
	right join airline a3 
		on (fst.airline_id = a3.id)
	right join plane_type pt 
		on (fst.plane_type_id = pt.id)
	order by 1,2,3,4;

select * from flight_schedule_template_options;

create or replace view flight_total
as 
with flight_total as ( 
	select p.id, p.flight_reservation_id, sum(p.amount) as total
		from payment p 
		where p.amount > -200
		group by 1,2
		order by 1,2		
), flight_refund as ( 
	select p.id, p.flight_reservation_id, sum(p.amount) as refund
		from payment p 
		where p.amount <= -200
		group by 1,2
		order by 1,2
)
select fs2.flight_number, fs2.departure_date, ft.total, fr.refund, ft.total + fr.refund as profit
	from flight_total ft
	inner join flight_refund fr 
		on (ft.flight_reservation_id = fr.flight_reservation_id)
	inner join flight_reservation fr2 
		on (fr2.id = ft.flight_reservation_id)
	inner join flight_schedule fs2 
		on (fr2.flight_schedule_id = fs2.id)
	order by 1;

create or replace function flight_total_with_id(flight_id integer, flight_day timestamp)
returns table (
	flight 			integer,
	departure_date	timestamp,
	total			numeric,
	refund			numeric,
	profit			numeric
)
language plpgsql
as $$
begin 	
	return query 
		select ft.flight_number as flight, ft.departure_date, ft.total, ft.refund, ft.profit
			from flight_total ft
			where ft.flight_number = flight_id and 
			ft.departure_date = flight_day;
end;
$$;

select * from flight_total_with_id(1, '2023-03-05 00:00:00.000')

create or replace function airline_total_with_id(a_id integer, flight_day timestamp)
returns table (
	flight 			integer,
	departure_date	timestamp,
	total			numeric,
	refund			numeric,
	profit			numeric
)
language plpgsql
as $$
begin 	
	return query 
		select distinct ft.flight_number as flight, ft.departure_date, ft.total, ft.refund, ft.profit
			from flight_total ft
			left join flight_schedule fs1
				on (fs1.flight_number = ft.flight_number)
			left join available_plane ap 
				on (fs1.assigned_plane = ap.id)
			left join plane p
				on (ap.plane_id = p.id)
			left join airline a 
				on (p.airline_id = a.id)
			where a.id = a_id and 
			ft.departure_date = flight_day;
end;
$$;

select * from airline_total_with_id(8, '2022-10-17 01:00:00.000')

