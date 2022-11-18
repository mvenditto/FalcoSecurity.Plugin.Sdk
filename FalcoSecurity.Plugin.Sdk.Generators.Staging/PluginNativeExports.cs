// auto-generated
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;

namespace FalcoSecurity.Plugin.Sdk.DummyPlugin
{
    using EventSourceInstanceOpaquePtr = IntPtr;

    public static unsafe class Plugin_NativeExports
    {
        private static IntPtr _pluginName;
        private static IntPtr _pluginDescription;
        private static IntPtr _pluginRequiredApiVersion;
        private static IntPtr _pluginVersion;
        private static IntPtr _pluginContact;


        private static IntPtr _eventSourceName;
        private static IntPtr _openParamsJsonArray;
        private static ulong _instanceId = 0;
        private static IDictionary<ulong, IEventSourceInstance> _instanceTable;



        private static IntPtr _pluginLastError = IntPtr.Zero;


        private static IntPtr _fieldsJsonArray;
        private static IntPtr _extractionSourcesJsonArray;
        private static ExtractionRequestPool _requestPool;
        private static int _numFields;


        private static uint _pluginId = 0;
        private static Plugin _plugin = new();

        // must be non-null, or some funcs like list_opem_params will early-exit
        private static void* _pluginState = NativeMemory.Alloc(1);

        static Plugin_NativeExports()
        {
            /* 'demographic' strings the returned strings must remain valid until the plugin is destroyed. */

            var pluginInfo = (FalcoPluginAttribute)Attribute.GetCustomAttribute(
                typeof(Plugin),
                typeof(FalcoPluginAttribute));

            _pluginId = pluginInfo.Id;
            _pluginDescription = Marshal.StringToCoTaskMemUTF8(pluginInfo.Description);
            _pluginRequiredApiVersion = Marshal.StringToCoTaskMemUTF8(pluginInfo.RequiredApiVersion);
            _pluginVersion = Marshal.StringToCoTaskMemUTF8(pluginInfo.Version);
            _pluginContact = Marshal.StringToCoTaskMemUTF8(pluginInfo.Contacts);
            _pluginName = Marshal.StringToCoTaskMemUTF8(pluginInfo.Name);




            var eventSource = (IEventSource)_plugin;
            _eventSourceName = Marshal.StringToCoTaskMemUTF8(eventSource.EventSourceName);
            var openParams = eventSource.OpenParameters;
            if (openParams != null && openParams.Count() > 0)
            {
                var openParamsJson = JsonSerializer.Serialize(openParams);
                _openParamsJsonArray = Marshal.StringToCoTaskMemUTF8(openParamsJson);
            }
            else
            {
                _openParamsJsonArray = Marshal.StringToCoTaskMemUTF8("[]");
            }



            var fieldExtractor = (IFieldExtractor)_plugin;
            var fields = fieldExtractor.Fields;

            if (fields != null && fields.Count() > 0)
            {
                _numFields = fields.Count();
                var fieldsJson = JsonSerializer.Serialize(fields);
                _fieldsJsonArray = Marshal.StringToCoTaskMemUTF8(fieldsJson);
            }
            else
            {
                _numFields = 0;
                _fieldsJsonArray = Marshal.StringToCoTaskMemUTF8("[]");
            }

            if (fieldExtractor.EventSourcesToExtract != null
                && fieldExtractor.EventSourcesToExtract.Count() > 0)
            {
                var eventSourcesToExtractJson = JsonSerializer.Serialize(
                    fieldExtractor.EventSourcesToExtract);
                _extractionSourcesJsonArray = Marshal.StringToCoTaskMemUTF8(
                    eventSourcesToExtractJson);
            }
            else
            {
                _extractionSourcesJsonArray = Marshal.StringToCoTaskMemUTF8("[]");
            }

        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_required_api_version", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetRequiredApiVersion()
        {
            return _pluginRequiredApiVersion;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_last_error", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetLastError(void* pluginState)
        {
            var lastError = _plugin.LastError ?? "cannot get error message: plugin last error not set.";
            _pluginLastError = Marshal.StringToCoTaskMemUTF8(lastError);
            return _pluginLastError;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_name", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginName()
        {
            return _pluginName;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_description", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginDescription()
        {
            return _pluginDescription;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_contact", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginAuthorsContact()
        {
            return _pluginContact;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_version", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetPluginVersion()
        {
            return _pluginVersion;
        }



        [UnmanagedCallersOnly(EntryPoint = "plugin_init", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void* Init(IntPtr configString, IntPtr returnCode)
        {

            _instanceTable = new Dictionary<ulong, IEventSourceInstance>();
            _requestPool = new ExtractionRequestPool(_numFields * 2);

            Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);
            return _pluginState;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_destroy", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void Destroy(void* pluginState)
        {
            Marshal.FreeCoTaskMem(_pluginName);
            Marshal.FreeCoTaskMem(_pluginRequiredApiVersion);
            Marshal.FreeCoTaskMem(_pluginVersion);
            Marshal.FreeCoTaskMem(_pluginContact);
            Marshal.FreeCoTaskMem(_pluginDescription);
            NativeMemory.Free(pluginState);




            Marshal.FreeCoTaskMem(_fieldsJsonArray);
            try
            {
                if (typeof(IDisposable).IsAssignableFrom(_requestPool.Pool.GetType()))
                {
                    ((IDisposable)_requestPool.Pool).Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }


            Marshal.FreeCoTaskMem(_eventSourceName);
            Marshal.FreeCoTaskMem(_openParamsJsonArray);
            try
            {
                foreach (var i in _instanceTable.Values)
                {
                    i.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

        }


        [UnmanagedCallersOnly(EntryPoint = "plugin_get_id", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static uint GetId()
        {
            return _pluginId;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_event_source", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetEventSourceName()
        {
            return _eventSourceName;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_open", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static EventSourceInstanceOpaquePtr Open(void* pluginState, IntPtr paramsString, IntPtr returnCode)
        {
            try
            {
                IList<OpenParam>? openParams = null;

                if (paramsString != IntPtr.Zero)
                {
                    var openParamsStr = Marshal.PtrToStringUTF8(paramsString);
                    if (string.IsNullOrEmpty(openParamsStr) == false)
                    {
                        openParams = JsonSerializer.Deserialize<IList<OpenParam>>(openParamsStr);
                    }
                }
                var instance = _plugin.Open(openParams);

                _instanceTable.Add(
                    Interlocked.Increment(ref _instanceId),
                    instance);

                Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);

                return (IntPtr)_instanceId;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Failure);
                _plugin.LastError = ex.ToString();
                return IntPtr.Zero;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_close", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void Close(void* pluginState, EventSourceInstanceOpaquePtr instance)
        {
            var instanceId = (ulong)instance;
            try
            {
                var evtSourceInstance = _instanceTable[instanceId];
                _plugin.Close(evtSourceInstance);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                _plugin.LastError = ex.ToString();
            }
            finally
            {
                _instanceTable.Remove(instanceId);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_list_open_params", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr ListOpenParams(UIntPtr pluginState, IntPtr returnCode)
        {
            Marshal.WriteInt32(returnCode, (int)PluginReturnCode.Success);
            return _openParamsJsonArray;
        }

        /*
        [UnmanagedCallersOnly(EntryPoint = "plugin_get_progress", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetReadProgress(void* pluginState, EventSourceInstanceOpaquePtr instance, IntPtr progress)
        {
            Marshal.WriteInt32(progress, 0);
            return IntPtr.Zero;
        }
        */

        [UnmanagedCallersOnly(EntryPoint = "plugin_event_to_string", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr EventToString(void* pluginState, IntPtr pluginEvent)
        {
            return _eventSourceName;
        }

        /// <remarks>
        /// When returning events via plugin_next_batch, 
        /// both the array of structs and the data payloads inside each struct 
        /// must remain valid until the next call to plugin_next_batch.
        /// </remarks>
        [UnmanagedCallersOnly(EntryPoint = "plugin_next_batch", CallConvs = new[] { typeof(CallConvCdecl) })]

        public static int GetNextEventsBatch(void* pluginState, EventSourceInstanceOpaquePtr instance, IntPtr numEvents, IntPtr events)
        {
            EventSourceInstanceContext? ctx = null;
            try
            {
                var instanceId = (ulong)instance;
                var evtSourceInstance = _instanceTable[instanceId];
                ctx = evtSourceInstance.NextBatch();

                Marshal.WriteInt32(numEvents, (int)ctx.BatchEventsNum);

                events = (IntPtr)evtSourceInstance.EventBatch.UnderlyingArray;

                if (ctx.IsEof)
                {
                    return (int)PluginReturnCode.Eof;
                }
                else if (ctx.HasTimeout)
                {
                    return (int)(ctx.BatchEventsNum > 0
                        ? PluginReturnCode.Success // partial batch flushed?
                        : PluginReturnCode.Timeout);
                }
                else if (ctx.HasFailure)
                {
                    Marshal.WriteInt32(numEvents, 0);
                    Marshal.WriteIntPtr(events, IntPtr.Zero);
                    return (int)PluginReturnCode.Failure;
                }

                return (int)PluginReturnCode.Success;

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                Marshal.WriteInt32(numEvents, 0);
                Marshal.WriteIntPtr(events, IntPtr.Zero);
                return (int)PluginReturnCode.Failure;
            }
            finally
            {
                ctx?.Reset();
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "plugin_get_fields", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetFields()
        {
            return _fieldsJsonArray;
        }

        [UnmanagedCallersOnly(EntryPoint = "plugin_get_extract_event_sources", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static IntPtr GetExtractEventSources()
        {
            return _extractionSourcesJsonArray;
        }

        /// <remarks>
        /// When returning extracted string values via plugin_extract_fields, 
        /// every extracted string must remain valid until the next call to plugin_extract_fields
        /// NOTE: freed on extractRequest.SetPtr
        /// </remarks>
        [UnmanagedCallersOnly(EntryPoint = "plugin_extract_fields", CallConvs = new[] { typeof(CallConvCdecl) })]
        public static int ExtractFields(void* pluginState, IntPtr pluginEvent, int numFields, IntPtr fieldsPtr)
        {
            try
            {
                var fieldExtractor = (IFieldExtractor)_plugin;
                var eventReader = new EventReader((PluginEvent*)pluginEvent);
                var fields = (PluginExtractField*)fieldsPtr;
                for (var i = 0; i < numFields; i++)
                {
                    var f = &fields[i];
                    var extractRequest = _requestPool.Pool.Get();
                    extractRequest.SetPtr(f);
                    fieldExtractor.Extract(extractRequest, eventReader);
                    _requestPool.Pool.Return(extractRequest);
                }
                return (int)PluginReturnCode.Success;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                _plugin.LastError = ex.ToString();
                return (int)PluginReturnCode.Failure;
            }
        }

    }
}