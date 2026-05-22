using System;

namespace PrefabViewer.Inspector
{
    [Serializable]
    public class PropertyInfoDto
    {
        public string name;
        public string typeName;
        public string value;
        public PropertyDisplayKind displayKind = PropertyDisplayKind.Default;
        public bool boolValue;
        public float vectorX;
        public float vectorY;
        public float vectorZ;
        public float vectorW;
    }
}
