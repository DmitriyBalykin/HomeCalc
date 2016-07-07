using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    public class RoutineService
    {
        public static void RunStartupRoutines()
        {
        }

        public static void RunClosingRoutines()
        {
            BackupService.BackupDatabase();
        }
    }
}
