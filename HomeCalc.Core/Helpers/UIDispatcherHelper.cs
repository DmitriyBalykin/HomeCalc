using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeCalc.Core.Helpers
{
    public class UIDispatcherHelper
    {
        public static void CallOnUIThread(Action action)
        {
            if (Application.Current == null)
            {
                action();
                return;
            }
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }
    }
}
