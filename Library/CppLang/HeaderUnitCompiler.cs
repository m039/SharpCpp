using System;
namespace SharpCpp
{
    using System.Text;
    using SharpCpp;
    using System.Collections.Generic;

    public class HeaderUnitCompiler : UnitCompiler
    {
        class HeaderWalker : YSyntaxWalker
        {
            readonly StringBuilder builder  = new StringBuilder();

            // true if needed closed bracket
            List<YSymbol> _nestedLevels = new List<YSymbol>();

            HashSet<string> _includes = new HashSet<string>();

            const string PublicMark = "{{public}}";

            const string PrivateMark = "{{private}}";

            const string IncludesMark = "{{includes}}";

            public HeaderWalker()
            {
                builder.Append("#pragma once\n");
                builder.AppendLine();
                builder.Append(IncludesMark);
            }

            protected override void Visit(YNamespace @namespace)
            {
                builder.Append("namespace " + @namespace.Name + "{");
                _nestedLevels.Add(@namespace);
            }

            // todo aggregate builders as class builders

            public StringBuilder _publicFields = new StringBuilder();

            public StringBuilder _privateFields = new StringBuilder();

            protected override void Visit(YClass @class)
            {
                if (!@class.IsNested) {
                    var nestedLevelsCount = _nestedLevels.Count - 1;
                    if (nestedLevelsCount >= 0 && _nestedLevels[nestedLevelsCount] is YClass) {
                        CloseClass();
                        _nestedLevels.RemoveAt(nestedLevelsCount);
                    }
                }

                builder.Append("class " + @class.Name + "{");

                _publicFields.Clear();
                _privateFields.Clear();

                if (@class.HasPrivateFields()) {
                    builder.Append(PrivateMark);
                }

                if (@class.HasPublicFields()) {
                    builder.Append(PublicMark);
                }

                _nestedLevels.Add(@class);
            }

            protected override void Visit(YField field)
            {
                StringBuilder b;

                if (field.Visibility == YVisibility.Public) {
                    b = _publicFields;
                } else if (field.Visibility == YVisibility.Private) {
                    b = _privateFields;
                } else {
                    throw new TException("Unsupported access");
                }

                if (field.Type == YType.Int) {
                    _includes.Add("<cstdint>");
                    b.Append("int32_t");
                } else {
                    throw new TException("Unsupported type");
                }

                b.Append(" " + field.Name);

                if (field.Value != null) {
                    if (field.Value is YConstExpr) {
                        b.Append("= " + field.Value);
                    } else {
                        throw new TException("Unsupported expr");
                    }
                }

                b.Append(";");
            }

            public string GeneratedText()
            {
                CloseBrackets();

                return builder.ToString();
            }

            void CloseBrackets()
            {
                for (int i = _nestedLevels.Count - 1; i >= 0; i--) {
                    var l = _nestedLevels[i];
                    if (l is YClass) {
                        CloseClass();
                    } else if (l is YNamespace) {
                        builder.Append("}");
                    } else {
                        throw new TException("Unsupported nesting");
                    }
                }

                _nestedLevels.Clear();

                // append includes
                var sb = new StringBuilder();
                foreach (var include in _includes) {
                    sb.Append("#include " + include + "\n");
                }
                if (sb.Length > 0) {
                    sb.AppendLine();
                }

                builder.Replace(IncludesMark, sb.ToString());
            }

            private void CloseClass()
            {
                builder.Append("};");

                if (_publicFields.Length > 0) {
                    builder.Replace(PrivateMark, "private:\n" + _privateFields);
                } else {
                    builder.Replace(PrivateMark, "");
                }

                if (_publicFields.Length > 0) {
                    builder.Replace(PublicMark, "public:\n" + _publicFields);
                } else {
                    builder.Replace(PublicMark, "");
                }
            }
        }

        public override string Compile(YRoot root, GenerationUnit unit)
        {
            var walker = new HeaderWalker();

            walker.Walk(root);

            return walker.GeneratedText();
        }
    }
}
