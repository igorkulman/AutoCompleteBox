using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinRTXamlToolkit.Controls;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace AutoCompleteBox
{
    public sealed class AutoCompleteBox : Control
    {
        #region fields        
        WatermarkTextBox tb = null;
        ListBox lb = null;
        Grid g = null;
        bool isShowing = true;
        #endregion

        #region dependency properties
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ICollection<string>), typeof(AutoCompleteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty SearchFunctionProperty = DependencyProperty.Register("SearchFunction", typeof(Func<string, string, bool>), typeof(AutoCompleteBox), new PropertyMetadata(new Func<string, string, bool>((itemInList, typedText) => { return itemInList.ToLower().RemoveDiacritics().StartsWith(typedText.ToLower().RemoveDiacritics()); })));
        #endregion

        #region properties
        public ICollection<string> ItemsSource
        {
            get { return (ICollection<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        public Func<string, string, bool> SearchFunction
        {
            get { return (Func<string, string, bool>)GetValue(SearchFunctionProperty); }
            set { SetValue(SearchFunctionProperty, value); }
        }
        #endregion

        //public Func<string, string, bool> SearchFunction = (itemInList, typedText) => { return itemInList.ToLower().RemoveDiacritics().StartsWith(typedText.ToLower().RemoveDiacritics()); };

        public AutoCompleteBox()
        {
            this.DefaultStyleKey = typeof(AutoCompleteBox);
        }

        public event EventHandler itemChosen;
       
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.tb = GetTemplateChild("tbChild") as WatermarkTextBox;
            this.lb = GetTemplateChild("lbChild") as ListBox;

            this.g = GetTemplateChild("spContainer") as Grid;

            if (tb != null && this.lb != null)
            {
                //tb.TextChanged += tb_TextChanged;
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;
                var keys = Observable.FromEventPattern<KeyRoutedEventArgs>(tb, "KeyUp").Throttle(TimeSpan.FromSeconds(0.5), CoreDispatcherScheduler.Current);
                keys.Subscribe(evt =>
                {
                    if (evt.EventArgs.Key == Windows.System.VirtualKey.Enter)
                    {
                        HideAndSelectFirst();                        
                        return;
                    }

                    if (!isShowing) return;

                    this.lb.SelectionChanged -= lb_SelectionChanged;

                    this.lb.Visibility = Visibility.Collapsed;

                    if (String.IsNullOrWhiteSpace(this.tb.Text) || this.ItemsSource == null || this.ItemsSource.Count == 0)
                        return;

                    //var sel = (from d in this.ItemsSource where d.ToLower().RemoveDiacritics().StartsWith(this.tb.Text.ToLower().RemoveDiacritics()) select d);
                    var sel = (from d in this.ItemsSource where SearchFunction(d,this.tb.Text) select d);

                    if (sel != null && sel.Count() > 0)
                    {
                        this.lb.ItemsSource = sel;
                        this.lb.Visibility = Visibility.Visible;

                        this.lb.SelectionChanged += lb_SelectionChanged;
                    }
                });

                tb.LostFocus += (s, e) =>
                {
                    HideAndSelectFirst();
                };
                tb.GotFocus += (s, e) =>
                {
                    isShowing = true;
                };
                
            }

            if (this.ItemsSource != null)
                this.lb.ItemsSource = ItemsSource;

            this.g.MaxHeight = this.MaxHeight;
        }
           
      

        private void HideAndSelectFirst()
        {
            isShowing = false;
            this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (String.IsNullOrWhiteSpace(this.tb.Text) || this.ItemsSource == null || this.ItemsSource.Count == 0)
                return;

            if (!ItemsSource.Contains(tb.Text, new Compare()))
            {
                var sel = (from d in this.ItemsSource where d.ToLower().StartsWith(this.tb.Text.ToLower()) select d);
                Text = sel.FirstOrDefault() ?? String.Empty;
            }
            else
            {
                Text = tb.Text;
                onItemChosen();
            }
        }
        

        void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tb.Text = (string)this.lb.SelectedValue;
            this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            onItemChosen();
        }

        private void onItemChosen()
        {
            ItemEventArgs args = new ItemEventArgs(tb.Text);
            itemChosen(this, args);
        }

        class Compare : IEqualityComparer<String>
        {
            public bool Equals(String x, String y)
            {
                if (x.ToLower() == y.ToLower())
                    return true;
                else 
                    return false;
            }
            public int GetHashCode(String codeh)
            {
                return 0;
            }
        }

        public class ItemEventArgs : EventArgs
        {
            public ItemEventArgs(String item)
            {
                this.item = item;
            }
            public String item { get; set; }
        }
    }
}
