// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: game.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nirge.Core {

  /// <summary>Holder for reflection information generated from game.proto</summary>
  public static partial class GameReflection {

    #region Descriptor
    /// <summary>File descriptor for game.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GameReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgpnYW1lLnByb3RvEgpOaXJnZS5Db3JlGg9EYXRhUHJvdG8ucHJvdG8iOgoB",
            "YhIJCgFmGAEgASgIEgkKAWcYAiABKAUSCQoBaBgDIAEoAhIJCgFwGAQgASgD",
            "EgkKAXEYBSABKAkikgEKAWESFwoBZhgBIAEoBUIMsqwdAggBsqwdAhAQEhcK",
            "AWcYAiABKAVCDLKsHQIIArKsHQIQEBIJCgFoGAMgASgDEgkKAXAYBCABKAkS",
            "CQoBcRgFIAMoBRIYCgFtGAYgASgLMg0uTmlyZ2UuQ29yZS5iEhgKAW4YByAD",
            "KAsyDS5OaXJnZS5Db3JlLmI6BqqsHQIIAUINqgIKTmlyZ2UuQ29yZWIGcHJv",
            "dG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Nirge.Core.DataProtoReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.b), global::Nirge.Core.b.Parser, new[]{ "F", "G", "H", "P", "Q" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Nirge.Core.a), global::Nirge.Core.a.Parser, new[]{ "F", "G", "H", "P", "Q", "M", "N" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class b : pb::IMessage<b> {
    private static readonly pb::MessageParser<b> _parser = new pb::MessageParser<b>(() => new b());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<b> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.GameReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public b() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public b(b other) : this() {
      f_ = other.f_;
      g_ = other.g_;
      h_ = other.h_;
      p_ = other.p_;
      q_ = other.q_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public b Clone() {
      return new b(this);
    }

    /// <summary>Field number for the "f" field.</summary>
    public const int FFieldNumber = 1;
    private bool f_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool F {
      get { return f_; }
      set {
        f_ = value;
      }
    }

    /// <summary>Field number for the "g" field.</summary>
    public const int GFieldNumber = 2;
    private int g_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int G {
      get { return g_; }
      set {
        g_ = value;
      }
    }

    /// <summary>Field number for the "h" field.</summary>
    public const int HFieldNumber = 3;
    private float h_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float H {
      get { return h_; }
      set {
        h_ = value;
      }
    }

    /// <summary>Field number for the "p" field.</summary>
    public const int PFieldNumber = 4;
    private long p_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long P {
      get { return p_; }
      set {
        p_ = value;
      }
    }

    /// <summary>Field number for the "q" field.</summary>
    public const int QFieldNumber = 5;
    private string q_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Q {
      get { return q_; }
      set {
        q_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as b);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(b other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (F != other.F) return false;
      if (G != other.G) return false;
      if (H != other.H) return false;
      if (P != other.P) return false;
      if (Q != other.Q) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (F != false) hash ^= F.GetHashCode();
      if (G != 0) hash ^= G.GetHashCode();
      if (H != 0F) hash ^= H.GetHashCode();
      if (P != 0L) hash ^= P.GetHashCode();
      if (Q.Length != 0) hash ^= Q.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (F != false) {
        output.WriteRawTag(8);
        output.WriteBool(F);
      }
      if (G != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(G);
      }
      if (H != 0F) {
        output.WriteRawTag(29);
        output.WriteFloat(H);
      }
      if (P != 0L) {
        output.WriteRawTag(32);
        output.WriteInt64(P);
      }
      if (Q.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Q);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (F != false) {
        size += 1 + 1;
      }
      if (G != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(G);
      }
      if (H != 0F) {
        size += 1 + 4;
      }
      if (P != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(P);
      }
      if (Q.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Q);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(b other) {
      if (other == null) {
        return;
      }
      if (other.F != false) {
        F = other.F;
      }
      if (other.G != 0) {
        G = other.G;
      }
      if (other.H != 0F) {
        H = other.H;
      }
      if (other.P != 0L) {
        P = other.P;
      }
      if (other.Q.Length != 0) {
        Q = other.Q;
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
            F = input.ReadBool();
            break;
          }
          case 16: {
            G = input.ReadInt32();
            break;
          }
          case 29: {
            H = input.ReadFloat();
            break;
          }
          case 32: {
            P = input.ReadInt64();
            break;
          }
          case 42: {
            Q = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class a : pb::IMessage<a> {
    private static readonly pb::MessageParser<a> _parser = new pb::MessageParser<a>(() => new a());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<a> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Nirge.Core.GameReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public a() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public a(a other) : this() {
      f_ = other.f_;
      g_ = other.g_;
      h_ = other.h_;
      p_ = other.p_;
      q_ = other.q_.Clone();
      M = other.m_ != null ? other.M.Clone() : null;
      n_ = other.n_.Clone();
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public a Clone() {
      return new a(this);
    }

    /// <summary>Field number for the "f" field.</summary>
    public const int FFieldNumber = 1;
    private int f_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int F {
      get { return f_; }
      set {
        f_ = value;
      }
    }

    /// <summary>Field number for the "g" field.</summary>
    public const int GFieldNumber = 2;
    private int g_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int G {
      get { return g_; }
      set {
        g_ = value;
      }
    }

    /// <summary>Field number for the "h" field.</summary>
    public const int HFieldNumber = 3;
    private long h_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long H {
      get { return h_; }
      set {
        h_ = value;
      }
    }

    /// <summary>Field number for the "p" field.</summary>
    public const int PFieldNumber = 4;
    private string p_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string P {
      get { return p_; }
      set {
        p_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "q" field.</summary>
    public const int QFieldNumber = 5;
    private static readonly pb::FieldCodec<int> _repeated_q_codec
        = pb::FieldCodec.ForInt32(42);
    private readonly pbc::RepeatedField<int> q_ = new pbc::RepeatedField<int>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<int> Q {
      get { return q_; }
    }

    /// <summary>Field number for the "m" field.</summary>
    public const int MFieldNumber = 6;
    private global::Nirge.Core.b m_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Nirge.Core.b M {
      get { return m_; }
      set {
        m_ = value;
      }
    }

    /// <summary>Field number for the "n" field.</summary>
    public const int NFieldNumber = 7;
    private static readonly pb::FieldCodec<global::Nirge.Core.b> _repeated_n_codec
        = pb::FieldCodec.ForMessage(58, global::Nirge.Core.b.Parser);
    private readonly pbc::RepeatedField<global::Nirge.Core.b> n_ = new pbc::RepeatedField<global::Nirge.Core.b>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Nirge.Core.b> N {
      get { return n_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as a);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(a other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (F != other.F) return false;
      if (G != other.G) return false;
      if (H != other.H) return false;
      if (P != other.P) return false;
      if(!q_.Equals(other.q_)) return false;
      if (!object.Equals(M, other.M)) return false;
      if(!n_.Equals(other.n_)) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (F != 0) hash ^= F.GetHashCode();
      if (G != 0) hash ^= G.GetHashCode();
      if (H != 0L) hash ^= H.GetHashCode();
      if (P.Length != 0) hash ^= P.GetHashCode();
      hash ^= q_.GetHashCode();
      if (m_ != null) hash ^= M.GetHashCode();
      hash ^= n_.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (F != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(F);
      }
      if (G != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(G);
      }
      if (H != 0L) {
        output.WriteRawTag(24);
        output.WriteInt64(H);
      }
      if (P.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(P);
      }
      q_.WriteTo(output, _repeated_q_codec);
      if (m_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(M);
      }
      n_.WriteTo(output, _repeated_n_codec);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (F != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(F);
      }
      if (G != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(G);
      }
      if (H != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(H);
      }
      if (P.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(P);
      }
      size += q_.CalculateSize(_repeated_q_codec);
      if (m_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(M);
      }
      size += n_.CalculateSize(_repeated_n_codec);
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(a other) {
      if (other == null) {
        return;
      }
      if (other.F != 0) {
        F = other.F;
      }
      if (other.G != 0) {
        G = other.G;
      }
      if (other.H != 0L) {
        H = other.H;
      }
      if (other.P.Length != 0) {
        P = other.P;
      }
      q_.Add(other.q_);
      if (other.m_ != null) {
        if (m_ == null) {
          m_ = new global::Nirge.Core.b();
        }
        M.MergeFrom(other.M);
      }
      n_.Add(other.n_);
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
            F = input.ReadInt32();
            break;
          }
          case 16: {
            G = input.ReadInt32();
            break;
          }
          case 24: {
            H = input.ReadInt64();
            break;
          }
          case 34: {
            P = input.ReadString();
            break;
          }
          case 42:
          case 40: {
            q_.AddEntriesFrom(input, _repeated_q_codec);
            break;
          }
          case 50: {
            if (m_ == null) {
              m_ = new global::Nirge.Core.b();
            }
            input.ReadMessage(m_);
            break;
          }
          case 58: {
            n_.AddEntriesFrom(input, _repeated_n_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code