using System;
using System.Runtime.Serialization;

namespace Avalarin.Exceptions {
    public class InvalidPasswordException : Exception {
        public string UserName { get; set; }

        public InvalidPasswordException(string userName) : base("Invalid password for user '" + userName + "'.") {
            UserName = userName;
        }

        public InvalidPasswordException(string userName, string message) : base(message) {
            UserName = userName;
        }

        public InvalidPasswordException(string userName,string message, Exception innerException) : base(message, innerException) {
            UserName = userName;
        }

        protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) {
            UserName = info.GetString("UserName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("UserName", UserName);
        }
    }
}