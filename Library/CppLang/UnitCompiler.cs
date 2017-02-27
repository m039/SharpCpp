using System;
using System.Text;
using SharpCpp;

namespace SharpCpp
{
    public abstract class UnitCompiler
    {
        public class UnitWalker : YSyntaxWalker
        {
            protected const string PublicMark = "{{public}}";

            protected const string PrivateMark = "{{private}}";

            protected const string IncludesMark = "{{includes}}";

            readonly StringBuilder _builder = new StringBuilder();

            protected YClass Class;

            protected UnitWalker(YClass @class)
            {
                Class = @class;
            }

#region YSyntaxWalker overrides

            protected override void Visit(YNamespace @namespace) {
                Visit(_builder, @namespace);
            }

            protected override void Visit(YClass @class) {
                Visit(_builder, @class);
            }

            protected override void Visit(YField @field) {
                Visit(_builder, @field);
            }

            protected override void Visit(YMethod @method)
            {
                Visit(_builder, @method);
            }

#endregion

            protected virtual void InitBuilder(StringBuilder builder) { }

            protected virtual void Visit(StringBuilder builder, YNamespace @namespace) { }

            protected virtual void Visit(StringBuilder builder, YClass @class) { }

            protected virtual void Visit(StringBuilder builder, YField @field) { }

            protected virtual void Visit(StringBuilder builder, YMethod @method) { }

            protected virtual void FinalizeBuilder(StringBuilder builder) { }

            protected override void OnPreWalk()
            {
                base.OnPreWalk();

                _builder.Clear();
                InitBuilder(_builder);
            }

            protected override void OnPostWalk()
            {
                base.OnPostWalk();

                FinalizeBuilder(_builder);
            }

            internal string GeneratedText()
            {
                return _builder.ToString();
            }
        }

        public string Compile(YRoot root, GenerationUnit unit)
        {
            var walker = CreateUnitWalker(unit.Class);

            walker.Walk(root);

            return walker.GeneratedText();
        }

        public abstract UnitWalker CreateUnitWalker(YClass @class);
    }
}
