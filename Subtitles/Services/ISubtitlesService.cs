using Subtitles.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Services
{
    public interface ISubtitlesService
    {
        public Task<Movie[]> GetLatestMovies(int count = 20);

        public ValueTask<Movie> GetMovie(int id);

        public Task<Movie> AddMovieToTranslation(string name, StreamReader subtitles);

        public Task<PageResult<Phrase>> GetPhrasesWithTranslations(int movieId, int skip = 0, int take = 100);

        public Task<PhraseTranslation> AddTranslation(long phraseId, string content);

        public Task ChangeVotesCount(long translationId, int votesDelta);
    }
}
