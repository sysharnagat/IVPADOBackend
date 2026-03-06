using System;

namespace SRMDevOps.Dto
{
    public class StoryHistoryDto
    {
        public int UserStoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime? FirstInprogressTime { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string IterationPath { get; set; } = string.Empty;
        public int TotalHistoryCount { get; set; }
    }
}