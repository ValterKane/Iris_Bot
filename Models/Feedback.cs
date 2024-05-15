using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Feedback
{
    public string PhoneNumber { get; set; } = null!;

    public string? TextRequest { get; set; }

    public DateTime? DateOfRquest { get; set; }

    public int ActionId { get; set; }

    public int? ActionRating { get; set; }

    public int? Rating { get; set; }

    public virtual Action Action { get; set; } = null!;

    public virtual Client PhoneNumberNavigation { get; set; } = null!;
}
