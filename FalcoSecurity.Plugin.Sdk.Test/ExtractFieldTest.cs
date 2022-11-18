using FalcoSecurity.Plugin.Sdk.Fields;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    public class ExtractFieldTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public ExtractFieldTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        
        [Fact]
        unsafe public void ExtractionRequestSetU64ArrayValue()
        {
            void* fieldPtr = null;
            try
            {
                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*) fieldPtr;

                f->FieldList = 1;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                var results = Enumerable
                    .Range(0, 512)
                    .Select(Convert.ToUInt64)
                    .ToArray()!;

                results[0] = ulong.MaxValue;
                results[1] = ulong.MinValue;

                er.SetValue(results);

                var rawResults = (ulong*)f->Result;

                Assert.Equal(ulong.MaxValue, rawResults[0]);
                Assert.Equal(ulong.MinValue, rawResults[1]);

                for (var i = 2; i < results.Length; i++)
                {
                    ulong v = rawResults[i];
                    Assert.Equal((ulong) i, v);
                }
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetU64Value()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                er.SetValue(42u);

                Assert.Equal(42u, f->Result->Unsigned64);

                Assert.Equal(42u, ((ulong*)f->Result)[0]);
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetStringValue()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                er.SetValue("TEST");

                var str = Marshal.PtrToStringUTF8(f->Result->String);

                Assert.Equal("TEST", str);

                str = Marshal.PtrToStringUTF8(((IntPtr*)f->Result)[0]);

                Assert.Equal("TEST", str);
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetStringArrayValue()
        {
            void* fieldPtr = null;

            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 1;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                var results = Enumerable
                    .Range(0, 3)
                    .Select(i => $"This is String {i}!")
                    .ToArray()!;

                er.SetValue(results);

                var rawResults = (IntPtr*)f->Result;

                for (var i = 0; i < results.Length; i++)
                {
                    var r = rawResults[i];
                    string s = Marshal.PtrToStringUTF8(r)!;
                    Assert.Equal($"This is String {i}!", s);
                }
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingU64ToStringTypeField()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(42u));
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingStringToU64TypeField()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(string.Empty));
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingListToNonListField()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(f);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(new ulong[] { 1, 2, 3, 4 }));
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }

        [Fact]
        unsafe public void ExtractionRequestPoolReturnMustNotFreeResourcesTest()
        {
            void* fieldPtr = null;
            try
            {

                fieldPtr = NativeMemory.Alloc((nuint)sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                var pool = new ExtractionRequestPool(1);

                var r1 = pool.Pool.Get();

                Assert.NotNull(r1);

                Assert.Equal(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                r1.SetPtr(f);

                Assert.NotEqual(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                pool.Pool.Return(r1);

                Assert.NotEqual(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                r1.Free();

                Assert.Equal(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);
            }
            finally
            {
                NativeMemory.Free(fieldPtr);
            }
        }
    }
}