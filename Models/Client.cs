using System;
using System.Collections.Generic;

namespace Iris_Bot.Models;

public partial class Client
{
    public string PhoneNumber { get; set; } = null!;

    public DateTime? DoB { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? LastName { get; set; }

    public string? Status { get; set; }

    public string? Comment { get; set; }

    public string? TelegramId { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<ChekIn> ChekIns { get; set; } = new List<ChekIn>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
