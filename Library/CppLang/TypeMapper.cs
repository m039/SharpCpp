using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCpp
{
    public class TypeMapper
    {
        public interface IncludeFinder
        {
            string FindInclude(string type);
        }

        ISet<string> _includes;
        IncludeFinder _includeFinder;

        internal TypeMapper(IncludeFinder includeFinder, ISet<string> includes)
        {
            this._includes = includes;
            this._includeFinder = includeFinder;
        }

        public string ValueOf(YType type)
        {
            string include;

            if (type == YType.Int) {
                include = _includeFinder.FindInclude("int32_t");
                if (include != null) {
                    _includes.Add(include);
                    return "int32_t";
                }

                return "int";
            } else if (type == YType.Void) {
                return "void";
            } else if (type is YRefType) {
                var name = ((YRefType)type).Name;

                include = _includeFinder.FindInclude("shared_ptr");
                if (include != null) {
                    _includes.Add(include);

                    include = _includeFinder.FindInclude(name);
                    if (include != null) {
                        _includes.Add(include);
                        return $"std::shared_ptr<{name}>";
                    }
                }

                throw new TException("Can't find include for referenced type");
            }

            throw new TException("Unsupported type");
        }

        public string ValueOf(YParameter @var)
        {
            return ValueOf(@var.Type) + " " + @var.Name;
        }

        public string ValueOf(YParameter[] @params)
        {
            if (@params == null || @params.Length < 0) {
                return "";
            }

            int length = @params.Length;

            var b = new StringBuilder();

            b.Append(ValueOf(@params[0]));

            for (var i = 1; i < length; i++) {
                b.Append(",");
                b.Append(ValueOf(@params[i]));
            }

            return b.ToString();
        }
    }
}
