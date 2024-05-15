using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Plan
{
    public int IdPlan { get; set; }

    public string? PlanName { get; set; }

    public virtual ICollection<ActionPlan> ActionPlans { get; set; } = new List<ActionPlan>();
}
