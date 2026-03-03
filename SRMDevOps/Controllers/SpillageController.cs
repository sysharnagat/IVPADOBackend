using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRMDevOps.DataAccess;
using SRMDevOps.Dto;
using SRMDevOps.Repo;

namespace SRMDevOps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpillageController : ControllerBase
    {
        private readonly ISpillage _spillage;

        public SpillageController(ISpillage spillage)
        {
            _spillage = spillage;
        }

        // Controller of fetching details for all feature or client

        [HttpGet("burnup-all/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetAllSprintStats(string projectName, int lastNSprints)
        {
            var finalResult = await _spillage.GetAllSprintStats(projectName, lastNSprints);

            return Ok(finalResult);
        }

        [HttpGet("burnup-timeline-all/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetAllSprintStatsByTime(string projectName, string timeframe)
        {
            var stats = await _spillage.GetAllSprintStatsByTime(projectName, timeframe);

            return Ok(stats);
        }

        [HttpGet("spillage-trend-all/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetAllSpillageTrend(string projectName, int lastNSprints)
        {
            var result = await _spillage.GetAllSpillageTrend(projectName, lastNSprints);

            return Ok(result);
        }

        [HttpGet("spillage-timeline-all/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetAllSpillageTimeline(string projectName, string timeframe)
        {
            var result = await _spillage.GetAllSpillageTimeline(projectName, timeframe);

            return Ok(result);
        }


        // Controller of fetching data for feature

        [HttpGet("burnup-feature/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetFeatureSprintStats(string projectName, int lastNSprints)
        {
            var finalResult = await _spillage.GetFeatureSprintStats(projectName, lastNSprints);

            return Ok(finalResult);
        }

        [HttpGet("burnup-timeline-feature/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetFeatureSprintStatsByTime(string projectName, string timeframe)
        {
            var stats = await _spillage.GetFeatureSprintStatsByTime(projectName, timeframe);

            return Ok(stats);
        }

        [HttpGet("spillage-trend-feature/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetFeatureSpillageTrend(string projectName, int lastNSprints)
        {
            var result = await _spillage.GetFeatureSpillageTrend(projectName, lastNSprints);

            return Ok(result);
        }

        [HttpGet("spillage-timeline-feature/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetFeatureSpillageTimeline(string projectName, string timeframe)
        {
            var result = await _spillage.GetFeatureSpillageTimeline(projectName, timeframe);

            return Ok(result);
        }

        // Controller of fetching data for client
        // Routes renamed to avoid conflicts with "all" endpoints.

        [HttpGet("burnup-client/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetClientSprintStats(string projectName, int lastNSprints)
        {
            var finalResult = await _spillage.GetClientSprintStats(projectName, lastNSprints);

            return Ok(finalResult);
        }

        [HttpGet("burnup-timeline-client/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetClientSprintStatsByTime(string projectName, string timeframe)
        {
            var stats = await _spillage.GetClientSprintStatsByTime(projectName, timeframe);

            return Ok(stats);
        }

        [HttpGet("spillage-trend-client/{projectName}/{lastNSprints}")]
        public async Task<IActionResult> GetClientSpillageTrend(string projectName, int lastNSprints)
        {
            var result = await _spillage.GetClientSpillageTrend(projectName, lastNSprints);

            return Ok(result);
        }

        [HttpGet("spillage-timeline-client/{projectName}/{timeframe}")]
        public async Task<IActionResult> GetClientSpillageTimeline(string projectName, string timeframe)
        {
            var result = await _spillage.GetClientSpillageTimeline(projectName, timeframe);

            return Ok(result);
        }


    }
}
