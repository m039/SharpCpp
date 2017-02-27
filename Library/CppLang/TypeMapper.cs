using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCpp
{
    public class TypeMapper
    {
        ISet<string> _includes;

        internal TypeMapper(ISet<string> includes)
        {
            _includes = includes;
        }

        public string ValueOf(YType type)
        {
            if (type == YType.Int) {
                _includes.Add("<cstdint>");
                return "int32_t";
            } else if (type == YType.Void) {
                return "void";
            } else {
                throw new TException("Unsupported type");
            }
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
