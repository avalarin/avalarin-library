using System;
using System.Runtime.Serialization;

namespace Avalarin.Exceptions {
    public class AccessDeniedException : Exception {
        public string EntityName { get; set; }

        public AccessDeniedException(string entityName)
            : base("Access denied for entity '" + entityName + "'.") {
            EntityName = entityName;
        }

        public AccessDeniedException(string entityName, string message)
            : base(message) {
                EntityName = entityName;
        }

        public AccessDeniedException(string entityName, string message, Exception innerException)
            : base(message, innerException) {
                EntityName = entityName;
        }

        protected AccessDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
                EntityName = info.GetString("EntityName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("EntityName", EntityName);
        }
    }
}