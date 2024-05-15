using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class ActionPlan
{
    public int IdAction { get; set; }

    public int IdPlan { get; set; }

    public double? Cost { get; set; }

    public virtual Action IdActionNavigation { get; set; } = null!;

    public virtual Plan IdPlanNavigation { get; set; } = null!;
}
