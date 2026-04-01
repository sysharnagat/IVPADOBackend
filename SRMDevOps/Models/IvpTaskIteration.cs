using System;
using System.Collections.Generic;

namespace SRMDevOps.Models;

public partial class IvpTaskIteration
{
    public int TaskId { get; set; }

    public DateTime AssignedDate { get; set; }

    public string? IterationPath { get; set; }
}
