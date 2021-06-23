using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Subtitles.Hubs;
using Subtitles.Models;
using Subtitles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Controllers
{
    [Route("api/subtitles")]
    [ApiController]
    public class SubtitlesApiController : ControllerBase
    {
        private readonly ISubtitlesService _subtitlesService;
        private readonly IHubContext<SubtitlesHub> _hubContext;

        public SubtitlesApiController(ISubtitlesService subtitlesService, IHubContext<SubtitlesHub> hubContext)
        {
            _subtitlesService = subtitlesService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("{movieId}/phrases")]
        public Task<PageResult<Phrase>> GetPhrases(int movieId, int skip = 0, int take = 100)
        {
            return _subtitlesService.GetPhrasesWithTranslations(movieId, skip, take);
        }

        [HttpPost]
        [Route("phrases/{phraseId}/translations")]
        public async Task<PhraseTranslation> AddTranslation(int phraseId, [FromBody] CreateTranslationDto data)
        {
            PhraseTranslation translation = await _subtitlesService.AddTranslation(phraseId, data.Content);
            Movie movie = await _subtitlesService.GetMovieByTranslationId(translation.Id);
            await GetSignalrReceivers(movie.Id).SendAsync("AddTranslation", translation);
            return translation;
        }

        [HttpPost]
        [Route("translations/{translationId}/votes/plus")]
        public async Task VotePlus(long translationId)
        {
            await _subtitlesService.ChangeVotesCount(translationId, 1);
            Movie movie = await _subtitlesService.GetMovieByTranslationId(translationId);

            await GetSignalrReceivers(movie.Id).SendAsync("VotePlus", translationId);
        }

        [HttpPost]
        [Route("translations/{translationId}/votes/minus")]
        public async Task VoteMinus(long translationId)
        {
            await _subtitlesService.ChangeVotesCount(translationId, -1);
            Movie movie = await _subtitlesService.GetMovieByTranslationId(translationId);
            await GetSignalrReceivers(movie.Id).SendAsync("VoteMinus", translationId);

        }

        public IClientProxy GetSignalrReceivers(int movieId)
        {
            if (Request.Query.ContainsKey("clientId"))
            {
                var clientId = Request.Query["clientId"].ToString(); ;
                return _hubContext.Clients.GroupExcept($"movie{movieId}", new[] { clientId });
            }
            return _hubContext.Clients.Group($"movie{movieId}");
        }

        public class CreateTranslationDto
        {
            public string Content { get; set; }
        }
    }
}
