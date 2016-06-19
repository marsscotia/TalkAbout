using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace TalkAbout.Triggers
{
    public class DeviceFamilyTrigger: StateTriggerBase
    {
        private string _currentDeviceFamily;
        private string _queriedDeviceFamily;

        public String DeviceFamily
        {
            get
            {
                return _queriedDeviceFamily;
            }
            set
            {
                _queriedDeviceFamily = value;
                _currentDeviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                SetActive(_queriedDeviceFamily == _currentDeviceFamily);
            }
        }
    }
}
