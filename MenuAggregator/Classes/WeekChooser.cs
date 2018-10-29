using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MenuAggregator.Classes
{
    public class WeekChooser : DockPanel, INotifyPropertyChanged
    {
        private int _currentweek;
        private int maxWeek = WeekCounter.MondayCounter(firstOfMonth, endOfMonth);
        public static DateTime today = DateTime.Today; 
        public static DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
        public static DateTime endOfMonth = new DateTime(today.Year,
                                           today.Month,
                                                        DateTime.DaysInMonth(today.Year,
                                                                 today.Month));

        public int CurrentWeek
        {
            get
            {
                return _currentweek;
            }
            set
            {
                HeldWeek = _currentweek;
                _currentweek = value;

                foreach (Border b in Children)
                {
                   
                    if(b.Tag == null)
                    {
                        TextBlock tb = (TextBlock) b.Child;

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
                RaisePropertyChanged("CurrentWeek");
            }
        }

        public int MinWeek { get; set; }
        public int MaxWeek { get; set; }
        public int HeldWeek { get; set; }

        public WeekChooser(int MinW, int MaxW, int CurW)
        {
            int ct;
            MinWeek = MinW;
            MaxWeek = MaxW;

            Border BorderLabel = new Border() { BorderBrush = Brushes.Black, VerticalAlignment = VerticalAlignment.Center, Name = "brdLabel", Tag = "Label" };
            TextBlock TextLabel = new TextBlock() { Text = "                      Week: ", TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 24, Name = "tbLabel", Tag = "Label" };

            BorderLabel.Child = TextLabel;
            Children.Add(BorderLabel);

            for (ct = 1; ct <= maxWeek; ct++)//CHECK HERE
            {
                Border brdWeek = new Border() { BorderBrush = Brushes.Black, Width = 40, VerticalAlignment = VerticalAlignment.Center, Name = "brdW" + ct };
                TextBlock tbWeek = new TextBlock() { Text = ct.ToString(), TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 24, Tag = ct, Name = "tbW" + ct };
                if ((ct < MinWeek) | (ct > MaxWeek))
                    brdWeek.IsEnabled = false;

                brdWeek.MouseEnter += HoverOverWeek;
                tbWeek.MouseEnter += HoverOverWeek;
                brdWeek.MouseLeave += LeaveWeek;
                tbWeek.MouseLeave += LeaveWeek;
                tbWeek.PreviewMouseDown += ChooseWeek;
                
                brdWeek.Child = tbWeek;
                Children.Add(brdWeek);
            }
            CurrentWeek = CurW;
          
        }

        private void HoverOverWeek(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
            {
                tb = (TextBlock)sender;
            }
            else
            {
                Border brd = (Border)sender;
                tb = (TextBlock)brd.Child;
            }
            tb.FontSize = 32;
        }

        private void LeaveWeek(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
            {
                tb = (TextBlock)sender;
            }
            else
            {
                Border brd = (Border)sender;
                tb = (TextBlock)brd.Child;
            }
            if (int.Parse(tb.Text) != CurrentWeek)
                tb.FontSize = 24;
        }

        private void ChooseWeek(object sender, MouseEventArgs e)
        {
            TextBlock tb;
            if ((sender) is TextBlock)
                tb = (TextBlock) sender;
            else
            {
                Border brd = (Border) sender;
                tb = (TextBlock) brd.Child;
            }

            if (int.Parse(tb.Text) != CurrentWeek)
                CurrentWeek = int.Parse(tb.Text);

            else if (CurrentWeek != 0)
                Reset();
        }

        public void Reset()
        {
            CurrentWeek = 0;
            foreach (Border brd in Children)
            {
                TextBlock tb = (TextBlock) brd.Child;
                if (brd.Tag == null)
                {
                    tb.Foreground = Brushes.Black;
                    tb.FontSize = 24;
                    tb.FontWeight = FontWeights.Normal;
                }
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the PropertyChange event for the property specified
        /// </summary>
        /// <param name="propertyName">Property name to update. Is case-sensitive.</param>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
            this.RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged(String propertyName)
        {
            VerifyPropertyName(propertyName);
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion // INotifyPropertyChanged Members

        #region Degugging
        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public virtual void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
        #endregion
    }
}

