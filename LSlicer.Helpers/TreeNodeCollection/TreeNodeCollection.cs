using LSlicer.Data.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Helpers
{
    public class TreeNodeCollection<T> : IEnumerable<T>
        where T : IIdentifier, ICopyable<T>
    {
        public TreeNode<T> Root;

        public TreeNodeCollection(T init)
        {
            Root = new TreeNode<T>(null, init);
        }

        public int Count() => Count(Root);

        public bool TryAttach(T value, int attachId)
        {
            Maybe<TreeNode<T>> findedItem = FindById(attachId, Root);
            if (findedItem.TryGetValue(out TreeNode<T> toAttachNode))
            {
                toAttachNode.Children.Add(new TreeNode<T>(toAttachNode, value));
                return true;
            }
            return false;
            //findedItem.Match(node => { node.Children.Add(new TreeNode<T>(node, value)); return true; }, () => false);
        }

        public bool TryRemove(T value)
        {
            var result = FindById(value.Id, Root);
            TreeNode<T> node;
            if (result.TryGetValue(out node))
            {
                var parent = node.Parent;
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    if (parent.Children[i] == node)
                    {
                        parent.Children.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            else return false;
        }

        public bool TryUpdate(T value) =>
            FindById(value.Id, Root)
                .Match(node => { node.Value = value; return true; }, () => false);

        public IEnumerable<T> GetChildren(int id)
        {
            var nodeResult = FindById(id, Root);
            TreeNode<T> node;
            if (nodeResult.TryGetValue(out node))
            {
                using (var enumerator = new TreeNodeEnumerator<T>(node))
                {
                    return enumerator.GetNodes().Select(x => x.Value);
                }
            }
            else return new List<T>();
        }

        public IEnumerator GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new TreeNodeEnumerator<T>(Root);

        public int CopyNode(int id, int newId) => 
            FindById(id, Root)
            .Match(node => CopyNode(node, newId), () => id);

        private int CopyNode(TreeNode<T> node, int copyId) 
        {
            var copy = node.Value.Copy(copyId);
            var copyNode = new TreeNode<T>(node.Parent, copy);
            node.Parent.Children.Add(copyNode);
            return copyId;
        }

        private Maybe<TreeNode<T>> FindById(int id, TreeNode<T> root)
        {
            if (root.Value.Id == id) 
                return root;
            foreach (var child in root.Children)
                if (child.Value.Id == id) 
                    return child;
            return Maybe.None;
        }

        private int Count(TreeNode<T> root)
        {
            int result = root.Children.Count;
            foreach (var child in root.Children)
                result = result + Count(child);
            return result;
        }
    }

}
