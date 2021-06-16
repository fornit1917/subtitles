using Microsoft.EntityFrameworkCore;
using Subtitles.Database;
using Subtitles.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subtitles.Services
{
    public class SubtitlesService : ISubtitlesService
    {
        private AppDbContext _db;

        public SubtitlesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Movie> AddMovieToTranslation(string name, StreamReader subtitles)
        {
            var movie = new Movie();
            movie.Name = name;
            movie.Phrases = new List<Phrase>();
            _db.Add(movie);

            StringBuilder sb = new StringBuilder();

            while (!subtitles.EndOfStream)
            {
                // skip empty strings
                string s = "";
                while (s == "" && !subtitles.EndOfStream)
                {
                    s = (await subtitles.ReadLineAsync()).Trim();
                }

                //now s contains sequence number
                var phrase = new Phrase();
                phrase.SequenceNumber = Convert.ToInt32(s);

                // time
                s = await subtitles.ReadLineAsync();
                phrase.Time = s;

                // read phrase lines
                sb.Clear();
                int linesCount = 0;
                while (s != "" && !subtitles.EndOfStream)
                {
                    s = await subtitles.ReadLineAsync();
                    if (s != "" && !subtitles.EndOfStream)
                    {
                        if (linesCount > 0)
                        {
                            sb.Append("\n");
                        }
                        sb.Append(s);
                        linesCount++;
                    }
                }
                phrase.Content = sb.ToString();

                movie.Phrases.Add(phrase);
            }

            await _db.SaveChangesAsync();
            return movie;
        }

        public async Task<PhraseTranslation> AddTranslation(long phraseId, string content)
        {
            var phraseTranslation = new PhraseTranslation();
            phraseTranslation.PhraseId = phraseId;
            phraseTranslation.Content = content;
            _db.PhraseTranslations.Add(phraseTranslation);
            await _db.SaveChangesAsync();
            return phraseTranslation;
        }

        public Task ChangeVotesCount(long translationId, int votesDelta)
        {
            return _db.Database
                .ExecuteSqlInterpolatedAsync($"UPDATE PhraseTranslations SET VotesCount = VotesCount + {votesDelta} WHERE Id={translationId}");
        }

        public Task<Movie[]> GetLatestMovies(int count = 20)
        {
            return _db.Movies.OrderByDescending(x => x.Id).Take(count).AsNoTracking().ToArrayAsync();
        }

        public ValueTask<Movie> GetMovie(int id)
        {
            return _db.Movies.FindAsync(id);
        }

        public Task<Movie> GetMovieByTranslationId(long translationId)
        {
            return _db.PhraseTranslations
                .Where(x => x.Id == translationId)
                .Include(x => x.Phrase)
                .ThenInclude(x => x.Movie)
                .Select(x => x.Phrase.Movie)
                .FirstAsync();
        }

        public async Task<PageResult<Phrase>> GetPhrasesWithTranslations(int movieId, int skip = 0, int take = 100)
        {
            int totalCount = await _db.Phrases.Where(x => x.MovieId == movieId).CountAsync();

            Phrase[] phrases = await _db.Phrases
                .Where(x => x.MovieId == movieId)
                .OrderBy(x => x.SequenceNumber)
                .Include(x => x.PhraseTranslations)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToArrayAsync();

            return new PageResult<Phrase>(phrases, totalCount, skip);
        }
    }
}
