using System;
using System.Collections.Generic;

namespace SRMDevOps.Models;

public partial class IvpUserStoryDetail
{
    public int UserStoryId { get; set; }

    public string Project { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public int? StoryPoints { get; set; }

    public DateTime? FirstInprogressTime { get; set; }

    public DateTime? ClosedDate { get; set; }

    public string? ParentType { get; set; }

    public int? ParentId { get; set; }

    public string? Title { get; set; }

    public string? AreaPath { get; set; }

    public DateTime LastUpdatedOn { get; set; }

    public string? State { get; set; }

    public decimal? DevEffort { get; set; }
}
