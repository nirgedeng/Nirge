#pragma once

#include <string>
#include <vector>
#include <map>

#include <google/protobuf/descriptor.h>
#include <google/protobuf/compiler/csharp/csharp_options.h>

namespace google
{
    namespace protobuf
    {
        namespace compiler
        {
            namespace csharp
            {
                class CServiceGenerator
                {
                    class CMethod
                    {
                    public:
                        CMethod(const MethodDescriptor* descriptor, const csharp::Options& options, std::string* error);
                        ~CMethod();

                        bool Init();
                        void Destroy();

                    public:
                        const MethodDescriptor* Descriptor()
                        {
                            return _descriptor;
                        }

                        int Uid() const
                        {
                            return _uid;
                        }

                        bool IsOneWay() const
                        {
                            return _isOneWay;
                        }

                        std::string CallName() const
                        {
                            return _callName;
                        }

                        bool ContainsArgs() const
                        {
                            return _containsArgs;
                        }

                    private:
                        const MethodDescriptor* _descriptor;
                        const csharp::Options& _options;
                        std::string* _error;

                        int _uid;
                        bool _isOneWay;
                        std::string _callName;
                        bool _containsArgs;

                    };

                    const char* gCls = "C";
                    const char* gInterface = "I";
                    const char* gvoid = "void";
                    const char* gargs = "args";
                    const char* gRpcService = "RpcService";
                    const char* gRpcCaller = "RpcCaller";
                    const char* gRpcCallee = "RpcCallee";
                    const char* gArgsEmpty = "ArgsEmpty";

                public:
                    CServiceGenerator(const ServiceDescriptor* descriptor, const csharp::Options& options, std::string* error);
                    ~CServiceGenerator();

                    bool Init();
                    void Destroy();

                public:
                    std::string InterfaceName() const
                    {
                        return _interfaceName;
                    }

                    std::string InterfaceNameNoPrefix() const
                    {
                        return _interfaceNameNoPrefix;
                    }

                    std::string CallerNameNoPrefix() const
                    {
                        return _callerNameNoPrefix;
                    }

                    std::string CalleeNameNoPrefix() const
                    {
                        return _calleeNameNoPrefix;
                    }

                    void Generate(io::Printer* printer);
                    void GenerateInterface(io::Printer* printer);
                    void GenerateCaller(io::Printer* printer);
                    void GenerateCallee(io::Printer* printer);

                private:
                    const ServiceDescriptor* _descriptor;
                    const csharp::Options& _options;
                    std::string* _error;

                    int _uid;
                    std::string _interfaceName;
                    std::string _interfaceNameNoPrefix;
                    std::string _serviceName;
                    std::string _callerName;
                    std::string _callerNameNoPrefix;
                    std::string _calleeName;
                    std::string _calleeNameNoPrefix;
                    std::string _descriptor_accessor;
                    std::vector<CMethod*> _methods;

                };
            }
        }
    }
}
