using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Dayofweak
{
    public int IdDay { get; set; }

    public string? ShortDay { get; set; }

    public string? LongDay { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
