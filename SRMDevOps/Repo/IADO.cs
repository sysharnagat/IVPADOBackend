using SRMDevOps.Controllers;
using SRMDevOps.Dto;

namespace SRMDevOps.Repo
{
    public interface IADO
    {
        Task<List<string>> GetTeamAreaPathsAsync(string projectId, string teamId);
        Task<List<SprintDto>> GetRecentSprintsAsync(string projectId, string teamId, int lastNSprints);
        Task<List<SprintProgressDto>> GetSprintDataByAreaPathAsync(
            string projectId,
            string teamId,
            int lastNSprints);

        //public Task<string> GetTeamsInProject(string projectName);
        //public Task<TeamFieldValuesDto> GetTeamAreaPaths(string projectName, string teamName);

        //public Task<List<SprintProgressDto>> GetSprintDataByAreaPathAsync(string projectId, string teamId, string selectedAreaPath, int lastNSprints);
    }
}
