using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp.Classes
{
    public class TestViewModel: BaseViewModel
    {
        public ObservableCollection<string> Items { get; private set; } 

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }
        private string _text;

        public Func<string,string,bool> SearchFunction
        {
            get { return _searchFunction; }
            set
            {
                if (_searchFunction != value)
                {
                    _searchFunction = value;
                    NotifyPropertyChanged("SearchFunction");
                }
            }
        }
        private Func<string,string,bool> _searchFunction;
        

        public ICommand TestCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {

                    var dialog = new Windows.UI.Popups.MessageDialog("The value of AutoCompleteBox is " + this.Text);
                    dialog.ShowAsync();

                });
            }
        }

        public TestViewModel()
        {
            SearchFunction = (itemInList, typedText) => itemInList.ToLower().Contains(typedText.ToLower());
            Items = new ObservableCollection<string>(new[] { "a", "ab", "abc", "b", "c", "d", "e", "f", "g" });

        }
    }
}
