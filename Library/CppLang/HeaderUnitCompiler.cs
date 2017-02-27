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
            #region aliases

            const YVisibility Public = YVisibility.Public;

            const YVisibility Private = YVisibility.Private;

            const BuilderId.BuilderType Field = BuilderId.BuilderType.Field;

            const BuilderId.BuilderType Constructor = BuilderId.BuilderType.Constructor;

            #endregion

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

                ClearBuilders();

                GetBuilder(Public, Constructor)
                    .Append(@class.Name + "();");

                builder.Append(PrivateMark);
                builder.Append(PublicMark);

                _nestedLevels.Add(@class);
            }

            protected override void Visit(StringBuilder builder, YField field)
            {
                StringBuilder b;

                if (field.Visibility == Public) {
                    b = GetBuilder(Public, Field);
                } else if (field.Visibility == Private) {
                    b = GetBuilder(Private, Field);
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

                var privateFields = GetBuilder(Private, Field);

                if (privateFields.Length > 0) {
                    builder.Replace(PrivateMark, "private:\n" + privateFields);
                } else {
                    builder.Replace(PrivateMark, "");
                }

                // add default constructor

                var publicFields = GetBuilder(Public, Field);

                builder.Replace(PublicMark,
                                 "public:\n" +
                                 GetBuilder(Public, Constructor) +
                                 (publicFields.Length > 0 ? publicFields.ToString() : ""));
            }

            #region builders functions

            // todo aggregate builders as class builders

            Dictionary<BuilderId, StringBuilder> _builders = new Dictionary<BuilderId, StringBuilder>();

            void ClearBuilders()
            {
                _builders.Clear();
            }

            StringBuilder GetBuilder(YVisibility visibility, BuilderId.BuilderType type)
            {
                // Note: a lot of calls to 'new'

                var id = new BuilderId(visibility, type);

                if (_builders.ContainsKey(id)) {
                    return _builders[id];
                }

                var builder = new StringBuilder();
                _builders[id] = builder;
                return builder;
            }

            /// Helper class
            class BuilderId
            {
                internal enum BuilderType
                {
                    Field, Constructor
                }

                readonly YVisibility visibility;

                readonly BuilderType type;

                internal BuilderId(YVisibility visibility, BuilderType type)
                {
                    this.visibility = visibility;
                    this.type = type;
                }

                public override bool Equals(object obj)
                {
                    return Equals(obj as BuilderId);
                }

                public bool Equals(BuilderId builderId)
                {
                    return builderId != null && builderId.visibility == visibility && builderId.type == type;
                }

                public override int GetHashCode()
                {
                    int hashCode = 1;

                    hashCode = 37 * hashCode + visibility.GetHashCode();
                    hashCode = 37 * hashCode + type.GetHashCode();

                    return hashCode;
                }
            }

            #endregion
        }

        public override UnitWalker CreateUnitWalker(YClass @class)
        {
            return new HeaderWalker(@class);
        }
    }
}
