using System;
using System.Runtime.Serialization;

namespace Avalarin.Exceptions {
    public class NotFoundException : Exception {

        public string EntityName { get; set; }

        public NotFoundException(string entityName) : base(entityName  + " not found.") {
            EntityName = entityName;
        }

        public NotFoundException(string entityName, string message)
            : base(message) {
            EntityName = entityName;
        }

        public NotFoundException(string entityName, string message, Exception innerException)
            : base(message, innerException) {
            EntityName = entityName;
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
            EntityName = info.GetString("EntityName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("EntityName", EntityName);
        }

    }
}