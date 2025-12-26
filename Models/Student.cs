using System;
using System.Collections.Generic;

namespace teduWallet.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public string Departmant { get; set; } = null!;

    public string? Password { get; set; }

    public string Username { get; set; } = null!;

    public decimal? Coins { get; set; }

    public virtual ICollection<Apply> Applies { get; set; } = new List<Apply>();

    public virtual ICollection<Complete> Completes { get; set; } = new List<Complete>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual Wallet? Wallet { get; set; }

    public virtual ICollection<WalletSpendsReward> WalletSpendsRewards { get; set; } = new List<WalletSpendsReward>();
}
