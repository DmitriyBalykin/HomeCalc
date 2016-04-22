using HomeCalc.Core;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DbService;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Services;
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
