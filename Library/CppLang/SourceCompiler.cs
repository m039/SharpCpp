using System;
using System.Text;

namespace SharpCpp
{
    public class SourceUnitCompiler : UnitCompiler
    {
        class SourceWalker : UnitWalker
        {
            public SourceWalker(YClass @class) : base(@class) { }

            StringBuilder _constructor = new StringBuilder();
            bool _constructorInited;

            protected override void InitBuilder(StringBuilder builder)
            {
                base.InitBuilder(builder);

                _constructor.Clear();
                _constructor.Append($"{ Class.Name }::{ Class.Name }()");
                _constructorInited = true;

                builder.Append($"#include \"{ Class.Name }.hpp\"\n");
                builder.Append(IncludesMark);

            }

            protected override void Visit(StringBuilder builder, YNamespace @namespace)
            {
                builder.Append("using namespace " + @namespace.Name + ";");
            }

            protected override void Visit(StringBuilder builder, YField field)
            {
                base.Visit(builder, field);

                if (field.Value is YConstExpr) {
                    if (_constructorInited) {
                        _constructor.Append(":");
                        _constructorInited = false;
                    } else {
                        _constructor.Append(",");
                    }

                    _constructor.Append(field.Name + "{" + field.Value + "}");
                }
            }

            protected override void FinalizeBuilder(StringBuilder builder)
            {
                base.FinalizeBuilder(builder);

                builder.Replace(IncludesMark, "");

                // or should be placed before any functions
                _constructor.Append("{}");
                builder.Append(_constructor);
            }
        }

        public override UnitWalker CreateUnitWalker(YClass @class)
        {
            return new SourceWalker(@class);
        }
    }
}
