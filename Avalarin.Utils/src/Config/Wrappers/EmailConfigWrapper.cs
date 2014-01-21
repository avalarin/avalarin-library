namespace Avalarin.Config.Wrappers {
    internal class EmailConfigWrapper : ConfigWrapperBase {
        private readonly SmtpConfigWrapper _smtp;
        
        public EmailConfigWrapper(IConfigurationProvider config) : base(config) {
            _smtp = new SmtpConfigWrapper(config);
        }

        public SmtpConfigWrapper Smtp {
            get { return _smtp; }
        }

        public string From {
            get { return GetValue("Email.From", "mailer@example.com"); }
            set { SetValue("Email.From", value); }
        }
    }
}