using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Subtitles.Models;
using Subtitles.Services;
using Subtitles.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISubtitlesService _subtitlesService;


        public HomeController(ILogger<HomeController> logger, ISubtitlesService subtitlesService)
        {
            _logger = logger;
            _subtitlesService = subtitlesService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new MainPageViewModel()
            {
                Movies = await _subtitlesService.GetLatestMovies()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Route("upload-subtitles")]
        public async Task<IActionResult> UploadSubtitles(string name, IFormFile subtitlesFile)
        {
            using (var stream = subtitlesFile.OpenReadStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    await _subtitlesService.AddMovieToTranslation(name, sr);
                }
            }
                
            return Redirect("/");
        }

        [HttpGet]
        [Route("subtitles/{id}")]
        public async Task<IActionResult> Subtitles(int id)
        {
            Movie movie = await _subtitlesService.GetMovie(id);
            var viewModel = new SubtitlesEditorViewModel();
            viewModel.Id = movie.Id;
            viewModel.Name = movie.Name;
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
