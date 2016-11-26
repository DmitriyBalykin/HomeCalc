using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Services.Messages
{
    public enum MessageType
    {
        TYPES_UPDATED,
        TYPES_LOADED,
        TYPES_UNLOADED,

        SUBTYPES_UPDATED,
        SUBTYPES_LOADED,
        SUBTYPES_UNLOADED,

        HISTORY_UPDATED,

        STATUS_CHANGED,

        PROGRESS_UPDATED,
        PROGRESS_STARTED,
        PROGRESS_FINISHED,

        UPDATES_AVAILABLE,

        RATING_PANEL_SHOW,
        RATING_PANEL_HIDE
    }
}
