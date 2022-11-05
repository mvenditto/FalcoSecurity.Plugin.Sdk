using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falco.Plugin.Sdk
{
    public enum PluginFieldType
    {
        FTypeUint64 = 8,
        FTtypeString = 9
    };

    public enum PluginReturnCode
    {
        Success = 0,
        Failure = 1,
        Timeout = -1,
        Eof = 2,
        NotSupported = 3,
    }

    public enum PluginSchemaType
    {
        None = 0,
        Json = 1
    }
}
