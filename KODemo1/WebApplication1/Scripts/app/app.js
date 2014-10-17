window.kodemoApp = {};

// Model
(function (app) {

	function Song() {
		var self = this;
		self.title = ko.observable("");
		self.artist = ko.observable("");
		self.album = ko.observable("");
		self.genre = ko.observable("");
		self.rating = ko.observable(0);
		self.url = ko.observable("");
	}

	// add to our namespace
	app.Song = Song;

}(window.kodemoApp));

// ViewModel
var viewModel = {

	selectedSong : ko.observable(),
	songCollection: ko.observableArray([]),
	searchCriteriaGenre: ko.observable(''),

	playSong: function () {
		if ((!viewModel.songCollection().length) || (!viewModel.selectedSong())) {
			return;
		}
		var song = viewModel.selectedSong();
		var songUrl = song.url();
		var player = $('#player').get(0);
		player.src = songUrl;
		player.play();
	},

	selectSong:function (song) {
		viewModel.selectedSong(song);
	},

	loadSongs: function () {
		// TODO: Use real repository UI development done
		viewModel.repository = new kodemoApp.MockRepository();
		var songs = viewModel.repository.getSongs();
		viewModel.songCollection(songs);
	},

	searchSongs: function () {
		// Get genre from bound UI elemen
		var genre = viewModel.searchCriteriaGenre();
		if (!genre) {
			return;
		}

		viewModel.songCollection.removeAll();
		// Create real repository
		viewModel.repository = new kodemoApp.Repository();
		var songs = viewModel.repository.getSongs(viewModel.songCollection, genre);
	}
};

viewModel.averageRating = ko.computed(function () {
	if ((!viewModel) || (!viewModel.songCollection().length)) {
		return 'N/A';
	}

	var total = 0;
	for (var i = 0; i < viewModel.songCollection().length; i++) {
		total = total + viewModel.songCollection()[i].rating();
	}
	return total / viewModel.songCollection().length;
});

viewModel.songsLoaded = ko.computed(function () {
	if ((!viewModel) || (!viewModel.songCollection().length)) {
		return false;
	}
	return true;
});

viewModel.songSelected = ko.computed(function () {
	if ((!viewModel) || (!viewModel.selectedSong())) {
		return false;
	}
	return true;
});

(function (app) {

	function App() {

		var self = this;

		this.run = function () {
			ko.applyBindings(viewModel);

			// Setup jQuery UI
			$("#buttonPlay").button();
			$("#buttonLoad").button();
			$("#buttonSearchSongs").button();

			// Setup UI bindings
			$('#buttonPlay')[0].onclick = viewModel.playSong;
			$('#buttonLoad')[0].onclick = viewModel.loadSongs;
			$('#buttonSearchSongs')[0].onclick = function () {
				viewModel.searchSongs();
			}
		}
	}

	app.App = App;

}(window.kodemoApp));

// Mock Repository
(function (app) {

	function MockRepository() {

		var self = this;

		self.getSongs = function () {
			var songs = [];

			var song = new kodemoApp.Song();
			song.artist('Physics');
			song.album('Love Is A Business');
			song.genre('Hip-Hop');
			song.rating(3);
			song.title('These Moments');
			song.url('https://drive.google.com/uc?id=0B4dXRBkWJRuDTzdZZ0d4RmJUNXc&export=download');
			songs.push(song);

			song = new kodemoApp.Song();
			song.artist('Flaming Lips');
			song.album('Yoshimi Battles the Pink Robots');
			song.genre('Rock');
			song.rating(1);
			song.title('Fight Test');
			song.url('https://drive.google.com/uc?id=0B4dXRBkWJRuDM3B3b2Z3bjh4UWc&export=download');
			songs.push(song);

			return songs;
		}
	}

	app.MockRepository = MockRepository;

}(window.kodemoApp));

// Real Repository
(function (app) {

	function Repository() {

		var self = this;

		self.getSongs = function (songCollection, genre) {
			var songs = [];

			var url = 'http://vvoidmusic.azurewebsites.net/api/Song?genre=' + genre + '&skip=0&callback=?';

			$.getJSON(url, {}, function (json) {
				var songs = [];
				for (var i = 0; i < json.length; i++) {
					var song = new kodemoApp.Song();
					song.artist(json[i].ARTIST);
					song.album(json[i].ALBUM);
					song.genre(json[i].GENRE);
					song.rating(json[i].RATING);
					song.title(json[i].TITLE);
					song.url(json[i].GDRIVE);
					songs.push(song);
				}
				songCollection(songs);
			});
		}
	}

	app.Repository = Repository;

}(window.kodemoApp));

$(document).ready(function () {
	var app = new kodemoApp.App();
	app.run();
});


