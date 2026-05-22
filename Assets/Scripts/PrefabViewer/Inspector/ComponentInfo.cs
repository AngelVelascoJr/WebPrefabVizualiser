using System;
using System.Collections.Generic;

namespace PrefabViewer.Inspector
{
    [Serializable]
    public class ComponentInfo
    {
        public string typeName;
        public bool enabled;
        public List<PropertyInfoDto> properties = new List<PropertyInfoDto>();
    }
}
