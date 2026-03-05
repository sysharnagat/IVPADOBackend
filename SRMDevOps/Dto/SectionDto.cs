namespace SRMDevOps.Dto
{
    // Represents one segment (All / Feature / Client) with stats and spillage/timeline.
    public class SectionDto
    {
        public object? Stats { get; init; }
        public object? Spillage { get; init; }
    }
}
