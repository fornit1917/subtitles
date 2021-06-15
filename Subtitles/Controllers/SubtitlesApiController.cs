using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public SubtitlesApiController(ISubtitlesService subtitlesService)
        {
            _subtitlesService = subtitlesService;
        }

        [HttpGet]
        [Route("{movieId}/phrases")]
        public Task<PageResult<Phrase>> GetPhrases(int movieId, int skip = 0, int take = 100)
        {
            return _subtitlesService.GetPhrasesWithTranslations(movieId, skip, take);
        }

        [HttpPost]
        [Route("phrases/{phraseId}/translations")]
        public Task<PhraseTranslation> AddTranslation(int phraseId, [FromBody] CreateTranslationDto data)
        {
            return _subtitlesService.AddTranslation(phraseId, data.Content);
        }

        [HttpPost]
        [Route("translations/{translationId}/votes/plus")]
        public Task VotePlus(long translationId)
        {
            return _subtitlesService.ChangeVotesCount(translationId, 1);
        }

        [HttpPost]
        [Route("translations/{translationId}/votes/minus")]
        public Task VoteMinus(long translationId)
        {
            return _subtitlesService.ChangeVotesCount(translationId, -1);
        }


        public class CreateTranslationDto
        {
            public string Content { get; set; }
        }
    }
}
