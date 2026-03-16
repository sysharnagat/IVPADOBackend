using SRMDevOps.Controllers;
using SRMDevOps.Dto;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SRMDevOps.Repo
{
    public class DevopsService : IADO
    {

        private readonly IConfiguration _configuration;

        public DevopsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetTeamsInProject(string projectId)
        {
            // Retrieve PAT from configuration securely
            var pat = _configuration["AzureDevOps:PAT"];
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // URL to fetch all teams for a specific project
                string url = $"https://dev.azure.com/Indusvalleypartners/_apis/projects/{projectId}/teams?api-version=7.1";

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    throw new Exception($"Failed to fetch teams. Status: {response.StatusCode}");
                }
            }
        }

        public async Task<TeamFieldValuesDto> GetTeamAreaPaths(string projectId, string teamId)
        {
            var pat = _configuration["AzureDevOps:PAT"];
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // API call to get Team Field Values (which are Area Paths by default)
                string url = $"https://dev.azure.com/Indusvalleypartners/{projectId}/{teamId}/_apis/work/teamsettings/teamfieldvalues?api-version=7.1"; 

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TeamFieldValuesDto>(rawJson, options);
                }
                throw new Exception($"Error: {response.StatusCode}");
            }
        }

        //public async Task<SprintProgressDto> GetStatsForSpecificAreaPath(
        //    string projectId,
        //    string iterationPath,
        //    string selectedAreaPath)
        //{
        //    var pat = _configuration["AzureDevOps:PAT"];
        //    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));

        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        //        // WIQL Query: Filter by Area Path AND Iteration Path
        //        var query = new
        //        {
        //            query = $@"SELECT [System.Id], [Microsoft.VSTS.Scheduling.StoryPoints], [System.State] 
        //               FROM WorkItems 
        //               WHERE [System.TeamProject] = '{projectId}' 
        //               AND [System.WorkItemType] = 'User Story'
        //               AND [System.IterationPath] = '{iterationPath}'
        //               AND [System.AreaPath] UNDER '{selectedAreaPath}'"
        //        };

        //        string url = $"https://dev.azure.com/Indusvalleypartners/{projectId}/_apis/wit/wiql?api-version=7.1";

        //        var response = await client.PostAsJsonAsync(url, query);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            // This returns a list of Work Item IDs. 
        //            // You then need to fetch the 'Story Points' details for these IDs in a batch.
        //            var result = await response.Content.ReadAsStringAsync();
        //            return ParseAndAggregate(result); // Helper to sum points
        //        }
        //        throw new Exception("WIQL Query Failed");
        //    }
        //}

        public async Task<List<SprintProgressDto>> GetSprintDataByAreaPathAsync(
    string projectId,
    string teamId,
    string selectedAreaPath,
    int lastNSprints)
        {
            var pat = _configuration["AzureDevOps:PAT"];
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
            var results = new List<SprintProgressDto>();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // 1. Get all iterations for the team
            string iterationsUrl = $"https://dev.azure.com/Indusvalleypartners/{projectId}/{teamId}/_apis/work/teamsettings/iterations?api-version=7.1";
            var iterationsResponse = await client.GetAsync(iterationsUrl);

            if (!iterationsResponse.IsSuccessStatusCode) return results;

            var iterationsRaw = await iterationsResponse.Content.ReadAsStringAsync();
            var allSprints = JsonSerializer.Deserialize<AzureDevOpsResponse<SprintDto>>(iterationsRaw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 2. Filter for the last N sprints based on Start Date
            var recentSprints = allSprints.Value
                .Where(s => s.Attributes.StartDate.HasValue)
                .OrderByDescending(s => s.Attributes.StartDate)
                .Take(lastNSprints)
                .ToList();

            // 3. Loop through sprints and fetch data via WIQL
            foreach (var sprint in recentSprints)
            {
                var wiqlQuery = new
                {
                    query = $@"SELECT [System.Id] FROM WorkItems 
               WHERE [System.IterationPath] = '{sprint.Path}' 
               AND [System.AreaPath] UNDER '{selectedAreaPath}'"
                };
                //{
                //    query = $@"SELECT [System.Id], [Microsoft.VSTS.Scheduling.StoryPoints], [System.State] 
                //       FROM WorkItems 
                //       WHERE [System.TeamProject] = '{projectId}' 
                //       AND [System.WorkItemType] = 'User Story'
                //       AND [System.IterationPath] = '{sprint.Path}'
                //       AND [System.AreaPath] UNDER '{selectedAreaPath}'"
                //};

                string wiqlUrl = $"https://dev.azure.com/Indusvalleypartners/{projectId}/_apis/wit/wiql?api-version=7.1";
                // Inside your loop for each sprint...
                var wiqlResponse = await client.PostAsJsonAsync(wiqlUrl, wiqlQuery);

                if (wiqlResponse.IsSuccessStatusCode)
                {
                    var wiqlResult = await wiqlResponse.Content.ReadFromJsonAsync<JsonElement>();
                    var workItemIds = wiqlResult.GetProperty("workItems")
                                                .EnumerateArray()
                                                .Select(x => x.GetProperty("id").GetInt32())
                                                .ToList();
                    Console.WriteLine($"--- Debugging Iteration: {sprint} ---");
                    Console.WriteLine($"Total IDs Found: {workItemIds.Count}");
                    Console.WriteLine($"IDs: {string.Join(", ", workItemIds)}");

                    // ... (inside the iteration loop)

                    if (workItemIds.Any())
                    {
                        var batchUrl = $"https://dev.azure.com/Indusvalleypartners/{projectId}/_apis/wit/workitemsbatch?api-version=7.1";
                        var batchRequest = new
                        {
                            ids = workItemIds,
                            fields = new[] { "Microsoft.VSTS.Scheduling.StoryPoints", "System.State", "System.Title" }
                        };

                        var batchResponse = await client.PostAsJsonAsync(batchUrl, batchRequest);
                        var batchData = await batchResponse.Content.ReadFromJsonAsync<JsonElement>();

                        // Check if "value" property exists in batchData
                        if (batchData.ValueKind != JsonValueKind.Null && batchData.TryGetProperty("value", out var valueArray))
                        {
                            double totalPoints = 0;
                            double completedPoints = 0;

                            foreach (var item in valueArray.EnumerateArray())
                            {
                                // CRITICAL FIX: Ensure "fields" exists before accessing it
                                if (!item.TryGetProperty("fields", out var fields))
                                {
                                    Console.WriteLine($"Warning: Item {item.GetProperty("id")} has no fields property.");
                                    continue;
                                }

                                // 1. Safe Story Point Extraction
                                double points = 0;
                                if (fields.TryGetProperty("Microsoft.VSTS.Scheduling.StoryPoints", out var pointElement) && pointElement.ValueKind != JsonValueKind.Null)
                                {
                                    points = pointElement.GetDouble();
                                }

                                // 2. Safe State Extraction
                                string state = "New";
                                if (fields.TryGetProperty("System.State", out var stateElement))
                                {
                                    state = stateElement.GetString() ?? "New";
                                }

                                totalPoints += points;

                                var completedStates = new[] { "Closed", "Done", "Verified", "Completed", "Live" };
                                if (completedStates.Contains(state, StringComparer.OrdinalIgnoreCase))
                                {
                                    completedPoints += points;
                                }
                            }

                            results.Add(new SprintProgressDto
                            {
                                // Use ?.Name to prevent NullReference if sprint is somehow null
                                IterationPath = sprint?.Name ?? "Unknown Sprint",
                                TotalPointsAssigned = totalPoints,
                                TotalPointsCompleted = completedPoints,
                                SortDate = sprint?.Attributes?.StartDate
                            });
                        }
                    }
                }
            }

            return results.OrderBy(r => r.SortDate).ToList();
        }
    }

    public class SprintDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Attributes Attributes { get; set; } // Contains StartDate and FinishDate
    }

    public class Attributes
    {
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string TimeFrame { get; set; } // "past", "current", or "future"
    }
}
