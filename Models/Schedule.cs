using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Schedule
{
    public int IdAction { get; set; }

    public int IdDay { get; set; }

    public string? Time { get; set; }

    public virtual Action IdActionNavigation { get; set; } = null!;

    public virtual Dayofweak IdDayNavigation { get; set; } = null!;
}
