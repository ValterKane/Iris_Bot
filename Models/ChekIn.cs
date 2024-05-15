using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class ChekIn
{
    public int ChekInId { get; set; }

    public int IdAction { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public sbyte? Status { get; set; }

    public DateTime? Date { get; set; }

    public virtual Action IdActionNavigation { get; set; } = null!;

    public virtual Client PhoneNumberNavigation { get; set; } = null!;
}
