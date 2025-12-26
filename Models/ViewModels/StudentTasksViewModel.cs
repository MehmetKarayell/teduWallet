using System.Collections.Generic;
using teduWallet.Models;

namespace teduWallet.Models.ViewModels
{
    public class StudentTasksViewModel
    {
        public List<Activity> AvailableActivities { get; set; } = new List<Activity>();
        public List<int> AppliedActivityIds { get; set; } = new List<int>();
    }
}
