//using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HomeCalc.ChartsLib.ViewModels
{
    public partial class UserControlViewModel : DependencyObject, INotifyPropertyChanged
    {
        //protected Logger logger;
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControlViewModel()
        {
            //logger = new Logger(this.GetType().ToString());
        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (property == null)
            {
                return;
            }
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }
    }
}
