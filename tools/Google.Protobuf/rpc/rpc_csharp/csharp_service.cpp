#include "csharp_service.h"
#include "csharp_helpers.h"
#include "Proto.pb.h"
#include "RpcProto.pb.h"

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
#include <google/protobuf/extension_set.h>

#include <google/protobuf/compiler/csharp/csharp_generator.h>
#include <google/protobuf/compiler/csharp/csharp_helpers.h>
#include <google/protobuf/compiler/csharp/csharp_names.h>
#include <google/protobuf/compiler/csharp/csharp_options.h>
#include <google/protobuf/compiler/csharp/csharp_reflection_class.h>

using google::protobuf::internal::scoped_ptr;

#include <boost/lexical_cast.hpp>
#include <boost/format.hpp>

//------------------------------------------------------------------

namespace google
{
    namespace protobuf
    {
        namespace compiler
        {
            namespace csharp
            {
                //------------------------------------------------------------------

                CServiceGenerator::CMethod::CMethod(const MethodDescriptor* descriptor, const Options& options, std::string* error)
                    : _descriptor(descriptor)
                    , _options(options)
                    , _error(error)
                {
                }
                
                CServiceGenerator::CMethod::~CMethod()
                {
                }

                bool CServiceGenerator::CMethod::Init()
                {
                    google::protobuf::internal::ExtensionIdentifier<::google::protobuf::MethodOptions
                    , google::protobuf::internal::MessageTypeTraits<Nirge::Core::RpcServiceCallOption>
                    , 11
                    , false>
                    args(60002, Nirge::Core::RpcServiceCallOption());
                    auto opt = _descriptor->options().GetExtension(args);
                    _uid = opt.uid();
                    _isOneWay = opt.isoneway();

                    _callName = _descriptor->name();
                    _containsArgs = _descriptor->input_type()->full_name() != Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name();

                    return true;
                }

                void CServiceGenerator::CMethod::Destroy()
                {
                }

                //------------------------------------------------------------------

                CServiceGenerator::CServiceGenerator(const ServiceDescriptor* descriptor, const Options& options, std::string* error)
                    : _descriptor(descriptor)
                    , _options(options)
                    , _error(error)
                {
                }

                CServiceGenerator::~CServiceGenerator()
                {
                }

                //------------------------------------------------------------------

                bool CServiceGenerator::Init()
                {
                    google::protobuf::internal::ExtensionIdentifier<::google::protobuf::ServiceOptions
                    , google::protobuf::internal::MessageTypeTraits<Nirge::Core::RpcServiceOption>
                    , 11
                    , false>
                    args(60001, Nirge::Core::RpcServiceOption());
                    auto opt = _descriptor->options().GetExtension(args);
                    _uid = opt.uid();

                    _interfaceName = gInterface + _descriptor->name() + gRpcService;
                    _callerName = gCls + _descriptor->name() + gRpcCaller;
                    _calleeName = gCls + _descriptor->name() + gRpcCallee;

                    for (int i = 0; i < _descriptor->method_count(); ++i)
                    {
                        CMethod* e = new CMethod(_descriptor->method(i), _options, _error);
                        _methods.push_back(e);
                        e->Init();
                    }

                    return true;
                }

                void CServiceGenerator::Destroy()
                {
for (auto& i : _methods)
                    {
                        i->Destroy();
                        delete i;
                    }
                }

                //------------------------------------------------------------------

                void CServiceGenerator::Generate(io::Printer* printer)
                {
                    GenerateInterface(printer);
                    GenerateCaller(printer);
                    GenerateCallee(printer);
                }

                void CServiceGenerator::GenerateInterface(io::Printer* printer)
                {
                    printer->Print("public interface $interfaceName$ : IRpcService {\n", "interfaceName", _interfaceName);
                    printer->Indent();
for (const auto& i : _methods)
                    {
                        printer->Print("$ret$ $call$(int channel"
                                       , "ret", i->IsOneWay() ? "void" : (i->Descriptor()->output_type()->full_name() == Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name() ? "void" : i->Descriptor()->output_type()->full_name())
                                       , "call", i->CallName());
                        if (i->ContainsArgs())
                            printer->Print(", $args$ args", "args", i->Descriptor()->input_type()->full_name());
                        printer->Print(");\n");

                    }
                    printer->Outdent();
                    printer->Print("}\n");
                }

                void CServiceGenerator::GenerateCaller(io::Printer* printer)
                {
                    printer->Print("public class $callerName$ : CRpcCaller {\n", "callerName", _callerName);
                    printer->Indent();
                    printer->Print("public $callerName$(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs) : base(args, log, stream, communicator, stubs, null, $uid$) {}\n"
                                   , "callerName", _callerName
                                   , "uid", boost::lexical_cast<std::string>(_uid));
for (const auto& i : _methods)
                    {
                        printer->Print("public $ret$ $call$("
                                       , "ret", i->IsOneWay() ? "void" : boost::str(boost::format("Task<%1%>") % i->Descriptor()->output_type()->full_name())
                                       , "call", i->CallName());
                        if (i->ContainsArgs())
                            printer->Print("$args$ args, ", "args", i->Descriptor()->input_type()->full_name());
                        printer->Print("int channel = 0){\n");
                        printer->Indent();
                        if (i->IsOneWay())
                        {
                            printer->Print("Call<$argstype$>(channel, $uid$, $args$);\n"
                                           , "argstype", i->Descriptor()->input_type()->full_name()
                                           , "uid", boost::lexical_cast<std::string>(i->Uid())
                                           , "args", i->Descriptor()->input_type()->full_name() == Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name() ? gArgsEmpty : "args");
                        }
                        else
                        {
                            printer->Print("return CallAsync<$argstype$, $rettype$>(channel, $uid$, $args$);\n"
                                           , "argstype", i->Descriptor()->input_type()->full_name()
                                           , "rettype", i->Descriptor()->output_type()->full_name()
                                           , "uid", boost::lexical_cast<std::string>(i->Uid())
                                           , "args", i->Descriptor()->input_type()->full_name() == Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name() ? gArgsEmpty : "args");
                        }
                        printer->Outdent();
                        printer->Print("}\n");
                    }
                    printer->Outdent();
                    printer->Print("}\n");
                }

                void CServiceGenerator::GenerateCallee(io::Printer* printer)
                {
                    printer->Print("public class $calleeName$ : CRpcCallee<$interfaceName$> {\n"
                                   , "calleeName", _calleeName
                                   , "interfaceName", _interfaceName);
                    printer->Indent();
                    printer->Print("public $calleeName$(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, $interfaceName$ service) : base(args, log, stream, communicator, null, service) {}\n"
                                   , "calleeName", _calleeName
                                   , "interfaceName", _interfaceName);
                    printer->Print("public override void Call(int channel, $req$ req) {\n", "req", Nirge::Core::RpcCallReq::descriptor()->full_name());
                    printer->Indent();
                    printer->Print("switch (req.Call) {\n");

for (const auto& i : _methods)
                    {
                        printer->Print("case $uid$:\n", "uid", boost::lexical_cast<std::string>(i->Uid()));
                        printer->Indent();
                        printer->Print("$call$<$argstype$, $rettype$>(channel, req, (_, args) => {\n"
                                       , "call", i->IsOneWay() ? "Call" : "CallAsync"
                                       , "argstype", i->Descriptor()->input_type()->full_name()
                                       , "rettype", i->Descriptor()->output_type()->full_name());
                        printer->Indent();
                        if (i->Descriptor()->output_type()->full_name() != Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name())
                            printer->Print("return ");
                        printer->Print("_service.$call$(channel", "call", i->CallName());
                        if (i->ContainsArgs())
                            printer->Print(", args");
                        printer->Print(");\n");
                        if (i->Descriptor()->output_type()->full_name() == Nirge::Core::RpcCallArgsEmpty::descriptor()->full_name())
                            printer->Print("return ArgsEmpty;\n");
                        printer->Outdent();
                        printer->Print("});\n");
                        printer->Print("break;\n");
                        printer->Outdent();
                    }

                    printer->Print("default:\n");
                    printer->Indent();
                    printer->Print("base.Call(channel, req);\n");
                    printer->Print("break;\n");
                    printer->Outdent();

                    printer->Print("}\n");
                    printer->Outdent();
                    printer->Print("}\n");
                    printer->Outdent();
                    printer->Print("}\n");
                }

                //------------------------------------------------------------------
            }
        }
    }
}

//------------------------------------------------------------------