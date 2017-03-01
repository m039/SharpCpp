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

            const BuilderId.BuilderType Method = BuilderId.BuilderType.Method;

            #endregion

            readonly List<YSymbol> _nestedLevels = new List<YSymbol>();

            readonly HashSet<string> _includes = new HashSet<string>();

            TypeMapper _typeMapper;

            public HeaderWalker(YClass @class, TypeMapper.IncludeFinder finder) : base(@class, finder) { 
                _typeMapper = new TypeMapper(finder, _includes);
            }

            internal protected override void InitBuilder(StringBuilder builder)
            {
                base.InitBuilder(builder);

                builder.Append("#pragma once\n");
                builder.AppendLine();
                builder.Append(IncludesMark);
            }

            internal protected override void Visit(StringBuilder builder, YNamespace @namespace)
            {
                builder.Append("namespace " + @namespace.Name + "{");
                _nestedLevels.Add(@namespace);
            }

            internal protected override void Visit(StringBuilder builder, YClass @class)
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

            internal protected override void Visit(StringBuilder builder, YField field)
            {
                StringBuilder b;

                if (field.Visibility == Public) {
                    b = GetBuilder(Public, Field);
                } else if (field.Visibility == Private) {
                    b = GetBuilder(Private, Field);
                } else {
                    throw new TException("Unsupported access");
                }

                b.Append(_typeMapper.ValueOf(field.Type));
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

            internal protected override void Visit(StringBuilder builder, YMethod method)
            {
                StringBuilder b;

                if (method.Visibility == Private) {
                    b = GetBuilder(Private, Method);
                } else if (method.Visibility == Public) {
                    b = GetBuilder(Public, Method);
                } else {
                    throw new TException("Unsupported visibility");
                }

                if (method.IsVirtual) {
                    b.Append("virtual ");
                }

                if (method is YDestructor) {
                    // todo move this declaration to the bottom of the generated file

                    b.Append("~" + Class.Name + "()");
                } else {
                    b.Append(_typeMapper.ValueOf(method.Signature.ReturnType));
                    b.Append(" ");
                    b.Append(method.Signature.Name);

                    b.Append("(");
                    b.Append(_typeMapper.ValueOf(method.Signature.Parameters));
                    b.Append(")");
                }

                if (method.IsPure) {
                    b.Append("=0");
                }

                b.Append(";");
            }

            internal protected override void FinalizeBuilder(StringBuilder builder)
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

                foreach (var p in new[] {
                    new KeyValuePair<YVisibility, string>(Private, PrivateMark),
                    new KeyValuePair<YVisibility, string>(Public, PublicMark)
                }) {
                    var b = new StringBuilder();

                    var fields = GetBuilder(p.Key, Field);
                    var methods = GetBuilder(p.Key, Method);
                    var constructors = GetBuilder(p.Key, Constructor);

                    if (fields.Length > 0 || methods.Length > 0 || constructors.Length > 0) {
                        b.Append(p.Key.GetName() + ":\n");
                        b.Append(fields);
                        b.Append(methods);
                        b.Append(constructors);
                    }

                    builder.Replace(p.Value, b.ToString());
                }
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
                    Field, Constructor, Method
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

        public override UnitWalker CreateUnitWalker(YClass @class, TypeMapper.IncludeFinder finder)
        {
            return new HeaderWalker(@class, finder);
        }
    }
}
