using System;

namespace Avalarin.Config.Wrappers {
    internal class SmtpConfigWrapper : ConfigWrapperBase {
        public SmtpConfigWrapper(IConfigurationProvider config) : base(config) {
        }

        public string Host {
            get { return GetValue("Email.Host", "smtp.example.com"); }
            set { SetValue("Email.Host", value); }
        }
        public int Port {
            get { return GetValue("Email.Port", 25); }
            set { SetValue("Email.Port", value); }
        }
        public string UserName {
            get { return GetValue<String>("Email.UserName", null); }
            set { SetValue("Email.UserName", value); }
        }
        public string Password {
            get { return GetValue<String>("Email.Password", null); }
            set { SetValue("Email.Password", value); }
        }
    }
}