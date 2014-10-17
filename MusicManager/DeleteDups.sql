select count(*) from song

select distinct hash from song

select * from song_history

delete from song_history

delete from song where id in (
select id from song s1 
where exists (select * from song s2 where s1.hash = s2.hash and s1.id<>s2.id))

delete from song where id in (
select id from song s1 
where exists (select * from song s2 where s1.location = s2.location and s1.id<>s2.id)
)
