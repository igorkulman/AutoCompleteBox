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
    }
}
