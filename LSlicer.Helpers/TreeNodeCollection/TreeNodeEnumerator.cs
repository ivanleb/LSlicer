using LSlicer.Data.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LSlicer.Helpers
{
    public class TreeNodeEnumerator<T> : IEnumerator<T>
        where T : IIdentifier, ICopyable<T>
    {
        private IList<TreeNode<T>> _nodeList = new List<TreeNode<T>>();
        private IEnumerator<TreeNode<T>> _nodeListEnumerator;

        public TreeNodeEnumerator(TreeNode<T> root)
        {
            Traverse(root);
            _nodeListEnumerator = _nodeList.GetEnumerator();
        }

        public T Current => _nodeListEnumerator.Current.Value != null
            ? _nodeListEnumerator.Current.Value
            : throw new NullReferenceException("Current value is null.");

        object IEnumerator.Current => Current;

        public void Dispose() => _nodeListEnumerator.Dispose();

        public bool MoveNext() => _nodeListEnumerator.MoveNext();

        public void Reset() => _nodeListEnumerator.Reset();

        public IEnumerable<TreeNode<T>> GetNodes() => _nodeList;

        private void Traverse(TreeNode<T> root)
        {
            var children = new Queue<TreeNode<T>>();
            children.Enqueue(root);

            while (children.Count > 0)
            {
                TreeNode<T> node = children.Dequeue();

                if (node.Parent != null)
                    _nodeList.Add(node);

                foreach (var child in node.Children)
                    children.Enqueue(child);
            }
        }
    }
}
