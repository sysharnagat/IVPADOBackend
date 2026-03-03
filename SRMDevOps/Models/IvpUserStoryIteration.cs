using System;
using System.Collections.Generic;

namespace SRMDevOps.Models;

public partial class IvpUserStoryIteration
{
    public int UserStoryId { get; set; }

    public DateTime AssignedDate { get; set; }

    public string? IterationPath { get; set; }
}
