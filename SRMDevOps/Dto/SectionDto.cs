namespace SRMDevOps.Dto
{
    // Represents one segment (All / Feature / Client) with stats, spillage/timeline and optional story history.
    public class SectionDto
    {
        public object? Stats { get; init; }
        public object? Spillage { get; init; }

        // New: per-story history relevant to this section (empty when not requested)
        public List<StoryHistoryDto> History { get; init; } = new();
    }
}
