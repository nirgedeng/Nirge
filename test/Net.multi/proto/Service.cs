// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: service.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nirge.Core {

  /// <summary>Holder for reflection information generated from service.proto</summary>
  public static partial class ServiceReflection {

    #region Descriptor
    /// <summary>File descriptor for service.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ServiceReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1zZXJ2aWNlLnByb3RvEgpOaXJnZS5Db3JlGg5ScGNQcm90by5wcm90byIz",
            "CgVnYXJncxIJCgFhGAEgASgFEgkKAWIYAiABKAUSCQoBYxgDIAEoBRIJCgFk",
            "GAQgAygFIjMKBXBhcmdzEgkKAWEYASABKAUSCQoBYhgCIAEoBRIJCgFjGAMg",
            "ASgFEgkKAWQYBCADKAUiMwoFcWFyZ3MSCQoBYRgBIAEoBRIJCgFiGAIgASgF",
            "EgkKAWMYAyABKAUSCQoBZBgEIAMoBSIyCgRxcmV0EgkKAWEYASABKAUSCQoB",
            "YhgCIAEoBRIJCgFjGAMgASgFEgkKAWQYBCADKAUy7AIKBEdhbWUSTQoBZhIc",
            "Lk5pcmdlLkNvcmUuUnBjQ2FsbEFyZ3NFbXB0eRocLk5pcmdlLkNvcmUuUnBj",
            "Q2FsbEFyZ3NFbXB0eSIMyrIdAggByrIdAhABEkIKAWcSES5OaXJnZS5Db3Jl",
            "LmdhcmdzGhwuTmlyZ2UuQ29yZS5ScGNDYWxsQXJnc0VtcHR5IgzKsh0CCALK",
            "sh0CEAESTQoBaBIcLk5pcmdlLkNvcmUuUnBjQ2FsbEFyZ3NFbXB0eRocLk5p",
            "cmdlLkNvcmUuUnBjQ2FsbEFyZ3NFbXB0eSIMyrIdAggDyrIdAhAAEkIKAXAS",
            "ES5OaXJnZS5Db3JlLnBhcmdzGhwuTmlyZ2UuQ29yZS5ScGNDYWxsQXJnc0Vt",
            "cHR5IgzKsh0CCATKsh0CEAASNgoBcRIRLk5pcmdlLkNvcmUucWFyZ3MaEC5O",
            "aXJnZS5Db3JlLnFyZXQiDMqyHQIIBcqyHQIQABoGqqwdAggBQg2qAgpOaXJn",
            "ZS5Db3JlYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Nirge.Core.RpcProtoReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.gargs), global::Nirge.Core.gargs.Parser, new[]{ "A", "B", "C", "D" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.pargs), global::Nirge.Core.pargs.Parser, new[]{ "A", "B", "C", "D" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.qargs), global::Nirge.Core.qargs.Parser, new[]{ "A", "B", "C", "D" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.qret), global::Nirge.Core.qret.Parser, new[]{ "A", "B", "C", "D" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class gargs : pb::IMessage<gargs> {
    private static readonly pb::MessageParser<gargs> _parser = new pb::MessageParser<gargs>(() => new gargs());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<gargs> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.ServiceReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public gargs() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public gargs(gargs other) : this() {
      a_ = other.a_;
      b_ = other.b_;
      c_ = other.c_;
      d_ = other.d_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public gargs Clone() {
      return new gargs(this);
    }

    /// <summary>Field number for the "a" field.</summary>
    public const int AFieldNumber = 1;
    private int a_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int A {
      get { return a_; }
      set {
        a_ = value;
      }
    }

    /// <summary>Field number for the "b" field.</summary>
    public const int BFieldNumber = 2;
    private int b_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int B {
      get { return b_; }
      set {
        b_ = value;
      }
    }

    /// <summary>Field number for the "c" field.</summary>
    public const int CFieldNumber = 3;
    private int c_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int C {
      get { return c_; }
      set {
        c_ = value;
      }
    }

    /// <summary>Field number for the "d" field.</summary>
    public const int DFieldNumber = 4;
    private static readonly pb::FieldCodec<int> _repeated_d_codec
        = pb::FieldCodec.ForInt32(34);
    private readonly pbc::RepeatedField<int> d_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> D {
      get { return d_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as gargs);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(gargs other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (A != other.A) return false;
      if (B != other.B) return false;
      if (C != other.C) return false;
      if(!d_.Equals(other.d_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (A != 0) hash ^= A.GetHashCode();
      if (B != 0) hash ^= B.GetHashCode();
      if (C != 0) hash ^= C.GetHashCode();
      hash ^= d_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (A != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(A);
      }
      if (B != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(B);
      }
      if (C != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(C);
      }
      d_.WriteTo(output, _repeated_d_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (A != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(A);
      }
      if (B != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(B);
      }
      if (C != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(C);
      }
      size += d_.CalculateSize(_repeated_d_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(gargs other) {
      if (other == null) {
        return;
      }
      if (other.A != 0) {
        A = other.A;
      }
      if (other.B != 0) {
        B = other.B;
      }
      if (other.C != 0) {
        C = other.C;
      }
      d_.Add(other.d_);
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
            A = input.ReadInt32();
            break;
          }
          case 16: {
            B = input.ReadInt32();
            break;
          }
          case 24: {
            C = input.ReadInt32();
            break;
          }
          case 34:
          case 32: {
            d_.AddEntriesFrom(input, _repeated_d_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class pargs : pb::IMessage<pargs> {
    private static readonly pb::MessageParser<pargs> _parser = new pb::MessageParser<pargs>(() => new pargs());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<pargs> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.ServiceReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pargs() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pargs(pargs other) : this() {
      a_ = other.a_;
      b_ = other.b_;
      c_ = other.c_;
      d_ = other.d_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pargs Clone() {
      return new pargs(this);
    }

    /// <summary>Field number for the "a" field.</summary>
    public const int AFieldNumber = 1;
    private int a_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int A {
      get { return a_; }
      set {
        a_ = value;
      }
    }

    /// <summary>Field number for the "b" field.</summary>
    public const int BFieldNumber = 2;
    private int b_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int B {
      get { return b_; }
      set {
        b_ = value;
      }
    }

    /// <summary>Field number for the "c" field.</summary>
    public const int CFieldNumber = 3;
    private int c_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int C {
      get { return c_; }
      set {
        c_ = value;
      }
    }

    /// <summary>Field number for the "d" field.</summary>
    public const int DFieldNumber = 4;
    private static readonly pb::FieldCodec<int> _repeated_d_codec
        = pb::FieldCodec.ForInt32(34);
    private readonly pbc::RepeatedField<int> d_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> D {
      get { return d_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as pargs);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(pargs other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (A != other.A) return false;
      if (B != other.B) return false;
      if (C != other.C) return false;
      if(!d_.Equals(other.d_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (A != 0) hash ^= A.GetHashCode();
      if (B != 0) hash ^= B.GetHashCode();
      if (C != 0) hash ^= C.GetHashCode();
      hash ^= d_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (A != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(A);
      }
      if (B != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(B);
      }
      if (C != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(C);
      }
      d_.WriteTo(output, _repeated_d_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (A != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(A);
      }
      if (B != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(B);
      }
      if (C != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(C);
      }
      size += d_.CalculateSize(_repeated_d_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pargs other) {
      if (other == null) {
        return;
      }
      if (other.A != 0) {
        A = other.A;
      }
      if (other.B != 0) {
        B = other.B;
      }
      if (other.C != 0) {
        C = other.C;
      }
      d_.Add(other.d_);
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
            A = input.ReadInt32();
            break;
          }
          case 16: {
            B = input.ReadInt32();
            break;
          }
          case 24: {
            C = input.ReadInt32();
            break;
          }
          case 34:
          case 32: {
            d_.AddEntriesFrom(input, _repeated_d_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class qargs : pb::IMessage<qargs> {
    private static readonly pb::MessageParser<qargs> _parser = new pb::MessageParser<qargs>(() => new qargs());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<qargs> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.ServiceReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qargs() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qargs(qargs other) : this() {
      a_ = other.a_;
      b_ = other.b_;
      c_ = other.c_;
      d_ = other.d_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qargs Clone() {
      return new qargs(this);
    }

    /// <summary>Field number for the "a" field.</summary>
    public const int AFieldNumber = 1;
    private int a_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int A {
      get { return a_; }
      set {
        a_ = value;
      }
    }

    /// <summary>Field number for the "b" field.</summary>
    public const int BFieldNumber = 2;
    private int b_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int B {
      get { return b_; }
      set {
        b_ = value;
      }
    }

    /// <summary>Field number for the "c" field.</summary>
    public const int CFieldNumber = 3;
    private int c_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int C {
      get { return c_; }
      set {
        c_ = value;
      }
    }

    /// <summary>Field number for the "d" field.</summary>
    public const int DFieldNumber = 4;
    private static readonly pb::FieldCodec<int> _repeated_d_codec
        = pb::FieldCodec.ForInt32(34);
    private readonly pbc::RepeatedField<int> d_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> D {
      get { return d_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as qargs);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(qargs other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (A != other.A) return false;
      if (B != other.B) return false;
      if (C != other.C) return false;
      if(!d_.Equals(other.d_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (A != 0) hash ^= A.GetHashCode();
      if (B != 0) hash ^= B.GetHashCode();
      if (C != 0) hash ^= C.GetHashCode();
      hash ^= d_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (A != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(A);
      }
      if (B != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(B);
      }
      if (C != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(C);
      }
      d_.WriteTo(output, _repeated_d_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (A != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(A);
      }
      if (B != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(B);
      }
      if (C != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(C);
      }
      size += d_.CalculateSize(_repeated_d_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(qargs other) {
      if (other == null) {
        return;
      }
      if (other.A != 0) {
        A = other.A;
      }
      if (other.B != 0) {
        B = other.B;
      }
      if (other.C != 0) {
        C = other.C;
      }
      d_.Add(other.d_);
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
            A = input.ReadInt32();
            break;
          }
          case 16: {
            B = input.ReadInt32();
            break;
          }
          case 24: {
            C = input.ReadInt32();
            break;
          }
          case 34:
          case 32: {
            d_.AddEntriesFrom(input, _repeated_d_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class qret : pb::IMessage<qret> {
    private static readonly pb::MessageParser<qret> _parser = new pb::MessageParser<qret>(() => new qret());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<qret> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.ServiceReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qret() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qret(qret other) : this() {
      a_ = other.a_;
      b_ = other.b_;
      c_ = other.c_;
      d_ = other.d_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public qret Clone() {
      return new qret(this);
    }

    /// <summary>Field number for the "a" field.</summary>
    public const int AFieldNumber = 1;
    private int a_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int A {
      get { return a_; }
      set {
        a_ = value;
      }
    }

    /// <summary>Field number for the "b" field.</summary>
    public const int BFieldNumber = 2;
    private int b_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int B {
      get { return b_; }
      set {
        b_ = value;
      }
    }

    /// <summary>Field number for the "c" field.</summary>
    public const int CFieldNumber = 3;
    private int c_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int C {
      get { return c_; }
      set {
        c_ = value;
      }
    }

    /// <summary>Field number for the "d" field.</summary>
    public const int DFieldNumber = 4;
    private static readonly pb::FieldCodec<int> _repeated_d_codec
        = pb::FieldCodec.ForInt32(34);
    private readonly pbc::RepeatedField<int> d_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> D {
      get { return d_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as qret);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(qret other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (A != other.A) return false;
      if (B != other.B) return false;
      if (C != other.C) return false;
      if(!d_.Equals(other.d_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (A != 0) hash ^= A.GetHashCode();
      if (B != 0) hash ^= B.GetHashCode();
      if (C != 0) hash ^= C.GetHashCode();
      hash ^= d_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (A != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(A);
      }
      if (B != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(B);
      }
      if (C != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(C);
      }
      d_.WriteTo(output, _repeated_d_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (A != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(A);
      }
      if (B != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(B);
      }
      if (C != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(C);
      }
      size += d_.CalculateSize(_repeated_d_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(qret other) {
      if (other == null) {
        return;
      }
      if (other.A != 0) {
        A = other.A;
      }
      if (other.B != 0) {
        B = other.B;
      }
      if (other.C != 0) {
        C = other.C;
      }
      d_.Add(other.d_);
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
            A = input.ReadInt32();
            break;
          }
          case 16: {
            B = input.ReadInt32();
            break;
          }
          case 24: {
            C = input.ReadInt32();
            break;
          }
          case 34:
          case 32: {
            d_.AddEntriesFrom(input, _repeated_d_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
