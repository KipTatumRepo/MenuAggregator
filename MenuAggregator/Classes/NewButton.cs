using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MenuAggregator.Classes
{
    public class NewButton : Button
    {
        private int _bid;

        public int Bid
        {
            get
            {
                return _bid;
            }
            set
            {
                _bid = value;
            }
        }
           
        public NewButton()
        {
            Bid = _bid;
        }

        public int GetBid()
        {
            return Bid;
        }
    }
}
