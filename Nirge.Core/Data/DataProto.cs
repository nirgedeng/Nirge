// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: DataProto.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nirge.Core {

  /// <summary>Holder for reflection information generated from DataProto.proto</summary>
  public static partial class DataProtoReflection {

    #region Descriptor
    /// <summary>File descriptor for DataProto.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DataProtoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9EYXRhUHJvdG8ucHJvdG8SCk5pcmdlLkNvcmUaIGdvb2dsZS9wcm90b2J1",
            "Zi9kZXNjcmlwdG9yLnByb3RvIisKDERhdGFJZE9wdGlvbhINCgVPcmRlchgB",
            "IAEoBRIMCgRCaXRzGAIgASgFOkkKBkRhdGFJZBIdLmdvb2dsZS5wcm90b2J1",
            "Zi5GaWVsZE9wdGlvbnMYxdUDIAEoCzIYLk5pcmdlLkNvcmUuRGF0YUlkT3B0",
            "aW9uQg2qAgpOaXJnZS5Db3JlYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { pbr::FileDescriptor.DescriptorProtoFileDescriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.DataIdOption), global::Nirge.Core.DataIdOption.Parser, new[]{ "Order", "Bits" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class DataIdOption : pb::IMessage<DataIdOption> {
    private static readonly pb::MessageParser<DataIdOption> _parser = new pb::MessageParser<DataIdOption>(() => new DataIdOption());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<DataIdOption> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.DataProtoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public DataIdOption() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public DataIdOption(DataIdOption other) : this() {
      order_ = other.order_;
      bits_ = other.bits_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public DataIdOption Clone() {
      return new DataIdOption(this);
    }

    /// <summary>Field number for the "Order" field.</summary>
    public const int OrderFieldNumber = 1;
    private int order_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Order {
      get { return order_; }
      set {
        order_ = value;
      }
    }

    /// <summary>Field number for the "Bits" field.</summary>
    public const int BitsFieldNumber = 2;
    private int bits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Bits {
      get { return bits_; }
      set {
        bits_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as DataIdOption);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(DataIdOption other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Order != other.Order) return false;
      if (Bits != other.Bits) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Order != 0) hash ^= Order.GetHashCode();
      if (Bits != 0) hash ^= Bits.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Order != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Order);
      }
      if (Bits != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Bits);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Order != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Order);
      }
      if (Bits != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Bits);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(DataIdOption other) {
      if (other == null) {
        return;
      }
      if (other.Order != 0) {
        Order = other.Order;
      }
      if (other.Bits != 0) {
        Bits = other.Bits;
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
            Order = input.ReadInt32();
            break;
          }
          case 16: {
            Bits = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
