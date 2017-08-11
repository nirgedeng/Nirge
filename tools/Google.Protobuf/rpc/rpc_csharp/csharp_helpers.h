#pragma once

#include <google/protobuf/compiler/code_generator.h>
#include <google/protobuf/compiler/plugin.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/descriptor.pb.h>
#include <google/protobuf/io/printer.h>
#include <google/protobuf/io/zero_copy_stream.h>
#include <google/protobuf/stubs/strutil.h>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/io/printer.h>
#include <google/protobuf/io/zero_copy_stream_impl_lite.h>

#include <google/protobuf/compiler/csharp/csharp_generator.h>
#include <google/protobuf/compiler/csharp/csharp_helpers.h>
#include <google/protobuf/compiler/csharp/csharp_names.h>
#include <google/protobuf/compiler/csharp/csharp_options.h>
#include <google/protobuf/compiler/csharp/csharp_reflection_class.h>

namespace google
{
    namespace protobuf
    {
        namespace compiler
        {
            namespace csharp
            {
                inline std::vector<std::string> tokenize(const std::string& input,
                        const std::string& delimiters)
                {
                    std::vector<std::string> tokens;
                    size_t pos, last_pos = 0;

                    for (;;)
                    {
                        bool done = false;
                        pos = input.find_first_of(delimiters, last_pos);
                        if (pos == std::string::npos)
                        {
                            done = true;
                            pos = input.length();
                        }

                        tokens.push_back(input.substr(last_pos, pos - last_pos));
                        if (done) return tokens;

                        last_pos = pos + 1;
                    }
                }

                inline bool StripPrefix(std::string* name, const std::string& prefix)
                {
                    if (name->length() >= prefix.length())
                    {
                        if (name->substr(0, prefix.size()) == prefix)
                        {
                            *name = name->substr(prefix.size());
                            return true;
                        }
                    }
                    return false;
                }

                inline bool StripSuffix(std::string* filename, const std::string& suffix)
                {
                    if (filename->length() >= suffix.length())
                    {
                        size_t suffix_pos = filename->length() - suffix.length();
                        if (filename->compare(suffix_pos, std::string::npos, suffix) == 0)
                        {
                            filename->resize(filename->size() - suffix.size());
                            return true;
                        }
                    }

                    return false;
                }

                inline std::string StripProto(std::string filename)
                {
                    if (!StripSuffix(&filename, ".protodevel"))
                    {
                        StripSuffix(&filename, ".proto");
                    }
                    return filename;
                }

                inline std::string CapitalizeFirstLetter(std::string s)
                {
                    if (s.empty())
                    {
                        return s;
                    }
                    s[0] = ::toupper(s[0]);
                    return s;
                }

                inline std::string LowerUnderscoreToUpperCamel(std::string str)
                {
                    std::vector<std::string> tokens = tokenize(str, "_");
                    std::string result = "";
                    for (unsigned int i = 0; i < tokens.size(); i++)
                    {
                        result += CapitalizeFirstLetter(tokens[i]);
                    }
                    return result;
                }

                inline std::string FileNameInUpperCamel(
                    const google::protobuf::FileDescriptor* file, bool include_package_path)
                {
                    std::vector<std::string> tokens = tokenize(StripProto(file->name()), "/");
                    std::string result = "";
                    if (include_package_path)
                    {
                        for (unsigned int i = 0; i < tokens.size() - 1; i++)
                        {
                            result += tokens[i] + "/";
                        }
                    }
                    result += LowerUnderscoreToUpperCamel(tokens.back());
                    return result;
                }

                inline bool ServicesFilename(const google::protobuf::FileDescriptor* file,
                                             std::string* file_name_or_error)
                {
                    *file_name_or_error =
                        FileNameInUpperCamel(file, false) + ".rpc.cs";
                    return true;
                }
            }
        }
    }
}
