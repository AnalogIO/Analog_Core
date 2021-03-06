﻿using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }


        /// <summary>
        ///     Gets the highscore of the specified preset 0 - Monthly, 1 - Semester and 2 - Total
        /// </summary>
        [HttpGet]
        public IActionResult Get(int preset, int top = 10)
        {
            var leaderboard = _leaderboardService.GetLeaderboard(preset, top);
            return Ok(leaderboard);
        }
    }
}