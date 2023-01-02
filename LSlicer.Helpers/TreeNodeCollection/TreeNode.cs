using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;

namespace LSlicer.Helpers
{
    public class TreeNode<T> : ICloneable where T : IIdentifier, ICopyable<T>
    {
        public TreeNode<T> Parent;
        public T Value;
        public IList<TreeNode<T>> Children;

        public TreeNode(TreeNode<T> parent, T value)
        {
            Parent = parent;
            Value = value;
            Children = new List<TreeNode<T>>();
        }

        public object Clone() => new TreeNode<T>(Parent, Value);
    }

}
