using System.Collections.Generic;
using UnityEngine;

namespace PrefabViewer.Hierarchy
{
    public class HierarchyNode
    {
        public string Id { get; }
        public string Name { get; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public HierarchyNode Parent { get; }
        public List<HierarchyNode> Children { get; } = new List<HierarchyNode>();
        public bool IsExpanded { get; set; } = true;
        public int Depth { get; }

        public HierarchyNode(GameObject go, HierarchyNode parent, string id, int depth)
        {
            GameObject = go;
            Transform = go.transform;
            Name = go.name;
            Parent = parent;
            Id = id;
            Depth = depth;
        }

        public static HierarchyNode BuildTree(GameObject root)
        {
            return BuildNode(root, null, "0", 0);
        }

        static HierarchyNode BuildNode(GameObject go, HierarchyNode parent, string id, int depth)
        {
            var node = new HierarchyNode(go, parent, id, depth);
            for (var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                var childId = id + "/" + i;
                node.Children.Add(BuildNode(child, node, childId, depth + 1));
            }
            return node;
        }

        public IEnumerable<HierarchyNode> FlattenVisible()
        {
            yield return this;
            if (!IsExpanded)
                yield break;
            foreach (var child in Children)
            {
                foreach (var n in child.FlattenVisible())
                    yield return n;
            }
        }
    }
}
