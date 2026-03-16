using SRMDevOps.Controllers;
using SRMDevOps.Dto;

namespace SRMDevOps.Repo
{
    public interface IADO
    {
        public Task<string> GetTeamsInProject(string projectName);
        public Task<TeamFieldValuesDto> GetTeamAreaPaths(string projectName, string teamName);

        public Task<List<SprintProgressDto>> GetSprintDataByAreaPathAsync(string projectId, string teamId, string selectedAreaPath, int lastNSprints);
    }
}
