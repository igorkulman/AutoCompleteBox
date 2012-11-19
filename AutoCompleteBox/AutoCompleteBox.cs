using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace AutoCompleteBox
{
    public sealed class AutoCompleteBox : Control
    {
        TextBox tb = null;
        ListBox lb = null;
        Grid g = null;
        bool isShowing = true;

        public AutoCompleteBox()
        {
            this.DefaultStyleKey = typeof(AutoCompleteBox);
        }

       

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.tb = GetTemplateChild("tbChild") as TextBox;
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

                    this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    if (String.IsNullOrWhiteSpace(this.tb.Text) || this.ItemsSource == null || this.ItemsSource.Count == 0)
                        return;

                    var sel = (from d in this.ItemsSource where d.ToLower().RemoveDiacritics().StartsWith(this.tb.Text.ToLower().RemoveDiacritics()) select d);

                    if (sel != null && sel.Count() > 0)
                    {
                        this.lb.ItemsSource = sel;
                        this.lb.Visibility = Windows.UI.Xaml.Visibility.Visible;

                        this.lb.SelectionChanged += lb_SelectionChanged;
                    }
                });

                tb.LostFocus += tb_LostFocus;
                tb.GotFocus += tb_GotFocus;
                
            }

            if (this.ItemsSource != null)
                this.lb.ItemsSource = ItemsSource;

            this.g.MaxHeight = this.MaxHeight;
        }

      

        void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            isShowing = true;
        }

        void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            HideAndSelectFirst();
        }

        private void HideAndSelectFirst()
        {
            isShowing = false;
            this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (String.IsNullOrWhiteSpace(this.tb.Text) || this.ItemsSource == null || this.ItemsSource.Count == 0)
                return;

            if (!ItemsSource.Contains(tb.Text))
            {
                var sel = (from d in this.ItemsSource where d.ToLower().StartsWith(this.tb.Text.ToLower()) select d);
                tb.Text = sel.FirstOrDefault();
            }
        }

        public ICollection<string> ItemsSource
        {
            get { return (ICollection<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Text { get { return (this.tb == null ? null : this.tb.Text); } }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ICollection<string>), typeof(AutoCompleteBox), new PropertyMetadata(null));

        
        

        void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.tb.TextChanged -= tb_TextChanged;

            this.tb.Text = (string)this.lb.SelectedValue;

            //this.tb.TextChanged += tb_TextChanged;

            this.lb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
