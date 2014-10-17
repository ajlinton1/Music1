var songModel = {
	player: null,
	currentSong: null,
	songs: ko.observableArray(),
	playedSongs: ko.observableArray(),
	genres: ko.observableArray(),
	artists: ko.observableArray(),
	albums: ko.observableArray(),
	segments: ko.observableArray(),
	currentTitle: ko.observable(''),
	currentArtist: ko.observable(''),
	currentAlbum: ko.observable(''),
	currentGenre: ko.observable(''),
	currentLocation: ko.observable(''),
	currentRating: ko.observable(''),
	currentId: ko.observable(''),
	songIndex: 0,
	artistSkip: 0,
	albumSkip: 0,
	segmentSkip: 0,
	batchCount: 100,

	updateSong: function () {
		if (!songModel.currentSong) {
			return;
		}

		songModel.currentSong.Title = songModel.currentTitle();
		songModel.currentSong.Artist = songModel.currentArtist();
		songModel.currentSong.Album = songModel.currentAlbum();
		songModel.currentSong.Genre = songModel.currentGenre();
		songModel.currentSong.Rating = songModel.currentRating();

		var jString = JSON.stringify(songModel.currentSong);
		var url = "api/Song/" + songModel.currentSong.ID;
		$.ajax({
			type: 'POST',
			url: url,
			data: jString,
			contentType: 'application/json',
			success: function (response) {
				$('#statusDiv')[0].innerHTML = "Updated";
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	likeSong: function () {
		var song = songModel.playedSongs()[0];
		songModel.updateSongRating(1);
	},

	dislikeSong: function () {
		songModel.updateSongRating(-1);
	},

	updateSongRating: function (delta) {
		var currentRating = parseInt($('#ratingInput')[0].value);
		var newRating = songModel.currentRating() + delta;
		songModel.currentSong.RATING = newRating;
		songModel.currentSong.RatingView(newRating);
		songModel.currentRating(newRating);

		var jString = JSON.stringify(songModel.currentSong);

		var url = "api/Song/" + songModel.currentSong.ID;
		$.ajax({
			type: 'POST',
			url: url,
			data: jString,
			contentType: 'application/json',
			success: function (response) {
				$('#statusDiv')[0].innerHTML = "Updated";
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	songSorter: function (a, b) {
		return a.Location.localeCompare(b.Location);
	},

	loadSongs: function () {
		$('#statusDiv')[0].innerHTML = "Loading Songs";
		var url = "api/Song";

		var desiredSkip = $("#skipFilter")[0].value;
/*		if (!desiredSkip) {
			desiredSkip = 0;
		} */
		url = url + "?skip=" + desiredSkip;

/*		var desiredSequence = $("#sequenceFilter")[0].value;
		if (!desiredSequence) {
			desiredSequence = "Random";
		}
		url = url + "?" + "genre=" + desiredGenre + "&sequence=" + desiredSequence;
		url = url + "&skip=" + desiredSkip; */

		var desiredArtist = $("#desiredArtist")[0].value;
		url = url + "&artist=" + desiredArtist;

		var desiredGenre = $("#genreFilter")[0].value;
		url = url + "&genre=" + desiredGenre;

/*		var desiredLocation = $("#desiredLocation")[0].value;
		if (desiredLocation) {
			url = url + "&location=" + desiredLocation;
		} */

//		url = "api/Song?skip=0&artist=a&genre=b";

		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (songs) {
				if (songs.length == 0) {
					$('#statusDiv')[0].innerHTML = "No Results";
					return;
				}
				songModel.songs.removeAll();
				for (var i = 0; i < songs.length; i++) {
					songs[i].RatingView = new ko.observable(songs[i].Rating);
					songModel.songs.push(songs[i]);
				}
				$('#statusDiv')[0].innerHTML = "Songs Loaded";

				$('.songTitle').addClass("hover");

				songModel.songs.sort(songModel.songSorter);

				songModel.loadGenres();
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	loadSongsImp: function (genre, artist, album, segment) {
		$('#statusDiv')[0].innerHTML = "Loading Songs";
		var url = "api/Song";
		var desiredSequence = "Newest";
		url = url + "?sequence=" + desiredSequence;
		var desiredSkip = 0;
		url = url + "&skip=" + desiredSkip;

		if (artist) {
			url = url + "&artist=" + artist;
		}
		if (genre) {
			url = url + "&genre=" + genre;
		}
		if (album) {
			url = url + "&album=" + album;
		}
		if (segment) {
			url = url + "&location=" + segment;
		}


		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (songs) {
				songModel.songs.removeAll();
				for (var i = 0; i < songs.length; i++) {
					songs[i].RatingView = new ko.observable(songs[i].RATING);
					songModel.songs.push(songs[i]);
				}
				$('#statusDiv')[0].innerHTML = "Songs Loaded";

				$('.songTitle').addClass("hover");

			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	playSong: function () {
		$('#statusDiv')[0].innerHTML = "Playing";
		if (songModel.songs().length == 0) {
			$('#statusDiv')[0].innerHTML = "Done";

			if ((songModel.playedSongs().length> 0) && (songModel.playedSongs().length < 100)) {
				for (var i = songModel.playedSongs().length-1; i >= 0; i--) {
					songModel.songs.push(songModel.playedSongs()[i]);
				}
				songModel.playSong();
			} else {
				var currentSkip = $("#skipFilter")[0].value;
				currentSkip = currentSkip + 100;
				$("#skipFilter")[0].value = currentSkip;
				songModel.loadSongs();
				songModel.playSong();
			}

			return;
		}
		songModel.currentSong = songModel.songs.shift();
		songModel.playSingleSong(songModel.currentSong);
	},

	playSingleSong: function (song) {
		songModel.playedSongs.unshift(song);
		//		var songUrl = song.DownloadUrl;

		var songUrl = "https://googledrive.com/host/";
		if (song.GDriveId) {
			songUrl = songUrl + song.GDriveId;
		} else {
			songUrl = songUrl + song.RowKey;
		}

		var player = $('#player1').get(0);
		songModel.player.src = songUrl;
		songModel.player.play();

		songModel.currentArtist(song.Artist);
		songModel.currentGenre(song.Genre);
		songModel.currentAlbum(song.Album);
		songModel.currentId(song.Id);
		songModel.currentLocation(song.Location);
		songModel.currentRating(song.Rating);
		songModel.currentTitle(song.Title);
	},

	next: function () {
		songModel.playSong();
	},

	previous: function () {
		if (songModel.playedSongs().length < 2) {
			return;
		}
		var previousSong = songModel.playedSongs()[songModel.playedSongs().length - 2];
		songModel.songs.push(songModel.currentSong);
		songModel.playSingleSong(previousSong);
	},

	loadGenres: function () {
		var url = "api/Genre";
		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (genres) {
				songModel.genres.removeAll();
				for (var i = 0; i < genres.length; i++) {
					songModel.genres.push(genres[i]);
				}
//				songModel.loadArtists();
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	loadArtists: function (skip) {
		var url = "api/Artist?";
		if (skip) {
			songModel.artistSkip = songModel.artistSkip + skip;
			url = url + "skip=" + songModel.artistSkip + "&";
		}

		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (artists) {
				songModel.artists.removeAll();
				for (var i = 0; i < artists.length; i++) {
					var artist = { 'name': artists[i].m_Item1, 'number': artists[i].m_Item2 };
					songModel.artists.push(artist);
				}
				if (!skip) {
					songModel.loadAlbums();
				}
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	loadAlbums: function (skip) {
		var url = "api/Album?";
		if (skip) {
			songModel.albumSkip = songModel.albumSkip + skip;
			url = url + "skip=" + songModel.albumSkip + "&";
		}
		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (albums) {
				songModel.albums.removeAll();
				for (var i = 0; i < albums.length; i++) {
					var album = { 'name': albums[i] };
					songModel.albums.push(album);
				}
				if (!skip) {
					songModel.loadSegments();
				}
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	loadSegments: function (skip) {
		var url = "api/Segment?";
		if (skip) {
			songModel.segmentSkip = songModel.segmentSkip + skip;
			url = url + "skip=" + songModel.segmentSkip + "&";
		}
		$.ajax({
			type: 'get',
			url: url,
			dataType: 'json',
			success: function (segments) {
				songModel.segments.removeAll();
				for (var i = 0; i < segments.length; i++) {
					var segment = { genre: segments[i].Genre, segmentAll: segments[i].SegmentAll, segment0: segments[i].Segment0, segment1: segments[i].Segment1, segment2: segments[i].Segment2, segment3: segments[i].Segment3, segment4: segments[i].Segment4, segment5: segments[i].Segment5 };
					songModel.segments.push(segment);
				}
			},
			error: function (xhr, textStatus, errorThrown) {
				debugger;
				$('#statusDiv')[0].innerHTML = "Error";
			}
		});
	},

	loadArtistsPrevious: function () {
		songModel.loadArtists(-songModel.batchCount);
	},

	loadArtistsNext: function () {
		songModel.loadArtists(songModel.batchCount);
	},

	loadAlbumsPrevious: function () {
		songModel.loadAlbums(-songModel.batchCount);
	},

	loadAlbumsNext: function () {
		songModel.loadAlbums(songModel.batchCount);
	}

};

function songClicked(e) {
	var id = e.ID;
	songModel.player = $('#player1').get(0);
	songModel.player.src = e.DownloadUrl;
	songModel.player.play();

	songModel.currentSong = e;
	songModel.playedSongs.push(songModel.currentSong);
	songModel.currentArtist(e.Artist);
	songModel.currentAlbum(e.Album);
	songModel.currentGenre(e.Genre);
	songModel.currentLocation(e.Location);
	songModel.currentRating(e.Rating);
	songModel.currentTitle(e.Title);

	var songsToMove = [];
	var songIndex = 0;
	for (var i = 0; i < songModel.songs().length; i++) {
		if (songModel.songs()[songIndex].Id == id) {
			songModel.songIndex = i;
			break;
		} else {
			songsToMove.push(songModel.songs()[songIndex]);
		}
		songIndex++;
	}
	songModel.songs.splice(0, songsToMove.length + 1);
	for (var i = 0; i < songsToMove.length; i++) {
		songModel.songs.push(songsToMove[i]);
	}
}

function artistClicked(e) {
	var desiredArtist = e.Artist;
	var matchingSongs = [];
	var nonMatchingSongs = [];
	var songIndex = 0;
	for (var i = 0; i < songModel.songs().length; i++) {
		if (songModel.songs()[songIndex].Artist == desiredArtist) {
			matchingSongs.push(songModel.songs()[songIndex]);
		} else {
			nonMatchingSongs.push(songModel.songs()[songIndex]);
		}
		songIndex++;
	}
	songModel.songs.removeAll();
	for (var i = 0; i < matchingSongs.length; i++) {
		songModel.songs.push(matchingSongs[i]);
	}
	for (var i = 0; i < nonMatchingSongs.length; i++) {
		songModel.songs.push(nonMatchingSongs[i]);
	}
	songClicked(songModel.songs()[0]);
}

function genreClicked(e) {
	$("#genreFilter")[0].value = e.Name;
	songModel.loadSongs();
}

function artistListClicked(e) {
	songModel.loadSongsImp('', e.name);
}

function albumClicked(e) {
	songModel.loadSongsImp('', '', e.name);
}

function segmentClicked(e) {
	songModel.loadSongsImp('', '', '', e.name);
}

$(document).ready(function () {

	ko.applyBindings(songModel);

	songModel.player = $('#player1').get(0);

	songModel.player.onended = function () {
		songModel.playSong();
	}

	songModel.player.onerror = function () {
		songModel.playSong();
	}

	$('#updateLink')[0].onclick = songModel.updateSong;
	$('#likeLink')[0].onclick = songModel.likeSong;
	$('#dislikeLink')[0].onclick = songModel.dislikeSong;

	$('#tabs').tabs();

	$("#buttonPlay").button();
	$("#buttonNext").button();
	$("#buttonPrevious").button();
	$('#buttonPlay')[0].onclick = songModel.playSong;
	$('#buttonNext')[0].onclick = songModel.next;
	$('#buttonPrevious')[0].onclick = songModel.previous;

	$("#buttonSearchSongs").button();
	$('#buttonSearchSongs')[0].onclick = songModel.loadSongs;

	$("#buttonArtistsPrevious").button();
	$('#buttonArtistsPrevious')[0].onclick = songModel.loadArtistsPrevious;
	$("#buttonArtistsNext").button();
	$('#buttonArtistsNext')[0].onclick = songModel.loadArtistsNext;

	$("#buttonAlbumsPrevious").button();
	$('#buttonAlbumsPrevious')[0].onclick = songModel.loadAlbumsPrevious;
	$("#buttonAlbumsNext").button();
	$('#buttonAlbumsNext')[0].onclick = songModel.loadAlbumsNext;

	songModel.loadSongs();
});


