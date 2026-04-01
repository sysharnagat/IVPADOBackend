using System;
using System.Collections.Generic;

namespace SRMDevOps.Models;

public partial class IvpTaskAssignee
{
    public int TaskId { get; set; }

    public DateTime AssignedDate { get; set; }

    public string? AssignedTo { get; set; }
}
