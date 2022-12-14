#ifdef _WIN32
    #include <windows.h>
    typedef HINSTANCE library_handle_t;
#else
    #include <dlfcn.h>
    typedef void* library_handle_t;
#endif

#include <stdio.h>
#include <stdlib.h>
#include <plugin_api.h>
#include <gtest/gtest.h>


#define SYM_RESOLVE(h, s) \
    *(void **)(&(h->api.s)) = getsym(h->handle, "plugin_"#s)

typedef enum
{
    CAP_NONE = 0,
    CAP_SOURCING = 1 << 0,
    CAP_EXTRACTION = 1 << 1
} plugin_caps_t;

typedef struct plugin_handle_t
{
#ifdef _WIN32
    HINSTANCE handle; ///< Handle of the dynamic library
#else
    void* handle; ///< Handle of the dynamic library
#endif
    plugin_api api; ///< The vtable method of the plugin that define its API
} plugin_handle_t;

static void* getsym(library_handle_t handle, const char* name)
{
#ifdef _WIN32
	return (void*) GetProcAddress(handle, name);
#else
	return (void*) dlsym(handle, name);
#endif
}

plugin_handle_t* plugin_load(const char* path)
{
    plugin_handle_t* ret = (plugin_handle_t*) calloc (1, sizeof(plugin_handle_t));

#ifdef _WIN32
    ret->handle = LoadLibrary(path);
    if(ret->handle == NULL)
    {
        DWORD flg = FORMAT_MESSAGE_ALLOCATE_BUFFER
            | FORMAT_MESSAGE_FROM_SYSTEM
            | FORMAT_MESSAGE_IGNORE_INSERTS;
        LPTSTR msg_buf = 0;
        if (FormatMessageA(flg, 0, GetLastError(), 0, (LPTSTR) &msg_buf, 0, NULL) && msg_buf)
        {
			fprintf(stderr, "%s\n", msg_buf);
            LocalFree(msg_buf);
        }
    }
#else
    ret->handle = dlopen(path, RTLD_LAZY);
#endif

    if (ret->handle == NULL)
    {
        return NULL;
    }
	
    SYM_RESOLVE(ret, get_required_api_version);
    SYM_RESOLVE(ret, get_version);
    SYM_RESOLVE(ret, get_last_error);
    SYM_RESOLVE(ret, get_name);
    SYM_RESOLVE(ret, get_description);
    SYM_RESOLVE(ret, get_contact);
    SYM_RESOLVE(ret, get_init_schema);
    SYM_RESOLVE(ret, init);
    SYM_RESOLVE(ret, destroy);
    SYM_RESOLVE(ret, get_id);
    SYM_RESOLVE(ret, get_event_source);
    SYM_RESOLVE(ret, open);
    SYM_RESOLVE(ret, close);
    SYM_RESOLVE(ret, next_batch);
    SYM_RESOLVE(ret, get_progress);
    SYM_RESOLVE(ret, list_open_params);
    SYM_RESOLVE(ret, event_to_string);
    SYM_RESOLVE(ret, get_fields);
    SYM_RESOLVE(ret, extract_fields);
    SYM_RESOLVE(ret, get_extract_event_sources);
    return ret;
}

void plugin_unload(plugin_handle_t* h)
{
    if (h)
    {
        if (h->handle)
        {
#ifdef _WIN32
            FreeLibrary(h->handle);
#else
            dlclose(h->handle);
#endif
        }
        free(h);
    }
}

plugin_caps_t plugin_get_capabilities(const plugin_handle_t* h)
{
    plugin_caps_t caps = CAP_NONE;

    if (h->api.get_id != NULL
        && h->api.get_event_source != NULL
        && h->api.open != NULL
        && h->api.close != NULL
        && h->api.next_batch != NULL)
    {
        caps = (plugin_caps_t)((uint32_t)caps | (uint32_t)CAP_SOURCING);
    }

    if (h->api.get_fields != NULL
        && h->api.extract_fields != NULL)
    {
        caps = (plugin_caps_t)((uint32_t)caps | (uint32_t)CAP_EXTRACTION);
    }

    return caps;
}

const char* PLUGIN_ALL_CAPS = 
    "../../test-plugins/PluginAll/bin/Release/net6.0/linux-x64/plugin_native.so";

const char* PLUGIN_EVT_SOURCE_ONLY =
    "../../test-plugins/PluginEventSourceOnly/bin/Release/net6.0/linux-x64/plugin_native.so";

const char* PLUGIN_FIELD_EXTRACTION_ONLY =
    "../../test-plugins/PluginFieldExtractionOnly/bin/Release/net6.0/linux-x64/plugin_native.so";

const int TEST_BATCH_SIZE = 10;
const uint64_t TEST_TIMESTAMP_VALUE = 18446744073709551615;

plugin_handle_t* _plugin_handle;
plugin_api _plugin;
void* _plugin_state;

void SetUpPlugin(const char* pluginPath) {
    _plugin_handle = plugin_load(pluginPath);
    _plugin = _plugin_handle->api;
    ss_plugin_rc ret = (ss_plugin_rc)0;
    _plugin_state = _plugin.init(NULL, &ret);
    // printf("SetUp: handle: 0x%X state: 0x%X\n", _plugin_handle, _plugin_state);
}

void TearDownPlugin()
{
    // printf("TearDown: handle: 0x%X state: 0x%X\n", _plugin_handle, _plugin_state);
    _plugin.destroy(_plugin_state);
    plugin_unload(_plugin_handle);
    _plugin_state = nullptr;
    _plugin_handle = nullptr;
}

class PluginBaseTest : public testing::Test
{
    public:    
        static void SetUpTestSuite()
        {
            SetUpPlugin(PLUGIN_ALL_CAPS);
        }

        static void TearDownTestSuite()
		{
            TearDownPlugin();
		}
};

class PluginEventSourceOnlyTest : public testing::Test
{
    public:    
        static void SetUpTestSuite()
        {
            SetUpPlugin(PLUGIN_EVT_SOURCE_ONLY);
        }

        static void TearDownTestSuite()
		{
            TearDownPlugin();
		}
};

class PluginFieldExtractionOnlyTest : public testing::Test
{
    public:
        static void SetUpTestSuite()
        {
            SetUpPlugin(PLUGIN_FIELD_EXTRACTION_ONLY);
        }

        static void TearDownTestSuite()
        {
            TearDownPlugin();
        }
};

#define ASSERT_SYM(a, s) \
    do { \
        ASSERT_NE(a.s, nullptr); \
    } while(0)

TEST_F(PluginBaseTest, PluginLoad) 
{
	ASSERT_NE(_plugin_handle, nullptr);
}

TEST_F(PluginBaseTest, PluginHasRequiredSymbols)
{
    ASSERT_SYM(_plugin, get_required_api_version);
    ASSERT_SYM(_plugin, get_version);
    ASSERT_SYM(_plugin, get_name);
    ASSERT_SYM(_plugin, get_description);
    ASSERT_SYM(_plugin, get_contact);
    ASSERT_SYM(_plugin, init);
    ASSERT_SYM(_plugin, destroy);
    ASSERT_SYM(_plugin, get_last_error);
}

TEST_F(PluginBaseTest, PluginGetName)
{
	ASSERT_STREQ(_plugin.get_name(), "test_plugin");
}

TEST_F(PluginBaseTest, PluginGetVersion)
{
	ASSERT_STREQ(_plugin.get_version(), "1.2.3");
}

TEST_F(PluginBaseTest, PluginGetContacts)
{
    ASSERT_STREQ(_plugin.get_contact(), "<test test@test.com>");
}

TEST_F(PluginBaseTest, PluginGetDescription)
{
    ASSERT_STREQ(_plugin.get_description(), "test_description_string!");
}

TEST_F(PluginBaseTest, PluginHasExpectedCaps)
{
    auto caps = (uint32_t) plugin_get_capabilities(_plugin_handle);
    ASSERT_EQ(caps && (uint32_t)CAP_SOURCING, true);
    ASSERT_EQ(caps && (uint32_t)CAP_EXTRACTION, true);
}

TEST_F(PluginBaseTest, PluginGetEventSource)
{
    ASSERT_STREQ(_plugin.get_event_source(), "test_eventsource");
}

TEST_F(PluginEventSourceOnlyTest, PluginGetId)
{
    ASSERT_EQ(_plugin.get_id(), 999);
}

TEST_F(PluginEventSourceOnlyTest, PluginGetEventSource2)
{
    ASSERT_STREQ(_plugin.get_event_source(), "test_eventsource");
}

TEST_F(PluginEventSourceOnlyTest, PluginGetNextBatch)
{
    ss_plugin_rc ret = SS_PLUGIN_SUCCESS;
    const char* openParams = "";
    void* instance = _plugin.open(_plugin_state, openParams, &ret);

    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);

    uint32_t numEvents = 0;
    ss_plugin_event* m_input_plugin_batch_evts = NULL;

    ret = _plugin.next_batch(
        _plugin_state, 
        instance, 
        &numEvents, 
        &m_input_plugin_batch_evts);
 
    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);
    ASSERT_EQ(TEST_BATCH_SIZE, numEvents);

    ss_plugin_event* plugin_evt;
    for (int i = 0; i < TEST_BATCH_SIZE; i++)
    {
        plugin_evt = &(m_input_plugin_batch_evts[i]);
        // printf("%d[0x%X] %lu\n", i, plugin_evt, plugin_evt->ts);
        ASSERT_EQ(TEST_TIMESTAMP_VALUE, plugin_evt->ts);
        ASSERT_EQ(i, (int)*plugin_evt->data);
    }
}

TEST_F(PluginEventSourceOnlyTest, PluginHasEventSourcingCapOnly)
{
    auto caps = (uint32_t)plugin_get_capabilities(_plugin_handle);
    ASSERT_EQ(caps, CAP_SOURCING);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginHasEventSourcingCapOnly)
{
    auto caps = (uint32_t)plugin_get_capabilities(_plugin_handle);
    ASSERT_EQ(caps, CAP_EXTRACTION);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginGetFields)
{
    const char* fieldsJson = "[{\"name\":\"test.u64\",\"type\":\"uint64\",\"isList\":false,\"display\":\"\\u003Cu64\\u003E\",\"desc\":\"an integer field\"},{\"name\":\"test.str\",\"type\":\"string\",\"isList\":false,\"display\":\"\\u003Cstr\\u003E\",\"desc\":\"a string field\"},{\"name\":\"test.[u64]\",\"type\":\"uint64\",\"isList\":true,\"display\":\"\\u003Cu64[]\\u003E\",\"desc\":\"an integer[] field\"},{\"name\":\"test.[str]\",\"type\":\"string\",\"isList\":true,\"display\":\"\\u003Cstr[]\\u003E\",\"desc\":\"a string[] field\"}]";
    ASSERT_STREQ(_plugin.get_fields(), fieldsJson);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginGetExtractionSources)
{
    const char* sourcesJson = "[\"some_evt_source_1\",\"some_evt_source_2\"]";
    ASSERT_STREQ(_plugin.get_extract_event_sources(), sourcesJson);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginExtractFieldSingleUint64)
{
    ss_plugin_event* evt = (ss_plugin_event*) malloc(sizeof(ss_plugin_event));
    uint64_t value = 42;
    evt->evtnum = 1;
    evt->ts = 0;
    evt->data = (const uint8_t*)malloc(sizeof(uint64_t));
    evt->datalen = sizeof(uint64_t);
    memcpy((void*) evt->data, &value, sizeof(uint64_t));

    auto extractReq = (ss_plugin_extract_field*)malloc(sizeof(ss_plugin_extract_field));
    const char* field_name = "test.u64";
    extractReq->field = field_name;
    extractReq->field_id = 0;
    extractReq->flist = 0;
    extractReq->ftype = FTYPE_UINT64;

    ss_plugin_rc ret = _plugin.extract_fields(
        _plugin_state,
        evt,
        1,
        extractReq);

    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);
    ASSERT_STREQ(field_name, extractReq->field);
    ASSERT_EQ(1, extractReq->res_len);
    ASSERT_EQ(value, *extractReq->res.u64);

    free((void*) evt->data);
    free(evt);
    free(extractReq);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginExtractFieldUint64Array)
{
    ss_plugin_event* evt = (ss_plugin_event*)malloc(sizeof(ss_plugin_event));
    uint64_t value = 10;
    evt->evtnum = 1;
    evt->ts = 0;
    evt->data = (const uint8_t*)malloc(sizeof(uint64_t));
    evt->datalen = sizeof(uint64_t);
    memcpy((void*)evt->data, &value, sizeof(uint64_t));

    auto extractReq = (ss_plugin_extract_field*)malloc(sizeof(ss_plugin_extract_field));
    const char* field_name = "test.[u64]";
    extractReq->field = field_name;
    extractReq->field_id = 0;
    extractReq->flist = 1;
    extractReq->ftype = FTYPE_UINT64;

    ss_plugin_rc ret = _plugin.extract_fields(
        _plugin_state,
        evt,
        1,
        extractReq);

    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);
    ASSERT_STREQ(field_name, extractReq->field);
    ASSERT_EQ(value, extractReq->res_len);

    for (int i = 0; i < value; i++)
    {
        uint64_t elem = extractReq->res.u64[i];
        ASSERT_EQ(i, elem);
    }

    free((void*)evt->data);
    free(evt);
    free(extractReq);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginExtractFieldSinglesString)
{
    ss_plugin_event* evt = (ss_plugin_event*)malloc(sizeof(ss_plugin_event));
    uint64_t value = 42;
    const char* strvalue = "Counter = 42";
    evt->evtnum = 1;
    evt->ts = 0;
    evt->data = (const uint8_t*)malloc(sizeof(uint64_t));
    evt->datalen = sizeof(uint64_t);
    memcpy((void*)evt->data, &value, sizeof(uint64_t));

    auto extractReq = (ss_plugin_extract_field*)malloc(sizeof(ss_plugin_extract_field));
    const char* field_name = "test.str";
    extractReq->field = field_name;
    extractReq->field_id = 0;
    extractReq->flist = 0;
    extractReq->ftype = FTYPE_STRING;

    ss_plugin_rc ret = _plugin.extract_fields(
        _plugin_state,
        evt,
        1,
        extractReq);

    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);
    ASSERT_STREQ(field_name, extractReq->field);
    ASSERT_EQ(1, extractReq->res_len);
    ASSERT_STREQ(strvalue, *extractReq->res.str);

    free((void*)evt->data);
    free(evt);
    free(extractReq);
}

TEST_F(PluginFieldExtractionOnlyTest, PluginExtractFieldStringArray)
{
    ss_plugin_event* evt = (ss_plugin_event*)malloc(sizeof(ss_plugin_event));
    uint64_t value = 10;
    evt->evtnum = 1;
    evt->ts = 0;
    evt->data = (const uint8_t*)malloc(sizeof(uint64_t));
    evt->datalen = sizeof(uint64_t);
    memcpy((void*)evt->data, &value, sizeof(uint64_t));

    auto extractReq = (ss_plugin_extract_field*)malloc(sizeof(ss_plugin_extract_field));
    const char* field_name = "test.[str]";
    extractReq->field = field_name;
    extractReq->field_id = 0;
    extractReq->flist = 1;
    extractReq->ftype = FTYPE_STRING;

    ss_plugin_rc ret = _plugin.extract_fields(
        _plugin_state,
        evt,
        1,
        extractReq);

    ASSERT_EQ(SS_PLUGIN_SUCCESS, ret);
    ASSERT_STREQ(field_name, extractReq->field);
    ASSERT_EQ(value, extractReq->res_len);

    std::string counter = "Counter: ";
    std::string expected;

    for (int i = 0; i < value; i++)
    {
        const char* elem = extractReq->res.str[i];
        expected = counter + std::to_string(i);
        ASSERT_STREQ(expected.c_str(), elem);
    }

    free((void*)evt->data);
    free(evt);
    free(extractReq);
}

int main(int argc, char** argv) {
    testing::GTEST_FLAG(output) = "xml:./Testing/Artifacts/";
	testing::InitGoogleTest(&argc, argv);
    int result = RUN_ALL_TESTS();
}