function ServerEventsClient() { }

ServerEventsClient.prototype.start = function (movieId, handlers) {
    var hubConnection = new signalR.HubConnectionBuilder()
        .withAutomaticReconnect()
        .withUrl("/subtitles-hub").build();

    hubConnection.on("AddTranslation", data => {
        handlers.onTranslationAdded(data);
    });

    hubConnection.on("VotePlus", translationId => {
        handlers.onVotePlus(translationId);
    });

    hubConnection.on("VoteMinus", translationId => {
        handlers.onVoteMinus(translationId);
    });

    hubConnection.start().then(() => {
        var clientId = hubConnection.connection.connectionId;
        this.clientId = clientId;
        hubConnection.invoke("JoinToMovieTranslation", movieId);
    });

    hubConnection.onreconnected(connectionId => {
        this.clientId = connectionId;
    })
}