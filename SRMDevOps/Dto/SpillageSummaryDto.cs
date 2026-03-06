namespace SRMDevOps.Dto
{
    // Produces JSON like:
    // { "All": { "stats": ..., "spillage": ... }, "Feature": { ... }, "Client": { ... } }
    public class SpillageSummaryDto
    {
        public SectionDto All { get; init; } = new();
        public SectionDto Feature { get; init; } = new();
        public SectionDto Client { get; init; } = new();
    }
}
