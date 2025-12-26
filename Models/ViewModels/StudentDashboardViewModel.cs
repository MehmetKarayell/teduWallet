using System.Collections.Generic;
using teduWallet.Models;

namespace teduWallet.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public decimal Balance { get; set; }
        public int CompletedTasksCount { get; set; }
        public int AvailableRewardsCount { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<Activity> ActiveActivities { get; set; } = new List<Activity>();
        public List<VwTop3StudentsThisWeek> TopStudents { get; set; } = new List<VwTop3StudentsThisWeek>();
    }
}
