using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestApp.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TestViewModel _model;

        public MainPage()
        {
            this.InitializeComponent();
            _model = new TestViewModel();
            this.DataContext = _model;
            var items = new List<string>() { "a", "ab","abc", "b", "c", "d", "e", "f", "g" };
            ABBox.ItemsSource = items;

            //custom search function
            ABBox.SearchFunction = (itemInList, typedText) => { return itemInList.ToLower().Contains(typedText.ToLower()); };
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }
    }
}
