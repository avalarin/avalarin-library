using System;
using System.Runtime.Serialization;

namespace Avalarin.Exceptions {
    public class UserLockedException : Exception {
        public string UserName { get; set; }

        public UserLockedException(string userName) : base("User '" + userName + "' locked.") {
            UserName = userName;
        }

        public UserLockedException(string userName, string message) : base(message) {
            UserName = userName;
        }

        public UserLockedException(string userName, string message, Exception innerException) : base(message, innerException) {
            UserName = userName;
        }

        protected UserLockedException(SerializationInfo info, StreamingContext context) : base(info, context) {
            UserName = info.GetString("UserName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("UserName", UserName);
        } 
    }
}