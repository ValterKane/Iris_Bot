using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Action
{
    public int IdAction { get; set; }

    public string? Name { get; set; }

    public string? Chair { get; set; }

    public int? Type { get; set; }

    public double? AvarageRating { get; set; }

    public string? AgeAllow { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<ActionPlan> ActionPlans { get; set; } = new List<ActionPlan>();

    public virtual ICollection<ChekIn> ChekIns { get; set; } = new List<ChekIn>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ActionType? TypeNavigation { get; set; }
}
