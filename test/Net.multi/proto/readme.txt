--------------------------------------
--------------------------------------
--------------------------------------
--------------------------------------
--------------------------------------
--------------------------------------
--------------------------------------

protoc.exe --proto_path=E:\Nirge\trunk\test\Net.multi\proto --csharp_out=E:\Nirge\trunk\test\Net.multi\proto E:\Nirge\trunk\test\Net.multi\proto\service.proto
protoc.exe -I ./ --csharp_out ./  ./service.proto --grpc_out ./ --plugin=protoc-gen-grpc=./rpc_csharp.exe

--------------------------------------

