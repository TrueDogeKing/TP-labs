using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveObjectBrowser.Model
{
    public enum WorkMode
    {
        Remote,
        OnSite,
        Hybrid
    }
    public class WorkerDetails
    {
        private int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                if (value < 1||value>10)
                    throw new ArgumentOutOfRangeException(nameof(Rating), "Rating must be between 1 and 10.");
                _rating = value;
            }
        }
        public WorkerDetails() { }
        public double Bonus { get; set; }
        public WorkMode Mode { get; set; }

        public WorkerDetails(int rating, double bonus, WorkMode mode)
        {
            Rating = rating;
            Bonus = bonus;
            Mode = mode;
        }

    }
}
