function initSubtitlesEditor(movieId) {
    const app = new Vue({
        el: "#app",
        data: {
            isLoading: true,
            offset: 0,
            phrases: [],
            translationsMap: {},
            hasNext: false,
            hasPrev: false,
        },
        methods: {
            nextPage: function () {
                this.offset += 100;
                getPhrasesWithTranslations(movieId, this.offset, 100)
                    .then(onLoaded);
            },

            prevPage: function () {
                this.offset -= 100;
                getPhrasesWithTranslations(movieId, this.offset, 100)
                    .then(onLoaded);
            },

            addTranslation: function (e) {
                var id = Number(e.target.dataset.id);
                var textarea = document.getElementById(`new-translation-${id}`);
                var content = textarea.value;
                addTranslation(id, content).then(function (newTranslation) {
                    onTranslationAdded(newTranslation);
                    textarea.value = "";
                });
            },

            votePlus: function (e) {
                var id = Number(e.target.dataset.id);
                sendVotePlus(id).then(() => onVotePlus(id));
            },

            voteMinus: function (e) {
                var id = Number(e.target.dataset.id);
                sendVoteMinus(id).then(() => onVoteMinus(id));
            },
        }
    });

    // helpers

    function onLoaded(data) {
        app.translationsMap = {};
        for (var i = 0; i < data.items.length; i++) {
            data.items[i].content = data.items[i].content.replace(/\n/g, "<br/>");
            for (var j = 0; j < data.items[i].phraseTranslations.length; j++) {
                data.items[i].phraseTranslations[j].content = data.items[i].phraseTranslations[j].content.replace(/\n/g, "<br/>");

                app.translationsMap[data.items[i].phraseTranslations[j].id] = data.items[i].phraseTranslations[j];
            }
        }
        
        app.phrases = data.items;
        app.hasNext = data.hasNext;
        app.hasPrev = data.hasPrev;
        app.isLoading = false;
    }

    function onTranslationAdded(newTranslation) {
        newTranslation.content = newTranslation.content.replace(/\n/g, "<br/>");
        var phrase = app.phrases.find(x => x.id == newTranslation.phraseId);
        if (!phrase) {
            return;
        }
        phrase.phraseTranslations.push(newTranslation);
        app.translationsMap[newTranslation.id] = newTranslation;
    }

    function onVotePlus(translationId) {
        onVoteUpdate(translationId, 1);
    }

    function onVoteMinus(translationId) {
        onVoteUpdate(translationId, -1);
    }

    function onVoteUpdate(translationId, votesDelta) {
        var phraseId = app.translationsMap[translationId] ? app.translationsMap[translationId].phraseId : null;
        var phrase = app.phrases.find(x => x.id == phraseId);
        if (!phrase) {
            return;
        }

        var translation = phrase.phraseTranslations.find(x => x.id == translationId);
        if (!translation) {
            return;
        }

        translation.votesCount += votesDelta;
    }

    // api

    function getPhrasesWithTranslations(movieId, skip, take) {
        return $.ajax({
            type: "GET",
            url: `/api/subtitles/${movieId}/phrases?skip=${skip}&take=${take}`,
        });
    }

    function addTranslation(phraseId, content) {
        return $.ajax({
            type: "POST",
            url: `/api/subtitles/phrases/${phraseId}/translations`,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ content }),
        })
    }

    function sendVotePlus(translationId) {
        return $.ajax({
            type: "POST",
            url: `/api/subtitles/translations/${translationId}/votes/plus`
        });
    }

    function sendVoteMinus(translationId) {
        return $.ajax({
            type: "POST",
            url: `/api/subtitles/translations/${translationId}/votes/minus`
        });
    }

    // ui handlers for server events

    var handlers = {
        onTranslationAdded,
        onVotePlus,
        onVoteMinus,
    }

    // init app
    getPhrasesWithTranslations(movieId, 0, 100).then((data) => {
        onLoaded(data);
    });
}


