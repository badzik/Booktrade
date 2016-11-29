using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class SettingsViewModel
    {
        public UserSettingsModel SettingsModel { get; set; }
        public string OldPassword {get; set;}
        public string NewPassword { get; set; }
        public string NewPasswordRepeat { get; set; }
    }
}