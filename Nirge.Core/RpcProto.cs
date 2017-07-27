// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: RpcProto.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nirge.Core {

  /// <summary>Holder for reflection information generated from RpcProto.proto</summary>
  public static partial class RpcProtoReflection {

    #region Descriptor
    /// <summary>File descriptor for RpcProto.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static RpcProtoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5ScGNQcm90by5wcm90bxIPZ29vZ2xlLnByb3RvYnVmGgtQcm90by5wcm90",
            "byJJCgpDMnNScGNDYWxsEg4KBlNlcmlhbBgBIAEoBRIPCgdTZXJ2aWNlGAIg",
            "ASgFEgwKBENhbGwYAyABKAUSDAoEQXJncxgEIAEoDCJICgpTMmNScGNDYWxs",
            "Eg4KBlNlcmlhbBgBIAEoBRIPCgdTZXJ2aWNlGAIgASgFEgwKBENhbGwYAyAB",
            "KAUSCwoDUmV0GAQgASgMIj4KDVMyY1JwY0NhbGxFcnISDgoGU2VyaWFsGAEg",
            "ASgFEg8KB1NlcnZpY2UYAiABKAUSDAoEQ2FsbBgDIAEoBUINqgIKTmlyZ2Uu",
            "Q29yZWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Nirge.Core.ProtoReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.C2sRpcCall), global::Nirge.Core.C2sRpcCall.Parser, new[]{ "Serial", "Service", "Call", "Args" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.S2cRpcCall), global::Nirge.Core.S2cRpcCall.Parser, new[]{ "Serial", "Service", "Call", "Ret" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.S2cRpcCallErr), global::Nirge.Core.S2cRpcCallErr.Parser, new[]{ "Serial", "Service", "Call" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class C2sRpcCall : pb::IMessage<C2sRpcCall> {
    private static readonly pb::MessageParser<C2sRpcCall> _parser = new pb::MessageParser<C2sRpcCall>(() => new C2sRpcCall());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<C2sRpcCall> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2sRpcCall() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2sRpcCall(C2sRpcCall other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
      args_ = other.args_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public C2sRpcCall Clone() {
      return new C2sRpcCall(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private int serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Serial {
      get { return serial_; }
      set {
        serial_ = value;
      }
    }

    /// <summary>Field number for the "Service" field.</summary>
    public const int ServiceFieldNumber = 2;
    private int service_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Service {
      get { return service_; }
      set {
        service_ = value;
      }
    }

    /// <summary>Field number for the "Call" field.</summary>
    public const int CallFieldNumber = 3;
    private int call_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Call {
      get { return call_; }
      set {
        call_ = value;
      }
    }

    /// <summary>Field number for the "Args" field.</summary>
    public const int ArgsFieldNumber = 4;
    private pb::ByteString args_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Args {
      get { return args_; }
      set {
        args_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as C2sRpcCall);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(C2sRpcCall other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Serial != other.Serial) return false;
      if (Service != other.Service) return false;
      if (Call != other.Call) return false;
      if (Args != other.Args) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Serial != 0) hash ^= Serial.GetHashCode();
      if (Service != 0) hash ^= Service.GetHashCode();
      if (Call != 0) hash ^= Call.GetHashCode();
      if (Args.Length != 0) hash ^= Args.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Serial != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Serial);
      }
      if (Service != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Service);
      }
      if (Call != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Call);
      }
      if (Args.Length != 0) {
        output.WriteRawTag(34);
        output.WriteBytes(Args);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Serial != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Serial);
      }
      if (Service != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Service);
      }
      if (Call != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Call);
      }
      if (Args.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Args);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(C2sRpcCall other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0) {
        Serial = other.Serial;
      }
      if (other.Service != 0) {
        Service = other.Service;
      }
      if (other.Call != 0) {
        Call = other.Call;
      }
      if (other.Args.Length != 0) {
        Args = other.Args;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Serial = input.ReadInt32();
            break;
          }
          case 16: {
            Service = input.ReadInt32();
            break;
          }
          case 24: {
            Call = input.ReadInt32();
            break;
          }
          case 34: {
            Args = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  public sealed partial class S2cRpcCall : pb::IMessage<S2cRpcCall> {
    private static readonly pb::MessageParser<S2cRpcCall> _parser = new pb::MessageParser<S2cRpcCall>(() => new S2cRpcCall());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2cRpcCall> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCall() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCall(S2cRpcCall other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
      ret_ = other.ret_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCall Clone() {
      return new S2cRpcCall(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private int serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Serial {
      get { return serial_; }
      set {
        serial_ = value;
      }
    }

    /// <summary>Field number for the "Service" field.</summary>
    public const int ServiceFieldNumber = 2;
    private int service_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Service {
      get { return service_; }
      set {
        service_ = value;
      }
    }

    /// <summary>Field number for the "Call" field.</summary>
    public const int CallFieldNumber = 3;
    private int call_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Call {
      get { return call_; }
      set {
        call_ = value;
      }
    }

    /// <summary>Field number for the "Ret" field.</summary>
    public const int RetFieldNumber = 4;
    private pb::ByteString ret_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Ret {
      get { return ret_; }
      set {
        ret_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2cRpcCall);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2cRpcCall other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Serial != other.Serial) return false;
      if (Service != other.Service) return false;
      if (Call != other.Call) return false;
      if (Ret != other.Ret) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Serial != 0) hash ^= Serial.GetHashCode();
      if (Service != 0) hash ^= Service.GetHashCode();
      if (Call != 0) hash ^= Call.GetHashCode();
      if (Ret.Length != 0) hash ^= Ret.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Serial != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Serial);
      }
      if (Service != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Service);
      }
      if (Call != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Call);
      }
      if (Ret.Length != 0) {
        output.WriteRawTag(34);
        output.WriteBytes(Ret);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Serial != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Serial);
      }
      if (Service != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Service);
      }
      if (Call != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Call);
      }
      if (Ret.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Ret);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2cRpcCall other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0) {
        Serial = other.Serial;
      }
      if (other.Service != 0) {
        Service = other.Service;
      }
      if (other.Call != 0) {
        Call = other.Call;
      }
      if (other.Ret.Length != 0) {
        Ret = other.Ret;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Serial = input.ReadInt32();
            break;
          }
          case 16: {
            Service = input.ReadInt32();
            break;
          }
          case 24: {
            Call = input.ReadInt32();
            break;
          }
          case 34: {
            Ret = input.ReadBytes();
            break;
          }
        }
      }
    }

  }

  public sealed partial class S2cRpcCallErr : pb::IMessage<S2cRpcCallErr> {
    private static readonly pb::MessageParser<S2cRpcCallErr> _parser = new pb::MessageParser<S2cRpcCallErr>(() => new S2cRpcCallErr());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<S2cRpcCallErr> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCallErr() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCallErr(S2cRpcCallErr other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public S2cRpcCallErr Clone() {
      return new S2cRpcCallErr(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private int serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Serial {
      get { return serial_; }
      set {
        serial_ = value;
      }
    }

    /// <summary>Field number for the "Service" field.</summary>
    public const int ServiceFieldNumber = 2;
    private int service_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Service {
      get { return service_; }
      set {
        service_ = value;
      }
    }

    /// <summary>Field number for the "Call" field.</summary>
    public const int CallFieldNumber = 3;
    private int call_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Call {
      get { return call_; }
      set {
        call_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as S2cRpcCallErr);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(S2cRpcCallErr other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Serial != other.Serial) return false;
      if (Service != other.Service) return false;
      if (Call != other.Call) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Serial != 0) hash ^= Serial.GetHashCode();
      if (Service != 0) hash ^= Service.GetHashCode();
      if (Call != 0) hash ^= Call.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Serial != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Serial);
      }
      if (Service != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Service);
      }
      if (Call != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Call);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Serial != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Serial);
      }
      if (Service != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Service);
      }
      if (Call != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Call);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(S2cRpcCallErr other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0) {
        Serial = other.Serial;
      }
      if (other.Service != 0) {
        Service = other.Service;
      }
      if (other.Call != 0) {
        Call = other.Call;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            Serial = input.ReadInt32();
            break;
          }
          case 16: {
            Service = input.ReadInt32();
            break;
          }
          case 24: {
            Call = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
