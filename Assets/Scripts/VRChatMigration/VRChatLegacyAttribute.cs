using System;

namespace VRChatMigration
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class VRChatLegacyAttribute : Attribute
    {
        public string VrcTypeName { get; }
        public string Description { get; }

        public VRChatLegacyAttribute(string vrcTypeName, string description = "")
        {
            VrcTypeName = vrcTypeName;
            Description = description;
        }
    }
}
