create or replace function check_current_flight_status(flight_log_id integer)
	returns interval
	language plpgsql
	as 
$$
declare 
	arrival_time	timestamp;
begin 
	select f.arrival_date into arrival_time
	from flight_log fl
	inner join flight_schedule f
		on (f.id = fl.flight_schedule_id)
	where fl.id = flight_log_id;
	
	return arrival_time - now()::timestamp;
end;
$$;

select check_current_flight_status(33);

create or replace function check_scheduled_flight_status(flight_schedule_id integer)
	returns text 
	language plpgsql
	as 
$$
declare 
	flight_cancelled 	bool;
	depart_time			timestamp;
begin
	select f.cancelled, f.departure_date into flight_cancelled, depart_time
	from flight_schedule f
	where f.id = flight_schedule_id;

	if flight_cancelled then
		return 'Cancelled';
	elsif check_flight_delayed(flight_schedule_id) then
		return 'Delayed';
	end if;
	return 'On time';
end;
$$;

create or replace function check_flight_delayed(flight_sched_id integer)
	returns boolean
	language plpgsql
	as 
$$
declare 
	take_off_time 	timestamp;
	land_time		timestamp;
	sched_depart_time	timestamp;
	sched_arr_time		timestamp;
	grace_period	interval = '30 minutes';
begin 
	select fl.depart_date, fl.arrival_date into take_off_time, land_time
	from flight_log fl 
	where fl.flight_schedule_id = flight_sched_id ;

	select f.departure_date, f.arrival_date into sched_depart_time, sched_arr_time
	from flight_schedule f
	where f.id = flight_sched_id;

	if take_off_time is null then
		if now()::timestamp > (sched_depart_time + grace_period) then 
			raise notice 'Flight has not departed at its scheduled time.';
			return true;
		end if;
	elsif land_time is null and (sched_arr_time + grace_period) < now() then 
		raise notice 'Flight is going to land late';
		return true;
	elsif take_off_time > (sched_depart_time + grace_period) then 
		raise notice 'Flight took off late';
		return true;
	elsif land_time is not null and land_time > (sched_arr_time + grace_period) then 
		raise notice 'Flight landed late';
		return true;	
	end if;
	return false;
end;
$$;

select check_scheduled_flight_status(67);
select check_scheduled_flight_status(1);
select check_scheduled_flight_status(64);