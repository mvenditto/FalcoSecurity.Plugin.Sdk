using FalcoSecurity.Plugin.Sdk.Fields;
using System.Runtime.InteropServices;
using Xunit;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    public class ExtractFieldTest
    {
        [Fact]
        unsafe public void ExtractionRequestSetU64ArrayValue()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*) fieldPtr;

                f->FieldList = 1;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

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
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetU64Value()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                er.SetValue(42u);

                Assert.Equal(42u, f->Result->Unsigned64);

                Assert.Equal(42u, ((ulong*)f->Result)[0]);
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetStringValue()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                er.SetValue("TEST");

                var str = Marshal.PtrToStringUTF8(f->Result->String);

                Assert.Equal("TEST", str);

                str = Marshal.PtrToStringUTF8(((IntPtr*)f->Result)[0]);

                Assert.Equal("TEST", str);
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ExtractionRequestSetStringArrayValue()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*) fieldPtr;

                f->FieldList = 1;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                var results = Enumerable
                    .Range(0, 512)
                    .Select(i => $"This is String {i}!")
                    .ToArray()!;

                er.SetValue(results);

                var rawResults = (IntPtr*)f->Result;

                for (var i = 0; i < results.Length; i++)
                {
                    string s = Marshal.PtrToStringUTF8(rawResults[i])!;
                    Assert.Equal($"This is String {i}!", s);
                }
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingU64ToStringTypeField()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTtypeString;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(42u));
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingStringToU64TypeField()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(string.Empty));
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ShouldThrowArgumentExceptionIfWritingListToNonListField()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                using var er = new ExtractionRequest();

                er.SetPtr(fieldPtr);

                Assert.Throws<ArgumentException>(
                    () => er.SetValue(new ulong[] { 1, 2, 3, 4 }));
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }

        [Fact]
        unsafe public void ExtractionRequestPoolReturnMustNotFreeResourcesTest()
        {
            var fieldPtr = IntPtr.Zero;
            try
            {

                fieldPtr = Marshal.AllocHGlobal(sizeof(PluginExtractField));

                var f = (PluginExtractField*)fieldPtr;

                f->FieldList = 0;
                f->ArgPresent = 0;
                f->FieldId = 0;
                f->FieldType = (uint)PluginFieldType.FTypeUint64;

                var pool = new ExtractionRequestPool(1);

                var r1 = pool.Pool.Get();

                Assert.NotNull(r1);

                Assert.Equal(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                r1.SetPtr(fieldPtr);

                Assert.NotEqual(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                pool.Pool.Return(r1);

                Assert.NotEqual(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);

                r1.Free();

                Assert.Equal(IntPtr.Zero, (IntPtr)r1.UnderlyingPtr);
            }
            finally
            {
                if (fieldPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fieldPtr);
                }
            }
        }
    }
}