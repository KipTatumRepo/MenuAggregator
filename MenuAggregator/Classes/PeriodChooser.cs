using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MenuAggregator.Classes
{
    public class PeriodChooser : DockPanel
    {
        private int _currentperiod;
        private WeekChooser Week;
        public int HeldPeriod;

        public int CurrentPeriod
        {
            get
            {
                return _currentperiod;
            }
            set
            {
                HeldPeriod = _currentperiod;
                _currentperiod = value;
                foreach (Border b in Children)
                {
                    //b.Tag = "Label"; // force the tag for the border to be Label
                    if (b.Tag == null)
                    {
                        TextBlock tb = (TextBlock)b.Child;
                        if (int.Parse(tb.Text) != value)
                        {
                            tb.Foreground = Brushes.LightGray;
                            tb.FontSize = 24;
                            tb.FontWeight = FontWeights.Normal;
                        }
                        else
                        {
                            tb.FontWeight = FontWeights.SemiBold;
                            tb.Foreground = Brushes.Black;
                            tb.FontSize = 32;
                        }
                    }
                }
            }
        }

        public int MinPeriod { get; set; }
        public int MaxPeriod { get; set; }
        public bool SelectAllEnabled;
        //public WeekChooser RelatedWeek = WeekChooser.
        public int SelectedCount { get; set; }

        public PeriodChooser(WeekChooser w, int minP, int maxP, int curP)
        {
            int ct;
            Week = w;
            MinPeriod = minP;
            MaxPeriod = maxP;

            Border BorderLabel = new Border() { BorderBrush = Brushes.Black, VerticalAlignment = VerticalAlignment.Center, Name = "brdLabel", Tag = "Label" };
            TextBlock TextLabel = new TextBlock() { Text = "     Month: ", TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 24, Name = "tbLabel", Tag = "Label" };
            BorderLabel.Child = TextLabel;
            Children.Add(BorderLabel);

            for (ct = 1; ct <= 12; ct++)
            {
                Border brdPeriod = new Border() { BorderBrush = Brushes.Black, Width = 40, VerticalAlignment = VerticalAlignment.Center, Name = "brdP" + ct };
                TextBlock tbPeriod = new TextBlock() { Text = ct.ToString(), TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 24, Tag = ct, Name = "tbP" + ct };
                /*if ((ct < MinPeriod) | (ct > MaxPeriod))
                    brdPeriod.IsEnabled = false;*/
                brdPeriod.MouseEnter += HoverOverPeriod;
                tbPeriod.MouseEnter += HoverOverPeriod;
                brdPeriod.MouseLeave += LeavePeriod;
                tbPeriod.MouseLeave += LeavePeriod;
                tbPeriod.PreviewMouseDown += ChoosePeriod;
                brdPeriod.Child = tbPeriod;
                Children.Add(brdPeriod);
            }
            CurrentPeriod = curP;
        }

        private void HoverOverPeriod(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
                tb = (TextBlock) sender;
            else
            {
                Border brd = (Border) sender;
                tb = (TextBlock) brd.Child;
            }
            tb.FontSize = 38;
        }

        private void LeavePeriod(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
                tb = (TextBlock) sender;
            else
            {
                Border brd = (Border) sender;
                tb = (TextBlock) brd.Child;
            }
            if (int.Parse(tb.Text) != CurrentPeriod)
                tb.FontSize = 24;
            else
                tb.FontSize = 32;
        }

        private void ChoosePeriod(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
                tb = (TextBlock) sender;
            else
            {
                Border brd = (Border) sender;
                tb = (TextBlock) brd.Child;
            }
            if ((int.Parse(tb.Text) != CurrentPeriod || SelectAllEnabled == false))
                CurrentPeriod = int.Parse(tb.Text);
            else if (CurrentPeriod != 0)
                Reset();
            /*if (CurrentPeriod < MaxPeriod)
            {
                Week.MaxWeek = GetMaxWeeks(CurrentPeriod);
                Week.CurrentWeek = 1;
            }
            else
            {
                Week.MaxWeek = GetCurrentWeek(Strings.FormatDateTime(DateTime.Now(), DateFormat.ShortDate));
                Week.CurrentWeek = GetCurrentWeek(Strings.FormatDateTime(DateTime.Now(), DateFormat.ShortDate));
            }*/
            //Week.EnableWeeks();
        }

        public void Reset()
        {
            CurrentPeriod = 0;
            foreach (Border brd in Children)
            {
                TextBlock tb = (TextBlock) brd.Child;
                if (brd.Tag == null)
                {
                    tb.Foreground = Brushes.Black;
                    tb.FontSize = 24;
                    tb.FontWeight = FontWeights.SemiBold;
                }
            }
        }

        private void EnableWeeks()
        {
            foreach (Border brd in Children)
            {
                if (brd.Tag == null)
                {
                    TextBlock tb = (TextBlock)brd.Child;
                    //if ((FormatNumber(tb.Tag, 0) < MinWeek) | (FormatNumber(tb.Tag, 0) > MaxWeek))
                    if (int.Parse(tb.Text) < 0 || int.Parse(tb.Text) > 5)
                    {
                        brd.IsEnabled = false;
                        brd.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        brd.IsEnabled = true;
                        brd.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}


