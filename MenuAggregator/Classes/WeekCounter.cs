using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuAggregator.Classes
{
    public class WeekCounter
    {
        public static int mondayCount; 

        public static int MondayCounter(DateTime startDate, DateTime endDate)
        {
            DateTime monthStart = startDate;
            DateTime monthEnd = endDate;

            mondayCount = 0;
            for (DateTime dt = startDate; dt < endDate.AddDays(1); dt = dt.AddDays(1))
            {
                if (dt.DayOfWeek == DayOfWeek.Monday)
                {
                    mondayCount++;
                }
            }
            return mondayCount;
        }
    }
}
