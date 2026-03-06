using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Unified summary endpoint. Use query parameters to select mode:
        /// - ?lastNSprints=5  -> last-N-sprints mode
        /// - ?timeframe=yearly -> timeframe mode ("yearly" or default 6 months)
        /// If both provided, lastNSprints takes precedence.
        /// </summary>
        [HttpGet("summary/{projectName}")]
        public async Task<IActionResult> GetSpillageSummary(
            string projectName,
            [FromQuery] int? lastNSprints,
            [FromQuery] string? timeframe)
        {
            // last-N-sprints mode (takes precedence when provided)
            if (lastNSprints.HasValue && lastNSprints.Value > 0)
            {
                // execute sequentially to avoid concurrent DbContext access
                var statsAll = await _spillage.GetAllSprintStats(projectName, lastNSprints.Value);
                var statsFeature = await _spillage.GetFeatureSprintStats(projectName, lastNSprints.Value);
                var statsClient = await _spillage.GetClientSprintStats(projectName, lastNSprints.Value);

                var trendAll = await _spillage.GetAllSpillageTrend(projectName, lastNSprints.Value);
                var trendFeature = await _spillage.GetFeatureSpillageTrend(projectName, lastNSprints.Value);
                var trendClient = await _spillage.GetClientSpillageTrend(projectName, lastNSprints.Value);

                var summary = new SpillageSummaryDto
                {
                    All = new SectionDto { Stats = statsAll, Spillage = trendAll },
                    Feature = new SectionDto { Stats = statsFeature, Spillage = trendFeature },
                    Client = new SectionDto { Stats = statsClient, Spillage = trendClient }
                };

                return Ok(summary);
            }

            // timeframe mode (defaults to service behavior if timeframe is null/empty)
            var statsAllTime = await _spillage.GetAllSprintStatsByTime(projectName, timeframe ?? string.Empty);
            var statsFeatureTime = await _spillage.GetFeatureSprintStatsByTime(projectName, timeframe ?? string.Empty);
            var statsClientTime = await _spillage.GetClientSprintStatsByTime(projectName, timeframe ?? string.Empty);

            var timelineAll = await _spillage.GetAllSpillageTimeline(projectName, timeframe ?? string.Empty);
            var timelineFeature = await _spillage.GetFeatureSpillageTimeline(projectName, timeframe ?? string.Empty);
            var timelineClient = await _spillage.GetClientSpillageTimeline(projectName, timeframe ?? string.Empty);

            var summaryTime = new SpillageSummaryDto
            {
                All = new SectionDto { Stats = statsAllTime, Spillage = timelineAll },
                Feature = new SectionDto { Stats = statsFeatureTime, Spillage = timelineFeature },
                Client = new SectionDto { Stats = statsClientTime, Spillage = timelineClient }
            };

            return Ok(summaryTime);
        }
    }
}

//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using SRMDevOps.Dto;
//using SRMDevOps.Repo;

//namespace SRMDevOps.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SpillageController : ControllerBase
//    {
//        private readonly ISpillage _spillage;

//        public SpillageController(ISpillage spillage)
//        {
//            _spillage = spillage;
//        }

//        // Aggregated endpoint for last N sprints:
//        // Returns SpillageSummaryDto with segregated sections for clarity on the frontend.
//        [HttpGet("summary-last/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetSpillageSummaryLast(string projectName, int lastNSprints)
//        {
//            // Execute repository calls sequentially to avoid concurrent DbContext access.
//            var statsAll = await _spillage.GetAllSprintStats(projectName, lastNSprints);
//            var statsFeature = await _spillage.GetFeatureSprintStats(projectName, lastNSprints);
//            var statsClient = await _spillage.GetClientSprintStats(projectName, lastNSprints);

//            var trendAll = await _spillage.GetAllSpillageTrend(projectName, lastNSprints);
//            var trendFeature = await _spillage.GetFeatureSpillageTrend(projectName, lastNSprints);
//            var trendClient = await _spillage.GetClientSpillageTrend(projectName, lastNSprints);

//            var summary = new SpillageSummaryDto
//            {
//                All = new SectionDto { Stats = statsAll, Spillage = trendAll },
//                Feature = new SectionDto { Stats = statsFeature, Spillage = trendFeature },
//                Client = new SectionDto { Stats = statsClient, Spillage = trendClient }
//            };

//            return Ok(summary);
//        }

//        // Aggregated endpoint for timeframe-based queries:
//        // Returns SpillageSummaryDto with segregated sections for clarity on the frontend.
//        [HttpGet("summary-time/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetSpillageSummaryTime(string projectName, string timeframe)
//        {
//            // Execute repository calls sequentially to avoid concurrent DbContext access.
//            var statsAll = await _spillage.GetAllSprintStatsByTime(projectName, timeframe);
//            var statsFeature = await _spillage.GetFeatureSprintStatsByTime(projectName, timeframe);
//            var statsClient = await _spillage.GetClientSprintStatsByTime(projectName, timeframe);

//            var timelineAll = await _spillage.GetAllSpillageTimeline(projectName, timeframe);
//            var timelineFeature = await _spillage.GetFeatureSpillageTimeline(projectName, timeframe);
//            var timelineClient = await _spillage.GetClientSpillageTimeline(projectName, timeframe);

//            var summary = new SpillageSummaryDto
//            {
//                All = new SectionDto { Stats = statsAll, Spillage = timelineAll },
//                Feature = new SectionDto { Stats = statsFeature, Spillage = timelineFeature },
//                Client = new SectionDto { Stats = statsClient, Spillage = timelineClient }
//            };

//            return Ok(summary);
//        }
//    }
//}

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SRMDevOps.DataAccess;
//using SRMDevOps.Dto;
//using SRMDevOps.Repo;

//namespace SRMDevOps.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SpillageController : ControllerBase
//    {
//        private readonly ISpillage _spillage;

//        public SpillageController(ISpillage spillage)
//        {
//            _spillage = spillage;
//        }

//        // Controller of fetching details for all feature or client

//        [HttpGet("burnup-all/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetAllSprintStats(string projectName, int lastNSprints)
//        {
//            var finalResult = await _spillage.GetAllSprintStats(projectName, lastNSprints);

//            return Ok(finalResult);
//        }

//        [HttpGet("burnup-timeline-all/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetAllSprintStatsByTime(string projectName, string timeframe)
//        {
//            var stats = await _spillage.GetAllSprintStatsByTime(projectName, timeframe);

//            return Ok(stats);
//        }

//        [HttpGet("spillage-trend-all/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetAllSpillageTrend(string projectName, int lastNSprints)
//        {
//            var result = await _spillage.GetAllSpillageTrend(projectName, lastNSprints);

//            return Ok(result);
//        }

//        [HttpGet("spillage-timeline-all/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetAllSpillageTimeline(string projectName, string timeframe)
//        {
//            var result = await _spillage.GetAllSpillageTimeline(projectName, timeframe);

//            return Ok(result);
//        }


//        // Controller of fetching data for feature

//        [HttpGet("burnup-feature/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetFeatureSprintStats(string projectName, int lastNSprints)
//        {
//            var finalResult = await _spillage.GetFeatureSprintStats(projectName, lastNSprints);

//            return Ok(finalResult);
//        }

//        [HttpGet("burnup-timeline-feature/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetFeatureSprintStatsByTime(string projectName, string timeframe)
//        {
//            var stats = await _spillage.GetFeatureSprintStatsByTime(projectName, timeframe);

//            return Ok(stats);
//        }

//        [HttpGet("spillage-trend-feature/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetFeatureSpillageTrend(string projectName, int lastNSprints)
//        {
//            var result = await _spillage.GetFeatureSpillageTrend(projectName, lastNSprints);

//            return Ok(result);
//        }

//        [HttpGet("spillage-timeline-feature/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetFeatureSpillageTimeline(string projectName, string timeframe)
//        {
//            var result = await _spillage.GetFeatureSpillageTimeline(projectName, timeframe);

//            return Ok(result);
//        }

//        // Controller of fetching data for client
//        // Routes renamed to avoid conflicts with "all" endpoints.

//        [HttpGet("burnup-client/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetClientSprintStats(string projectName, int lastNSprints)
//        {
//            var finalResult = await _spillage.GetClientSprintStats(projectName, lastNSprints);

//            return Ok(finalResult);
//        }

//        [HttpGet("burnup-timeline-client/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetClientSprintStatsByTime(string projectName, string timeframe)
//        {
//            var stats = await _spillage.GetClientSprintStatsByTime(projectName, timeframe);

//            return Ok(stats);
//        }

//        [HttpGet("spillage-trend-client/{projectName}/{lastNSprints}")]
//        public async Task<IActionResult> GetClientSpillageTrend(string projectName, int lastNSprints)
//        {
//            var result = await _spillage.GetClientSpillageTrend(projectName, lastNSprints);

//            return Ok(result);
//        }

//        [HttpGet("spillage-timeline-client/{projectName}/{timeframe}")]
//        public async Task<IActionResult> GetClientSpillageTimeline(string projectName, string timeframe)
//        {
//            var result = await _spillage.GetClientSpillageTimeline(projectName, timeframe);

//            return Ok(result);
//        }


//    }
//}
