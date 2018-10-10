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
            "Cg5ScGNQcm90by5wcm90bxIKTmlyZ2UuQ29yZRogZ29vZ2xlL3Byb3RvYnVm",
            "L2Rlc2NyaXB0b3IucHJvdG8iHwoQUnBjU2VydmljZU9wdGlvbhILCgNVaWQY",
            "ASABKAUiNQoUUnBjU2VydmljZUNhbGxPcHRpb24SCwoDVWlkGAEgASgFEhAK",
            "CElzT25lV2F5GAIgASgIIhIKEFJwY0NhbGxBcmdzRW1wdHkiSQoKUnBjQ2Fs",
            "bFJlcRIOCgZTZXJpYWwYASABKAQSDwoHU2VydmljZRgCIAEoBRIMCgRDYWxs",
            "GAMgASgFEgwKBEFyZ3MYBCABKAwiSAoKUnBjQ2FsbFJzcBIOCgZTZXJpYWwY",
            "ASABKAQSDwoHU2VydmljZRgCIAEoBRIMCgRDYWxsGAMgASgFEgsKA1JldBgE",
            "IAEoDCJXChNScGNDYWxsRXhjZXB0aW9uUnNwEg4KBlNlcmlhbBgBIAEoBBIP",
            "CgdTZXJ2aWNlGAIgASgFEgwKBENhbGwYAyABKAUSEQoJRXhjZXB0aW9uGAQg",
            "ASgFOlMKClJwY1NlcnZpY2USHy5nb29nbGUucHJvdG9idWYuU2VydmljZU9w",
            "dGlvbnMYxdUDIAEoCzIcLk5pcmdlLkNvcmUuUnBjU2VydmljZU9wdGlvbjpa",
            "Cg5ScGNTZXJ2aWNlQ2FsbBIeLmdvb2dsZS5wcm90b2J1Zi5NZXRob2RPcHRp",
            "b25zGKnWAyABKAsyIC5OaXJnZS5Db3JlLlJwY1NlcnZpY2VDYWxsT3B0aW9u",
            "Qg2qAgpOaXJnZS5Db3JlYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { pbr::FileDescriptor.DescriptorProtoFileDescriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcServiceOption), global::Nirge.Core.RpcServiceOption.Parser, new[]{ "Uid" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcServiceCallOption), global::Nirge.Core.RpcServiceCallOption.Parser, new[]{ "Uid", "IsOneWay" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcCallArgsEmpty), global::Nirge.Core.RpcCallArgsEmpty.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcCallReq), global::Nirge.Core.RpcCallReq.Parser, new[]{ "Serial", "Service", "Call", "Args" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcCallRsp), global::Nirge.Core.RpcCallRsp.Parser, new[]{ "Serial", "Service", "Call", "Ret" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.RpcCallExceptionRsp), global::Nirge.Core.RpcCallExceptionRsp.Parser, new[]{ "Serial", "Service", "Call", "Exception" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class RpcServiceOption : pb::IMessage<RpcServiceOption> {
    private static readonly pb::MessageParser<RpcServiceOption> _parser = new pb::MessageParser<RpcServiceOption>(() => new RpcServiceOption());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcServiceOption> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceOption() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceOption(RpcServiceOption other) : this() {
      uid_ = other.uid_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceOption Clone() {
      return new RpcServiceOption(this);
    }

    /// <summary>Field number for the "Uid" field.</summary>
    public const int UidFieldNumber = 1;
    private int uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RpcServiceOption);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcServiceOption other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Uid != other.Uid) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Uid != 0) hash ^= Uid.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Uid);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Uid);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RpcServiceOption other) {
      if (other == null) {
        return;
      }
      if (other.Uid != 0) {
        Uid = other.Uid;
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
            Uid = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class RpcServiceCallOption : pb::IMessage<RpcServiceCallOption> {
    private static readonly pb::MessageParser<RpcServiceCallOption> _parser = new pb::MessageParser<RpcServiceCallOption>(() => new RpcServiceCallOption());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcServiceCallOption> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceCallOption() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceCallOption(RpcServiceCallOption other) : this() {
      uid_ = other.uid_;
      isOneWay_ = other.isOneWay_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcServiceCallOption Clone() {
      return new RpcServiceCallOption(this);
    }

    /// <summary>Field number for the "Uid" field.</summary>
    public const int UidFieldNumber = 1;
    private int uid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Uid {
      get { return uid_; }
      set {
        uid_ = value;
      }
    }

    /// <summary>Field number for the "IsOneWay" field.</summary>
    public const int IsOneWayFieldNumber = 2;
    private bool isOneWay_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsOneWay {
      get { return isOneWay_; }
      set {
        isOneWay_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RpcServiceCallOption);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcServiceCallOption other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Uid != other.Uid) return false;
      if (IsOneWay != other.IsOneWay) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Uid != 0) hash ^= Uid.GetHashCode();
      if (IsOneWay != false) hash ^= IsOneWay.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Uid != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Uid);
      }
      if (IsOneWay != false) {
        output.WriteRawTag(16);
        output.WriteBool(IsOneWay);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Uid != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Uid);
      }
      if (IsOneWay != false) {
        size += 1 + 1;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RpcServiceCallOption other) {
      if (other == null) {
        return;
      }
      if (other.Uid != 0) {
        Uid = other.Uid;
      }
      if (other.IsOneWay != false) {
        IsOneWay = other.IsOneWay;
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
            Uid = input.ReadInt32();
            break;
          }
          case 16: {
            IsOneWay = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class RpcCallArgsEmpty : pb::IMessage<RpcCallArgsEmpty> {
    private static readonly pb::MessageParser<RpcCallArgsEmpty> _parser = new pb::MessageParser<RpcCallArgsEmpty>(() => new RpcCallArgsEmpty());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcCallArgsEmpty> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallArgsEmpty() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallArgsEmpty(RpcCallArgsEmpty other) : this() {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallArgsEmpty Clone() {
      return new RpcCallArgsEmpty(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RpcCallArgsEmpty);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcCallArgsEmpty other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RpcCallArgsEmpty other) {
      if (other == null) {
        return;
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
        }
      }
    }

  }

  public sealed partial class RpcCallReq : pb::IMessage<RpcCallReq> {
    private static readonly pb::MessageParser<RpcCallReq> _parser = new pb::MessageParser<RpcCallReq>(() => new RpcCallReq());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcCallReq> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallReq() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallReq(RpcCallReq other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
      args_ = other.args_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallReq Clone() {
      return new RpcCallReq(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private ulong serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Serial {
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
      return Equals(other as RpcCallReq);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcCallReq other) {
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
      if (Serial != 0UL) hash ^= Serial.GetHashCode();
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
      if (Serial != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Serial);
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
      if (Serial != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Serial);
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
    public void MergeFrom(RpcCallReq other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0UL) {
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
            Serial = input.ReadUInt64();
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

  public sealed partial class RpcCallRsp : pb::IMessage<RpcCallRsp> {
    private static readonly pb::MessageParser<RpcCallRsp> _parser = new pb::MessageParser<RpcCallRsp>(() => new RpcCallRsp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcCallRsp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[4]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallRsp() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallRsp(RpcCallRsp other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
      ret_ = other.ret_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallRsp Clone() {
      return new RpcCallRsp(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private ulong serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Serial {
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
      return Equals(other as RpcCallRsp);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcCallRsp other) {
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
      if (Serial != 0UL) hash ^= Serial.GetHashCode();
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
      if (Serial != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Serial);
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
      if (Serial != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Serial);
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
    public void MergeFrom(RpcCallRsp other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0UL) {
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
            Serial = input.ReadUInt64();
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

  public sealed partial class RpcCallExceptionRsp : pb::IMessage<RpcCallExceptionRsp> {
    private static readonly pb::MessageParser<RpcCallExceptionRsp> _parser = new pb::MessageParser<RpcCallExceptionRsp>(() => new RpcCallExceptionRsp());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RpcCallExceptionRsp> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.RpcProtoReflection.Descriptor.MessageTypes[5]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallExceptionRsp() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallExceptionRsp(RpcCallExceptionRsp other) : this() {
      serial_ = other.serial_;
      service_ = other.service_;
      call_ = other.call_;
      exception_ = other.exception_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RpcCallExceptionRsp Clone() {
      return new RpcCallExceptionRsp(this);
    }

    /// <summary>Field number for the "Serial" field.</summary>
    public const int SerialFieldNumber = 1;
    private ulong serial_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Serial {
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

    /// <summary>Field number for the "Exception" field.</summary>
    public const int ExceptionFieldNumber = 4;
    private int exception_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Exception {
      get { return exception_; }
      set {
        exception_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RpcCallExceptionRsp);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RpcCallExceptionRsp other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Serial != other.Serial) return false;
      if (Service != other.Service) return false;
      if (Call != other.Call) return false;
      if (Exception != other.Exception) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Serial != 0UL) hash ^= Serial.GetHashCode();
      if (Service != 0) hash ^= Service.GetHashCode();
      if (Call != 0) hash ^= Call.GetHashCode();
      if (Exception != 0) hash ^= Exception.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Serial != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Serial);
      }
      if (Service != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Service);
      }
      if (Call != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Call);
      }
      if (Exception != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(Exception);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Serial != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Serial);
      }
      if (Service != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Service);
      }
      if (Call != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Call);
      }
      if (Exception != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Exception);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RpcCallExceptionRsp other) {
      if (other == null) {
        return;
      }
      if (other.Serial != 0UL) {
        Serial = other.Serial;
      }
      if (other.Service != 0) {
        Service = other.Service;
      }
      if (other.Call != 0) {
        Call = other.Call;
      }
      if (other.Exception != 0) {
        Exception = other.Exception;
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
            Serial = input.ReadUInt64();
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
          case 32: {
            Exception = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
