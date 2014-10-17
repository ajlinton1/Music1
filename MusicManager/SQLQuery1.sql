select top 100 * from song

select * from song where location like '%Music\101 ru Radio\Genesis - The Spark Of Life - 0;00.mp3'

select top 10 * from song order by id desc

select * from song where location is null

delete from song where location is null

delete from dbo.SONG_HISTORY

select * from song_rated where song_id not in
(select id from song)

select * from song where id not in
(select song_id from song_rated)