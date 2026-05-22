using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabViewer
{
    [CreateAssetMenu(fileName = "PrefabCatalog", menuName = "Prefab Viewer/Catalog")]
    public class PrefabCatalog : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string displayName;
            public string category;
            public GameObject prefab;
        }

        [SerializeField]
        List<Entry> entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => entries;

        public int Count => entries.Count;

        public Entry GetEntry(int index)
        {
            if (index < 0 || index >= entries.Count)
                return null;
            return entries[index];
        }
    }
}
