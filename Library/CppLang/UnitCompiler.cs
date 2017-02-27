using System;
using System.Text;
using SharpCpp;

namespace SharpCpp
{
    public abstract class UnitCompiler
    {
        // To hide YSyntaxWalker's functions from descedants of UnitWalker.
        class YSyntaxWalkerAdapter : YSyntaxWalker
        {
            UnitWalker _walker;

            public YSyntaxWalkerAdapter(UnitWalker walker)
            {
                _walker = walker;
            }

            #region YSyntaxWalker overrides

            protected override void Visit(YNamespace @namespace)
            {
                _walker.Visit(_walker._builder, @namespace);
            }

            protected override void Visit(YClass @class)
            {
                _walker.Visit(_walker._builder, @class);
            }

            protected override void Visit(YField @field)
            {
                _walker.Visit(_walker._builder, @field);
            }

            protected override void Visit(YMethod @method)
            {
                _walker.Visit(_walker._builder, @method);
            }

            protected override void OnPreWalk()
            {
                base.OnPreWalk();

                _walker._builder.Clear();
                _walker.InitBuilder(_walker._builder);
            }

            protected override void OnPostWalk()
            {
                base.OnPostWalk();

                _walker.FinalizeBuilder(_walker._builder);
            }

            #endregion
        }

        public class UnitWalker
        {
            protected const string PublicMark = "{{public}}";

            protected const string PrivateMark = "{{private}}";

            protected const string IncludesMark = "{{includes}}";

            internal readonly StringBuilder _builder = new StringBuilder();

            protected YClass Class;

            protected UnitWalker(YClass @class)
            {
                Class = @class;
            }

            internal protected virtual void InitBuilder(StringBuilder builder) { }

            internal protected virtual void Visit(StringBuilder builder, YNamespace @namespace) { }

            internal protected virtual void Visit(StringBuilder builder, YClass @class) { }

            internal protected virtual void Visit(StringBuilder builder, YField @field) { }

            internal protected virtual void Visit(StringBuilder builder, YMethod @method) { }

            internal protected virtual void FinalizeBuilder(StringBuilder builder) { }

            internal string GeneratedText()
            {
                return _builder.ToString();
            }
        }

        public string Compile(YRoot root, GenerationUnit unit)
        {
            var walker = CreateUnitWalker(unit.Class);
            var walkerAdapter = new YSyntaxWalkerAdapter(walker);

            walkerAdapter.Walk(root);

            return walker.GeneratedText();
        }

        public abstract UnitWalker CreateUnitWalker(YClass @class);
    }
}
