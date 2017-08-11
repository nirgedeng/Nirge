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
                        CMethod(const MethodDescriptor* descriptor, const Options& options, std::string* error);
                        ~CMethod();

                    public:
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
                        const Options& _options;
                        std::string* _error;

                        int _uid;
                        bool _isOneWay;
                        std::string _callName;
                        bool _containsArgs;

                    };

                    const char* gCls = "C";
                    const char* gInterface = "I";
                    const char* gRpcService = "RpcService";
                    const char* gRpcCaller = "RpcCaller";
                    const char* gRpcCallee = "RpcCallee";
                    const char* gArgsEmpty = "ArgsEmpty";

                public:
                    CServiceGenerator(const ServiceDescriptor* descriptor, const Options& options, std::string* error);
                    ~CServiceGenerator();

                public:
                    bool Init();
                    void Destroy();

                public:
                    void Generate(io::Printer* printer);
                private:
                    void GenerateInterface(io::Printer* printer);
                    void GenerateCaller(io::Printer* printer);
                    void GenerateCallee(io::Printer* printer);

                private:
                    const ServiceDescriptor* _descriptor;
                    const Options& _options;
                    std::string* _error;

                    int _uid;
                    std::string _interfaceName;
                    std::string _callerName;
                    std::string _calleeName;
                    std::vector<CMethod*> _methods;

                };
            }
        }
    }
}
