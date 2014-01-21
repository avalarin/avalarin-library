namespace Avalarin.Config.Wrappers {
    public class PopConfigWrapper : ConfigWrapperBase {
        public PopConfigWrapper(IConfigurationProvider config) : base(config) {
        }

        public string HostName {
            get {
                return GetValue("POP.HostName", "");
            }
            set {
               SetValue("POP.HostName", value);
            }
        }

        public int Port {
            get {
                return GetValue("POP.Port", 110);
            }
            set {
                SetValue("POP.Port", value);
            }
        }

        public bool UseSsl {
            get {
                return GetValue("POP.UseSsl", false);
            }
            set {
                SetValue("POP.UseSsl", value);
            }
        }

        public string UserName {
            get {
                return GetValue<string>("POP.UserName", null);
            }
            set {
                SetValue("POP.UserName", value);
            }
        }

        public string Password {
            get {
                return GetValue<string>("POP.Password", null);
            }
            set {
                SetValue("POP.Password", value);
            }
        }
    }
}