#pragma once

#include <string>

#include <google/protobuf/compiler/code_generator.h>

namespace google
{
    namespace protobuf
    {
        namespace compiler
        {
            namespace csharp
            {
                class CServiceGenerator;

                class CGenerator : public google::protobuf::compiler::CodeGenerator
                {
                public:
                    virtual bool Generate(const FileDescriptor* file, const std::string& parameter, GeneratorContext* generator_context, std::string* error) const;

                };
            }
        }
    }
}
