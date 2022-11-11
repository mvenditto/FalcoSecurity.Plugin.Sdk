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

const char* PLUGIN_LIB_PATH = 
	"../../FalcoSecurity.Plugin.Sdk.TestPlugin/bin/Release/net6.0/linux-x64/plugin_native.so";
	
class PluginIntegrationTest : public testing::Test 
{
	protected:
		plugin_handle_t* _plugin_handle;
		plugin_api _plugin;
	public:
		void SetUp() override 
		{
			_plugin_handle = plugin_load(PLUGIN_LIB_PATH);
			_plugin = _plugin_handle->api;
		}
		
		void TearDown() override
		{
			//_plugin.destroy(nullptr);
			plugin_unload(_plugin_handle);
		}
};

#define ASSERT_SYM(a, s) \
    do { \
        ASSERT_NE(a.s, nullptr); \
    } while(0)

TEST_F(PluginIntegrationTest, PluginLoad) 
{
	ASSERT_NE(_plugin_handle, nullptr);
}

TEST_F(PluginIntegrationTest, PluginHasRequiredSymbols)
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

TEST_F(PluginIntegrationTest, PluginGetName)
{
	ASSERT_STREQ(_plugin.get_name(), "test_plugin");
}

TEST_F(PluginIntegrationTest, PluginGetVersion)
{
	ASSERT_STREQ(_plugin.get_version(), "1.2.3");
}

int main(int argc, char** argv) {
    testing::GTEST_FLAG(output) = "xml:./Testing/Artifacts/";
	testing::InitGoogleTest(&argc, argv);
    int result = RUN_ALL_TESTS();
}