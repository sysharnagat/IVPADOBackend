using SRMDevOps.Dto;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SRMDevOps.Repo
{
    public class DevopsService : IADO
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl = "https://dev.azure.com/Indusvalleypartners";

        public DevopsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private AuthenticationHeaderValue GetAuthHeader()
        {
            var pat = _configuration["AzureDevOps:PAT"];
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
            return new AuthenticationHeaderValue("Basic", credentials);
        }

        // 1. Fetch exact Area Paths owned by a team
        public async Task<List<string>> GetTeamAreaPathsAsync(string projectId, string teamId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = GetAuthHeader();

            string url = $"{_baseUrl}/{projectId}/{teamId}/_apis/work/teamsettings/teamfieldvalues?api-version=7.1";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<JsonElement>();
                return data.GetProperty("values")
                           .EnumerateArray()
                           .Select(v => v.GetProperty("value").GetString())
                           .ToList();
            }
            return new List<string>();
        }

        // 2. Fetch official Sprint Dates from ADO
        public async Task<List<SprintDto>> GetRecentSprintsAsync(string projectId, string teamId, int lastNSprints)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = GetAuthHeader();

            string url = $"{_baseUrl}/{projectId}/{teamId}/_apis/work/teamsettings/iterations?api-version=7.1";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode) return new List<SprintDto>();

            var data = await response.Content.ReadFromJsonAsync<AzureDevOpsResponse<SprintDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data.Value
                .Where(s => s.Attributes.StartDate.HasValue && s.Attributes.FinishDate.HasValue)
                .OrderByDescending(s => s.Attributes.StartDate)
                .Take(lastNSprints)
                .ToList();
        }

        // 3. Combined method to prepare data for your DB query
        public async Task<List<SprintProgressDto>> GetSprintDataByAreaPathAsync(
            string projectId,
            string teamId,
            int lastNSprints)
        {
            // First, get the "Source of Truth" from ADO API
            var teamPaths = await GetTeamAreaPathsAsync(projectId, teamId);
            var sprints = await GetRecentSprintsAsync(projectId, teamId, lastNSprints);

            // This list will be passed to your SpillageService or DB Repository
            // For now, returning the Sprint list to be processed by your DB logic
            var results = new List<SprintProgressDto>();

            foreach (var sprint in sprints)
            {
                // Here is where you integrate the DB calculation we discussed
                // results.Add(await _spillageService.CalculateFromDb(teamPaths, sprint.Attributes.StartDate, sprint.Attributes.FinishDate));
            }

            return results;
        }
    }

    public class AzureDevOpsResponse<T>
    {
        public int Count { get; set; }
        public List<T> Value { get; set; }
    }

    public class SprintDto
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Attributes Attributes { get; set; }
    }

    public class Attributes
    {
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}