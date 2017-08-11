#include "csharp_generator.h"
#include "csharp_helpers.h"
#include "csharp_service.h"

#include <string>
#include <sstream>
#include <vector>

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

using google::protobuf::internal::scoped_ptr;

//------------------------------------------------------------------

namespace google
{
    namespace protobuf
    {
        namespace compiler
        {
            namespace csharp
            {
                bool CGenerator::Generate(
                    const FileDescriptor* file,
                    const string& parameter,
                    GeneratorContext* context,
                    string* error) const
                {
                    if (file->syntax() != FileDescriptor::SYNTAX_PROTO3 && !IsDescriptorProto(file))
                    {
                        *error = "C# code generation only supports proto3 syntax";
                        return false;
                    }

                    std::vector<std::pair<string, string> > options;
                    Options opts;
                    ParseGeneratorParameter(parameter, &options);

                    for (int i = 0; i < options.size(); i++)
                    {
                        if (options[i].first == "")
                        {
                        }
                        else if (options[i].first == "")
                        {
                        }
                        else
                        {
                            *error = "Unknown generator option: " + options[i].first;
                            return false;
                        }
                    }

                    if (file->service_count() == 0)
                        return true;

                    std::vector<CServiceGenerator*> services;

                    for (int i = 0; i < file->service_count(); ++i)
                    {
                        CServiceGenerator* e = new CServiceGenerator(file->service(i), opts, error);
                        services.push_back(e);
                        e->Init();
                    }

                    std::string s;
                    {
                        io::StringOutputStream stream(&s);
                        io::Printer printer(&stream, '$');

                        printer.Print
                        (
                            "// Generated by the protocol buffer compiler.  DO NOT EDIT!\n"
                            "// source: $file_name$\n"
                            "#pragma warning disable 1591, 0612, 3021\n"
                            "\n"
                            "#region Designer generated code\n"
                            "\n"
                            "using pbc = global::Google.Protobuf.Collections;\n"
                            "using pbr = global::Google.Protobuf.Reflection;\n"
                            "using pb = global::Google.Protobuf;\n"
                            "using System.Collections.Generic;\n"
                            "using System.Threading.Tasks;\n"
                            "using Nirge.Core;\n"
                            "using log4net;\n"
                            "using System;\n"
                            "\n",
                            "file_name", file->name()
                        );

                        printer.Print("namespace $namespace$ {\n", "namespace", GetFileNamespace(file));
                        printer.Indent();

for (auto& i : services) i->Generate(&printer);
for (auto& i : services)
                        {
                            i->Destroy();
                            delete i;
                        }

                        printer.Outdent();
                        printer.Print("}\n");
                        printer.Print("\n");
                        printer.Print("#endregion\n");
                    }

                    std::string csfile;
                    if (!ServicesFilename(file, &csfile))
                    {
                        *error = "ServicesFilename error";
                        return false;
                    }

                    std::unique_ptr<google::protobuf::io::ZeroCopyOutputStream> stream(context->Open(csfile));
                    google::protobuf::io::CodedOutputStream code(stream.get());
                    code.WriteRaw(s.data(), s.size());

                    return true;
                }
            }
        }
    }
}

//------------------------------------------------------------------
