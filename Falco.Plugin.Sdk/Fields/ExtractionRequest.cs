using System.Runtime.InteropServices;

namespace Falco.Plugin.Sdk.Fields
{
    public unsafe class ExtractionRequest : IExtractionRequest
    {
        private PluginExtractField* _extractFieldPtr;

        private string? _fieldName;

        private string? _argKey;

        private IntPtr _resBuf;

        private int _resBufLen = MinResultBufferLen;

        private const int MinResultBufferLen = 512;

        public ulong FieldId => _extractFieldPtr->FieldId;

        public ulong ArgIdx => _extractFieldPtr->ArgIdx;

        public bool ArgPresent => _extractFieldPtr->ArgPresent == 1;

        public bool IsList => _extractFieldPtr->FieldList == 1;

        public PluginFieldType FieldType =>
            (PluginFieldType)_extractFieldPtr->FieldType;

        private bool _dirty = false;

        public PluginExtractField* UnderlyingPtr => _extractFieldPtr;

        public string? FieldName
        {
            get
            {
                _fieldName ??= Marshal.PtrToStringUTF8(_extractFieldPtr->Field);

                return _fieldName;
            }
        }

        public string? ArgKey
        {
            get
            {
                _argKey ??= Marshal.PtrToStringUTF8(_extractFieldPtr->ArgKey);

                return _argKey;
            }
        }

        public void SetPtr(IntPtr ptr)
        {
            SetPtr((PluginExtractField*)ptr);
        }
        public void SetPtr(PluginExtractField* ptr)
        {
            if (_dirty)
            {
                Free();
            }

            _extractFieldPtr = ptr;
            _resBuf = Marshal.AllocHGlobal(sizeof(FieldResult) * MinResultBufferLen);
            _fieldName = null;
            _argKey = null;
            _dirty = true;
        }

        public void SetValue(string str)
        {
            var fieldType = FieldType;
            var isList = IsList;

            if (fieldType != PluginFieldType.FTtypeString || isList == true)
            {
                throw new ArgumentException(
                    $"field type is not String but {fieldType} (islist={isList})");
            }

            _resBufLen = 1;
            ((nint*)_resBuf)[0] = Marshal.StringToCoTaskMemUTF8(str);
            _extractFieldPtr->Result = (FieldResult*)_resBuf;
            _extractFieldPtr->ResultLen = 1;
        }

        public void SetValue(ReadOnlySpan<string> strBuff)
        {
            var fieldType = FieldType;
            var isList = IsList;

            if (fieldType != PluginFieldType.FTtypeString || isList == false)
            {
                throw new ArgumentException(
                    $"field type is not String[] but {fieldType} (islist={isList})");
            }

            if (strBuff.Length > _resBufLen)
            {
                Marshal.FreeHGlobal(_resBuf);
                _resBufLen = strBuff.Length;
                _resBuf = Marshal.AllocHGlobal(sizeof(FieldResult) * _resBufLen);
            }

            var strPtrBuff = (IntPtr*)_resBuf;

            for (var i = 0; i < strBuff.Length; i++)
            {
                strPtrBuff[i] = Marshal.StringToCoTaskMemUTF8(strBuff[i]);
            }

            _extractFieldPtr->Result = (FieldResult*) _resBuf;
            _extractFieldPtr->ResultLen = (uint) strBuff.Length;
        }

        public void SetValue(ulong u64)
        {
            var fieldType = FieldType;
            var isList = IsList;

            if (fieldType != PluginFieldType.FTypeUint64 || isList == true)
            {
                throw new ArgumentException(
                    $"field type is not UInt64 but {fieldType} (islist={isList})");
            }
            _resBufLen = 1;
            ((ulong*)_resBuf)[0] = u64;
            _extractFieldPtr->Result = (FieldResult*)_resBuf;
            _extractFieldPtr->ResultLen = 1;
        }

        public void SetValue(ReadOnlySpan<ulong> u64Buff)
        {
            var fieldType = FieldType;
            var isList = IsList;

            if (fieldType != PluginFieldType.FTypeUint64 || isList == false)
            {
                throw new ArgumentException(
                    $"field type is not UInt64[] but {fieldType} (islist={isList})");
            }

            if (u64Buff.Length > _resBufLen)
            {
                Marshal.FreeHGlobal(_resBuf);
                _resBufLen = u64Buff.Length;
                _resBuf = Marshal.AllocHGlobal(sizeof(FieldResult) * _resBufLen);
            }

            var resBufSpan = new Span<ulong>((void*)_resBuf, _resBufLen);
            u64Buff.CopyTo(resBufSpan);
            _extractFieldPtr->Result =  (FieldResult*) _resBuf;
            _extractFieldPtr->ResultLen = (uint) u64Buff.Length;
        }

        public void Free()
        {
            if ((IntPtr)_extractFieldPtr != IntPtr.Zero)
            {
                if (IsList && FieldType == PluginFieldType.FTtypeString && _resBufLen > 0)
                {
                    var strs = (IntPtr*)_resBuf;

                    for (var i = 0; i < _resBufLen; i++)
                    {
                        Marshal.FreeHGlobal(strs[i]);
                    }
                }
            }
            
            _extractFieldPtr = (PluginExtractField*) IntPtr.Zero;
            _fieldName = null;
            _argKey = null;
            _resBufLen = 0;
            Marshal.FreeHGlobal(_resBuf);

            _dirty = false;
        }

        public void Dispose() 
        {
            Free();
            GC.SuppressFinalize(this);
        }
    }
}
