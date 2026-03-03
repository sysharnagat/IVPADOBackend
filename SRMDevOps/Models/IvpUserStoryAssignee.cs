using System;
using System.Collections.Generic;

namespace SRMDevOps.Models;

public partial class IvpUserStoryAssignee
{
    public int UserStoryId { get; set; }

    public DateTime AssignedDate { get; set; }

    public string? AssignedTo { get; set; }
}
