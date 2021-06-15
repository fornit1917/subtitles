using Subtitles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.ViewModels
{
    public class MainPageViewModel
    {
        public IReadOnlyList<Movie> Movies { get; set; }
    }
}
