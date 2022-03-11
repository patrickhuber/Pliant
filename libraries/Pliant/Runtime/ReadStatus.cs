using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Runtime
{
    public enum ReadStatus
    {
        // Failure means a read failed and could not be recovered
        Failure,
        // Success means the read succeeded without error
        Success,
        // Recovery is a successful read, but there was an error that was recovered from
        Recovery
    }
}
