using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp.Classes
{
    public class TestViewModel: BaseViewModel
    {
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }
        private string _Text;

        public Func<string,string,bool> SearchFunction
        {
            get { return _SearchFunction; }
            set
            {
                if (_SearchFunction != value)
                {
                    _SearchFunction = value;
                    NotifyPropertyChanged("SearchFunction");
                }
            }
        }
        private Func<string,string,bool> _SearchFunction;
        

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
            SearchFunction = (itemInList, typedText) => { return itemInList.ToLower().Contains(typedText.ToLower()); };
        }
    }
}
