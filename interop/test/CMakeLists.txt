cmake_minimum_required(VERSION 3.14)
project(plugintest)

# GoogleTest requires at least C++14
set(CMAKE_CXX_STANDARD 14)

include(FetchContent)
FetchContent_Declare(
  googletest
  URL https://github.com/google/googletest/archive/release-1.12.1.zip
)

# For Windows: Prevent overriding the parent project's compiler/linker settings
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

enable_testing()

add_executable(
  integration_test
  integration_test.cc
)

target_link_libraries(
  integration_test
  GTest::gtest_main
  ${CMAKE_DL_LIBS} # -ldl
)

add_test(
	integration_test
	integration_test
)

include(GoogleTest)
include_directories(../../Falco.Plugin.Sdk/)

gtest_discover_tests(integration_test)

