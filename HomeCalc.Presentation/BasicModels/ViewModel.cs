using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HomeCalc.Presentation.BasicModels
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        protected Logger logger;
        public event PropertyChangedEventHandler PropertyChanged;

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
        private IDictionary<string, ICommand> commandCache = new Dictionary<string, ICommand>();
        public void AddCommand(string name, ICommand command)
        {
            if (string.IsNullOrEmpty(name) || command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (commandCache.Keys.Contains(name))
            {
                throw new ArgumentException(string.Format("Command {0} already exists", name));
            }
            commandCache[name] = command;
        }
        public void RemoveCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("command");
            }
            commandCache.Remove(name);
        }
    }
    public partial class ViewModel : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            ICommand command;
            if (!commandCache.TryGetValue(binder.Name, out command))
            {
                logger.Warn("Binding property not found: {0}", binder.Name);
                return base.TryGetMember(binder, out result);
            }
            return (result = command) != null;
        }
    }
}
