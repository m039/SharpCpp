using System;
namespace SharpCpp
{
    public class YClass : YSymbol
    {
        // hack: nested classes are not supported
        public bool IsNested = false;

        public YClass(string name)
        {
            Name = name;
        }

        public bool HasPublicFields()
        {
            // Note: not performance frendly, better to use AddChild
            foreach (var node in Nodes) {
                if (node is YField && (((YField)node).Visibility == YVisibility.Public)) {
                    return true;
                }
            }

            return false;
        }

        public bool HasPrivateFields()
        {
            return !HasPublicFields();
        }
    }
}
