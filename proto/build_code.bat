






for %%c in (*.proto) do ..\tools\protobuf\protoc %%c   --csharp_out=..\kerryhe.Test\Proto


pause
