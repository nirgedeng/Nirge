#include <iostream>

#include <google/protobuf/compiler/plugin.h>

#include "csharp_generator.h"

int main(int argc, char* argv[])
{
    return ::google::protobuf::compiler::PluginMain(argc
            , argv
            , &::google::protobuf::compiler::csharp::CGenerator());
}
