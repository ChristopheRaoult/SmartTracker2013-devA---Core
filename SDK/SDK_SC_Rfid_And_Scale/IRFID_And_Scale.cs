using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataClass;

namespace SDK_SC_Rfid_And_Scale
{
    interface IRFID_And_Scale
    {
        bool IsRfidConnected { get; }
        bool IsScaleConnected { get; }

        rfidPluggedInfo[] getRFIDpluggedDevice();
        bool ConnectRFIDandScale(string serialRFID, string portCom);
        retCode getTagAndWeight(int timeoutScale, out string tagUID, out double scaleWeight);
    }
}
