using System;
namespace SharpCpp
{
    using System.Text;
    using SharpCpp;
    using System.Collections.Generic;

    public class HeaderUnitCompiler : UnitCompiler
    {
        class HeaderWalker : UnitWalker
        {
            // true if needed closed bracket
            readonly List<YSymbol> _nestedLevels = new List<YSymbol>();

            readonly HashSet<string> _includes = new HashSet<string>();

            public HeaderWalker(YClass @class) : base(@class) { }

            protected override void InitBuilder(StringBuilder builder)
            {
                base.InitBuilder(builder);

                builder.Append("#pragma once\n");
                builder.AppendLine();
                builder.Append(IncludesMark);
            }

            protected override void Visit(StringBuilder builder, YNamespace @namespace)
            {
                builder.Append("namespace " + @namespace.Name + "{");
                _nestedLevels.Add(@namespace);
            }

            class BuilderId
            {
                internal enum BuilderType
                {
                    Field, Constructor
                }
                
                YVisibility visibility;
                BuilderType type;

                internal BuilderId(YVisibility visibility, BuilderType type)
                {
                    this.visibility = visibility;
                    this.type = type;
                }
            }

            Dictionary<BuilderId, StringBuilder> _builders = new Dictionary<BuilderId, StringBuilder>();

            // todo aggregate builders as class builders

            StringBuilder _publicFields = new StringBuilder();

            StringBuilder _privateFields = new StringBuilder();

            StringBuilder _contructors = new StringBuilder();

            StringBuilder GetBuilder(YVisibility visibility, BuilderId.BuilderType type)
            {
                var id = new BuilderId(visibility, type);
                if (_builders.ContainsKey(id)) {
                    return _builders[id];
                }

                var builder = new StringBuilder();
                _builders[id] = builder;
                return builder;
            }

            protected override void Visit(StringBuilder builder, YClass @class)
            {
                if (!@class.IsNested) {
                    var nestedLevelsCount = _nestedLevels.Count - 1;
                    if (nestedLevelsCount >= 0 && _nestedLevels[nestedLevelsCount] is YClass) {
                        CloseClass(builder);
                        _nestedLevels.RemoveAt(nestedLevelsCount);
                    }
                }

                builder.Append("class " + @class.Name + "{");

                _publicFields.Clear();
                _privateFields.Clear();
                _contructors.Clear();

                _contructors.Append(@class.Name + "();");

                builder.Append(PrivateMark);
                builder.Append(PublicMark);

                _nestedLevels.Add(@class);
            }

            protected override void Visit(StringBuilder builder, YField field)
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
                        b.Append("=" + field.Value);
                    } else {
                        throw new TException("Unsupported expr");
                    }
                }

                b.Append(";");
            }

            protected override void FinalizeBuilder(StringBuilder builder)
            {
                base.FinalizeBuilder(builder);

                for (int i = _nestedLevels.Count - 1; i >= 0; i--) {
                    var l = _nestedLevels[i];
                    if (l is YClass) {
                        CloseClass(builder);
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

            void CloseClass(StringBuilder builder)
            {
                builder.Append("};");

                if (_privateFields.Length > 0) {
                    builder.Replace(PrivateMark, "private:\n" + _privateFields);
                } else {
                    builder.Replace(PrivateMark, "");
                }

                // add default constructor

                builder.Replace(PublicMark,
                                 "public:\n" +
                                 _contructors +
                                 (_publicFields.Length > 0 ? _publicFields.ToString() : ""));
            }
        }

        public override UnitWalker CreateUnitWalker(YClass @class)
        {
            return new HeaderWalker(@class);
        }
    }
}
