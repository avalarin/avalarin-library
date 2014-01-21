using System;
using System.Runtime.Serialization;

namespace Avalarin.Exceptions {
    [Serializable]
    public class DublicateValueException : Exception {
        public string FieldName { get; set; }

        public DublicateValueException(string fieldName) : base ("Dublicate value.") {
            FieldName = fieldName;
        }

        public DublicateValueException(string fieldName, string message)
            : base(message) {
                FieldName = fieldName;
        }

        public DublicateValueException(string fieldName, string message, Exception inner) : base(message, inner) {
            FieldName = fieldName;
        }

        protected DublicateValueException(SerializationInfo info, StreamingContext context) : base(info, context) {
            FieldName = info.GetString("FieldName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("FieldName", FieldName);
            base.GetObjectData(info, context);
        }
    }
}