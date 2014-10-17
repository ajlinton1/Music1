select count(*) from song

select * from song where location is null

delete from song where location is null

select count(*) from song_history

select * from song_history sh
join song s on s.id = sh.song_id

select count(*) from song_rated

select sr.id from song_rated sr
left join song s on s.id = sr.song_id
where s.id is null

delete from song_rated 
where id not in (select id from song)

delete from song_history where song_ID in 
(select id from song where location is null)