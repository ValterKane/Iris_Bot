using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class ActionType
{
    public int IdType { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Action> Actions { get; set; } = new List<Action>();
}
