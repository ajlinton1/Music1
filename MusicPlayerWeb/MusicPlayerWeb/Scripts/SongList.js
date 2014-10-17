var songModel = {
    player: null,
    currentSong: null,
    songs: ko.observableArray(),
    currentTitle: ko.observable(''),
    currentArtist: ko.observable(''),
    currentGenre: ko.observable(''),
    currentLocation: ko.observable(''),
    currentRating: ko.observable(''),
    currentId: ko.observable(''),

    getNewSongs: function () {
        var url = "api/SONG?";
        songModel.loadSongs(url);
    },

    loadSongs: function (url) {
        $.ajax({
            type: 'get',
            url: url,
            dataType: 'json',
            success: function (songs) {
                songModel.songs.removeAll();
                for (var i = 0; i < songs.length; i++) {
                    songModel.songs.push(songs[i]);
                }

                $('.songTitle').addClass("hover");

            },
            error: function (xhr, textStatus, errorThrown) {
                debugger;
                $('#statusDiv')[0].innerHTML = "Error";
            }
        });
    },

};

$(document).ready(function () {

    songModel.getNewSongs();
});

ko.applyBindings(songModel);
