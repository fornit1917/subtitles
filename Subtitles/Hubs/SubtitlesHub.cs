using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Hubs
{
    public class SubtitlesHub : Hub
    {
        public Task JoinToMovieTranslation(int movieId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"movie{movieId}");
        }
    }
}
