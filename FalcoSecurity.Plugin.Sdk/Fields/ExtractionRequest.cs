using System.Runtime.InteropServices;

namespace FalcoSecurity.Plugin.Sdk.Fields
{
    public unsafe class ExtractionRequest : IExtractionRequest
    {
        private PluginExtractField* _extractFieldPtr;

        private string? _fieldName;

        private string? _argKey;

        private FieldResult* _resBuf;

        private int _resBufLen = MinResultBufferLen;

        private int _resultsNum;

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

        public void SetPtr(PluginExtractField* ptr)
        {
            if (_dirty)
            {
                Free();
            }

            _extractFieldPtr = ptr;
            _resBuf = (FieldResult*) NativeMemory.Alloc(
                elementCount: MinResultBufferLen,
                elementSize: (nuint)sizeof(FieldResult)
            );
            _resultsNum = 0;
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

            _resultsNum = 1;
            ((nint*)_resBuf)[0] = Marshal.StringToCoTaskMemUTF8(str);
            _extractFieldPtr->Result = _resBuf;
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
                if (IsList && FieldType == PluginFieldType.FTtypeString && _resultsNum > 0)
                {
                    for (var i = 0; i < _resultsNum; i++)
                    {
                        Marshal.FreeCoTaskMem(_resBuf[i].String);
                    }
                }

                NativeMemory.Free(_resBuf);
                
                _resBufLen = strBuff.Length;
                _resultsNum = _resBufLen;
                _resBuf = (FieldResult*)NativeMemory.Alloc(
                    elementCount: (nuint)_resBufLen,
                    elementSize: (nuint)sizeof(FieldResult)
                );
            }

            var strPtrBuff = (IntPtr*)_resBuf;

            for (var i = 0; i < strBuff.Length; i++)
            {
                strPtrBuff[i] = Marshal.StringToCoTaskMemUTF8(strBuff[i]);
            }

            _resultsNum = strBuff.Length;
            _extractFieldPtr->Result = _resBuf;
            _extractFieldPtr->ResultLen = (uint)_resultsNum;
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

            _resultsNum = 1;
            ((ulong*)_resBuf)[0] = u64;
            _extractFieldPtr->Result = _resBuf;
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
                if (IsList && FieldType == PluginFieldType.FTtypeString && _resBufLen > 0)
                {
                    for (var i = 0; i < _resBufLen; i++)
                    {
                        Marshal.FreeCoTaskMem(_resBuf[i].String);
                    }
                }

                NativeMemory.Free(_resBuf);

                _resBufLen = u64Buff.Length;
                _resultsNum = _resBufLen;
                _resBuf = (FieldResult*)NativeMemory.Alloc(
                    elementCount: (nuint)_resBufLen,
                    elementSize: (nuint)sizeof(FieldResult)
                );
            }

            _resultsNum = u64Buff.Length;
            var resBufSpan = new Span<ulong>(_resBuf, _resBufLen);
            u64Buff.CopyTo(resBufSpan);
            _extractFieldPtr->Result = _resBuf;
            _extractFieldPtr->ResultLen = (uint) u64Buff.Length;
        }

        public void Free()
        {
            if (IsList && FieldType == PluginFieldType.FTtypeString && _resultsNum > 0)
            {
                for (var i = 0; i < _resultsNum; i++)
                {
                    Marshal.FreeCoTaskMem(_resBuf[i].String);
                }
            }

            NativeMemory.Free(_resBuf);

            _extractFieldPtr = null;
            _fieldName = null;
            _argKey = null;
            _resBufLen = 0;
            _dirty = false;
        }

        public void Dispose() 
        {
            Free();
            GC.SuppressFinalize(this);
        }
    }
}
