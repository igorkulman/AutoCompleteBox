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
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList<string>), typeof(AutoCompleteBox), new PropertyMetadata(null));
        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty SearchFunctionProperty = DependencyProperty.Register("SearchFunction", typeof(Func<string, string, bool>), typeof(AutoCompleteBox), new PropertyMetadata(new Func<string, string, bool>((itemInList, typedText) => { return itemInList.ToLower().RemoveDiacritics().StartsWith(typedText.ToLower().RemoveDiacritics()); })));
        #endregion

        #region properties
        public IList<string> ItemsSource
        {
            get { return (IList<string>)GetValue(ItemsSourceProperty); }
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

        public event Action<String> ItemChosen;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tb = GetTemplateChild("tbChild") as WatermarkTextBox;
            lb = GetTemplateChild("lbChild") as ListBox;
            g = GetTemplateChild("spContainer") as Grid;

            if (tb == null || lb == null || g == null) return;


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

                lb.SelectionChanged -= lb_SelectionChanged;
                lb.Visibility = Visibility.Collapsed;

                if (String.IsNullOrWhiteSpace(this.tb.Text) || this.ItemsSource == null || this.ItemsSource.Count == 0)
                    return;

                //var sel = (from d in this.ItemsSource where d.ToLower().RemoveDiacritics().StartsWith(this.tb.Text.ToLower().RemoveDiacritics()) select d);
                var sel = (from d in this.ItemsSource where SearchFunction(d, this.tb.Text) select d);

                if (sel.Any())
                {
                    lb.ItemsSource = sel;
                    lb.Visibility = Visibility.Visible;
                    lb.SelectionChanged += lb_SelectionChanged;
                }
            });

            tb.LostFocus += (s, e) => HideAndSelectFirst();
            tb.GotFocus += (s, e) =>
            {
                isShowing = true;
            };

            if (ItemsSource != null)
            {
                lb.ItemsSource = ItemsSource;
            }

            g.MaxHeight = MaxHeight;
        }



        private void HideAndSelectFirst()
        {
            isShowing = false;
            lb.Visibility = Visibility.Collapsed;

            if (String.IsNullOrWhiteSpace(tb.Text) || ItemsSource == null || ItemsSource.Count == 0)
                return;

            if (!ItemsSource.Contains(tb.Text, new Compare()))
            {
                var sel = (from d in ItemsSource where SearchFunction(d, this.tb.Text) select d);
                Text = sel.FirstOrDefault() ?? tb.Text;
            }
            else
            {
                Text = tb.Text;
                OnItemChosen();
            }
        }


        void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.tb.Text = (string)this.lb.SelectedValue;
            this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            OnItemChosen();
        }

        private void OnItemChosen()
        {
            if (ItemChosen != null)
            {
                ItemChosen(tb.Text);
            }
        }
    }
}
