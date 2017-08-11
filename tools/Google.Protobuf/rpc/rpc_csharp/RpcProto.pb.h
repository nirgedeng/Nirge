// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: RpcProto.proto

#ifndef PROTOBUF_RpcProto_2eproto__INCLUDED
#define PROTOBUF_RpcProto_2eproto__INCLUDED

#include <string>

#include <google/protobuf/stubs/common.h>

#if GOOGLE_PROTOBUF_VERSION < 3003000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please update
#error your headers.
#endif
#if 3003002 < GOOGLE_PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_table_driven.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/unknown_field_set.h>
#include <google/protobuf/descriptor.pb.h>
#include "Proto.pb.h"
// @@protoc_insertion_point(includes)
namespace Nirge {
namespace Core {
class RpcCallArgsEmpty;
class RpcCallArgsEmptyDefaultTypeInternal;
extern RpcCallArgsEmptyDefaultTypeInternal _RpcCallArgsEmpty_default_instance_;
class RpcCallExceptionRsp;
class RpcCallExceptionRspDefaultTypeInternal;
extern RpcCallExceptionRspDefaultTypeInternal _RpcCallExceptionRsp_default_instance_;
class RpcCallReq;
class RpcCallReqDefaultTypeInternal;
extern RpcCallReqDefaultTypeInternal _RpcCallReq_default_instance_;
class RpcCallRsp;
class RpcCallRspDefaultTypeInternal;
extern RpcCallRspDefaultTypeInternal _RpcCallRsp_default_instance_;
class RpcServiceCallOption;
class RpcServiceCallOptionDefaultTypeInternal;
extern RpcServiceCallOptionDefaultTypeInternal _RpcServiceCallOption_default_instance_;
class RpcServiceOption;
class RpcServiceOptionDefaultTypeInternal;
extern RpcServiceOptionDefaultTypeInternal _RpcServiceOption_default_instance_;
}  // namespace Core
}  // namespace Nirge

namespace Nirge {
namespace Core {

namespace protobuf_RpcProto_2eproto {
// Internal implementation detail -- do not call these.
struct TableStruct {
  static const ::google::protobuf::internal::ParseTableField entries[];
  static const ::google::protobuf::internal::AuxillaryParseTableField aux[];
  static const ::google::protobuf::internal::ParseTable schema[];
  static const ::google::protobuf::uint32 offsets[];
  static void InitDefaultsImpl();
  static void Shutdown();
};
void AddDescriptors();
void InitDefaults();
}  // namespace protobuf_RpcProto_2eproto

// ===================================================================

class RpcServiceOption : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcServiceOption) */ {
 public:
  RpcServiceOption();
  virtual ~RpcServiceOption();

  RpcServiceOption(const RpcServiceOption& from);

  inline RpcServiceOption& operator=(const RpcServiceOption& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcServiceOption& default_instance();

  static inline const RpcServiceOption* internal_default_instance() {
    return reinterpret_cast<const RpcServiceOption*>(
               &_RpcServiceOption_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    0;

  void Swap(RpcServiceOption* other);

  // implements Message ----------------------------------------------

  inline RpcServiceOption* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcServiceOption* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcServiceOption& from);
  void MergeFrom(const RpcServiceOption& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcServiceOption* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // int32 Uid = 1;
  void clear_uid();
  static const int kUidFieldNumber = 1;
  ::google::protobuf::int32 uid() const;
  void set_uid(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcServiceOption)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::int32 uid_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// -------------------------------------------------------------------

class RpcServiceCallOption : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcServiceCallOption) */ {
 public:
  RpcServiceCallOption();
  virtual ~RpcServiceCallOption();

  RpcServiceCallOption(const RpcServiceCallOption& from);

  inline RpcServiceCallOption& operator=(const RpcServiceCallOption& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcServiceCallOption& default_instance();

  static inline const RpcServiceCallOption* internal_default_instance() {
    return reinterpret_cast<const RpcServiceCallOption*>(
               &_RpcServiceCallOption_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    1;

  void Swap(RpcServiceCallOption* other);

  // implements Message ----------------------------------------------

  inline RpcServiceCallOption* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcServiceCallOption* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcServiceCallOption& from);
  void MergeFrom(const RpcServiceCallOption& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcServiceCallOption* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // int32 Uid = 1;
  void clear_uid();
  static const int kUidFieldNumber = 1;
  ::google::protobuf::int32 uid() const;
  void set_uid(::google::protobuf::int32 value);

  // bool IsOneWay = 2;
  void clear_isoneway();
  static const int kIsOneWayFieldNumber = 2;
  bool isoneway() const;
  void set_isoneway(bool value);

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcServiceCallOption)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::int32 uid_;
  bool isoneway_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// -------------------------------------------------------------------

class RpcCallArgsEmpty : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcCallArgsEmpty) */ {
 public:
  RpcCallArgsEmpty();
  virtual ~RpcCallArgsEmpty();

  RpcCallArgsEmpty(const RpcCallArgsEmpty& from);

  inline RpcCallArgsEmpty& operator=(const RpcCallArgsEmpty& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcCallArgsEmpty& default_instance();

  static inline const RpcCallArgsEmpty* internal_default_instance() {
    return reinterpret_cast<const RpcCallArgsEmpty*>(
               &_RpcCallArgsEmpty_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    2;

  void Swap(RpcCallArgsEmpty* other);

  // implements Message ----------------------------------------------

  inline RpcCallArgsEmpty* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcCallArgsEmpty* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcCallArgsEmpty& from);
  void MergeFrom(const RpcCallArgsEmpty& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcCallArgsEmpty* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcCallArgsEmpty)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// -------------------------------------------------------------------

class RpcCallReq : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcCallReq) */ {
 public:
  RpcCallReq();
  virtual ~RpcCallReq();

  RpcCallReq(const RpcCallReq& from);

  inline RpcCallReq& operator=(const RpcCallReq& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcCallReq& default_instance();

  static inline const RpcCallReq* internal_default_instance() {
    return reinterpret_cast<const RpcCallReq*>(
               &_RpcCallReq_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    3;

  void Swap(RpcCallReq* other);

  // implements Message ----------------------------------------------

  inline RpcCallReq* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcCallReq* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcCallReq& from);
  void MergeFrom(const RpcCallReq& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcCallReq* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // bytes Args = 4;
  void clear_args();
  static const int kArgsFieldNumber = 4;
  const ::std::string& args() const;
  void set_args(const ::std::string& value);
  #if LANG_CXX11
  void set_args(::std::string&& value);
  #endif
  void set_args(const char* value);
  void set_args(const void* value, size_t size);
  ::std::string* mutable_args();
  ::std::string* release_args();
  void set_allocated_args(::std::string* args);

  // int32 Serial = 1;
  void clear_serial();
  static const int kSerialFieldNumber = 1;
  ::google::protobuf::int32 serial() const;
  void set_serial(::google::protobuf::int32 value);

  // int32 Service = 2;
  void clear_service();
  static const int kServiceFieldNumber = 2;
  ::google::protobuf::int32 service() const;
  void set_service(::google::protobuf::int32 value);

  // int32 Call = 3;
  void clear_call();
  static const int kCallFieldNumber = 3;
  ::google::protobuf::int32 call() const;
  void set_call(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcCallReq)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::internal::ArenaStringPtr args_;
  ::google::protobuf::int32 serial_;
  ::google::protobuf::int32 service_;
  ::google::protobuf::int32 call_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// -------------------------------------------------------------------

class RpcCallRsp : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcCallRsp) */ {
 public:
  RpcCallRsp();
  virtual ~RpcCallRsp();

  RpcCallRsp(const RpcCallRsp& from);

  inline RpcCallRsp& operator=(const RpcCallRsp& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcCallRsp& default_instance();

  static inline const RpcCallRsp* internal_default_instance() {
    return reinterpret_cast<const RpcCallRsp*>(
               &_RpcCallRsp_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    4;

  void Swap(RpcCallRsp* other);

  // implements Message ----------------------------------------------

  inline RpcCallRsp* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcCallRsp* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcCallRsp& from);
  void MergeFrom(const RpcCallRsp& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcCallRsp* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // bytes Ret = 4;
  void clear_ret();
  static const int kRetFieldNumber = 4;
  const ::std::string& ret() const;
  void set_ret(const ::std::string& value);
  #if LANG_CXX11
  void set_ret(::std::string&& value);
  #endif
  void set_ret(const char* value);
  void set_ret(const void* value, size_t size);
  ::std::string* mutable_ret();
  ::std::string* release_ret();
  void set_allocated_ret(::std::string* ret);

  // int32 Serial = 1;
  void clear_serial();
  static const int kSerialFieldNumber = 1;
  ::google::protobuf::int32 serial() const;
  void set_serial(::google::protobuf::int32 value);

  // int32 Service = 2;
  void clear_service();
  static const int kServiceFieldNumber = 2;
  ::google::protobuf::int32 service() const;
  void set_service(::google::protobuf::int32 value);

  // int32 Call = 3;
  void clear_call();
  static const int kCallFieldNumber = 3;
  ::google::protobuf::int32 call() const;
  void set_call(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcCallRsp)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::internal::ArenaStringPtr ret_;
  ::google::protobuf::int32 serial_;
  ::google::protobuf::int32 service_;
  ::google::protobuf::int32 call_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// -------------------------------------------------------------------

class RpcCallExceptionRsp : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:Nirge.Core.RpcCallExceptionRsp) */ {
 public:
  RpcCallExceptionRsp();
  virtual ~RpcCallExceptionRsp();

  RpcCallExceptionRsp(const RpcCallExceptionRsp& from);

  inline RpcCallExceptionRsp& operator=(const RpcCallExceptionRsp& from) {
    CopyFrom(from);
    return *this;
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const RpcCallExceptionRsp& default_instance();

  static inline const RpcCallExceptionRsp* internal_default_instance() {
    return reinterpret_cast<const RpcCallExceptionRsp*>(
               &_RpcCallExceptionRsp_default_instance_);
  }
  static PROTOBUF_CONSTEXPR int const kIndexInFileMessages =
    5;

  void Swap(RpcCallExceptionRsp* other);

  // implements Message ----------------------------------------------

  inline RpcCallExceptionRsp* New() const PROTOBUF_FINAL { return New(NULL); }

  RpcCallExceptionRsp* New(::google::protobuf::Arena* arena) const PROTOBUF_FINAL;
  void CopyFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void MergeFrom(const ::google::protobuf::Message& from) PROTOBUF_FINAL;
  void CopyFrom(const RpcCallExceptionRsp& from);
  void MergeFrom(const RpcCallExceptionRsp& from);
  void Clear() PROTOBUF_FINAL;
  bool IsInitialized() const PROTOBUF_FINAL;

  size_t ByteSizeLong() const PROTOBUF_FINAL;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input) PROTOBUF_FINAL;
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const PROTOBUF_FINAL;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* target) const PROTOBUF_FINAL;
  int GetCachedSize() const PROTOBUF_FINAL { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const PROTOBUF_FINAL;
  void InternalSwap(RpcCallExceptionRsp* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return NULL;
  }
  inline void* MaybeArenaPtr() const {
    return NULL;
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const PROTOBUF_FINAL;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // int32 Serial = 1;
  void clear_serial();
  static const int kSerialFieldNumber = 1;
  ::google::protobuf::int32 serial() const;
  void set_serial(::google::protobuf::int32 value);

  // int32 Service = 2;
  void clear_service();
  static const int kServiceFieldNumber = 2;
  ::google::protobuf::int32 service() const;
  void set_service(::google::protobuf::int32 value);

  // int32 Call = 3;
  void clear_call();
  static const int kCallFieldNumber = 3;
  ::google::protobuf::int32 call() const;
  void set_call(::google::protobuf::int32 value);

  // int32 Exception = 4;
  void clear_exception();
  static const int kExceptionFieldNumber = 4;
  ::google::protobuf::int32 exception() const;
  void set_exception(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:Nirge.Core.RpcCallExceptionRsp)
 private:

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::int32 serial_;
  ::google::protobuf::int32 service_;
  ::google::protobuf::int32 call_;
  ::google::protobuf::int32 exception_;
  mutable int _cached_size_;
  friend struct protobuf_RpcProto_2eproto::TableStruct;
};
// ===================================================================

static const int kRpcServiceFieldNumber = 60001;
extern ::google::protobuf::internal::ExtensionIdentifier< ::google::protobuf::ServiceOptions,
    ::google::protobuf::internal::MessageTypeTraits< ::Nirge::Core::RpcServiceOption >, 11, false >
  RpcService;
static const int kRpcServiceCallFieldNumber = 60002;
extern ::google::protobuf::internal::ExtensionIdentifier< ::google::protobuf::MethodOptions,
    ::google::protobuf::internal::MessageTypeTraits< ::Nirge::Core::RpcServiceCallOption >, 11, false >
  RpcServiceCall;

// ===================================================================

#if !PROTOBUF_INLINE_NOT_IN_HEADERS
// RpcServiceOption

// int32 Uid = 1;
inline void RpcServiceOption::clear_uid() {
  uid_ = 0;
}
inline ::google::protobuf::int32 RpcServiceOption::uid() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcServiceOption.Uid)
  return uid_;
}
inline void RpcServiceOption::set_uid(::google::protobuf::int32 value) {
  
  uid_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcServiceOption.Uid)
}

// -------------------------------------------------------------------

// RpcServiceCallOption

// int32 Uid = 1;
inline void RpcServiceCallOption::clear_uid() {
  uid_ = 0;
}
inline ::google::protobuf::int32 RpcServiceCallOption::uid() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcServiceCallOption.Uid)
  return uid_;
}
inline void RpcServiceCallOption::set_uid(::google::protobuf::int32 value) {
  
  uid_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcServiceCallOption.Uid)
}

// bool IsOneWay = 2;
inline void RpcServiceCallOption::clear_isoneway() {
  isoneway_ = false;
}
inline bool RpcServiceCallOption::isoneway() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcServiceCallOption.IsOneWay)
  return isoneway_;
}
inline void RpcServiceCallOption::set_isoneway(bool value) {
  
  isoneway_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcServiceCallOption.IsOneWay)
}

// -------------------------------------------------------------------

// RpcCallArgsEmpty

// -------------------------------------------------------------------

// RpcCallReq

// int32 Serial = 1;
inline void RpcCallReq::clear_serial() {
  serial_ = 0;
}
inline ::google::protobuf::int32 RpcCallReq::serial() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallReq.Serial)
  return serial_;
}
inline void RpcCallReq::set_serial(::google::protobuf::int32 value) {
  
  serial_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallReq.Serial)
}

// int32 Service = 2;
inline void RpcCallReq::clear_service() {
  service_ = 0;
}
inline ::google::protobuf::int32 RpcCallReq::service() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallReq.Service)
  return service_;
}
inline void RpcCallReq::set_service(::google::protobuf::int32 value) {
  
  service_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallReq.Service)
}

// int32 Call = 3;
inline void RpcCallReq::clear_call() {
  call_ = 0;
}
inline ::google::protobuf::int32 RpcCallReq::call() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallReq.Call)
  return call_;
}
inline void RpcCallReq::set_call(::google::protobuf::int32 value) {
  
  call_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallReq.Call)
}

// bytes Args = 4;
inline void RpcCallReq::clear_args() {
  args_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline const ::std::string& RpcCallReq::args() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallReq.Args)
  return args_.GetNoArena();
}
inline void RpcCallReq::set_args(const ::std::string& value) {
  
  args_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallReq.Args)
}
#if LANG_CXX11
inline void RpcCallReq::set_args(::std::string&& value) {
  
  args_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:Nirge.Core.RpcCallReq.Args)
}
#endif
inline void RpcCallReq::set_args(const char* value) {
  GOOGLE_DCHECK(value != NULL);
  
  args_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:Nirge.Core.RpcCallReq.Args)
}
inline void RpcCallReq::set_args(const void* value, size_t size) {
  
  args_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:Nirge.Core.RpcCallReq.Args)
}
inline ::std::string* RpcCallReq::mutable_args() {
  
  // @@protoc_insertion_point(field_mutable:Nirge.Core.RpcCallReq.Args)
  return args_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* RpcCallReq::release_args() {
  // @@protoc_insertion_point(field_release:Nirge.Core.RpcCallReq.Args)
  
  return args_.ReleaseNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void RpcCallReq::set_allocated_args(::std::string* args) {
  if (args != NULL) {
    
  } else {
    
  }
  args_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), args);
  // @@protoc_insertion_point(field_set_allocated:Nirge.Core.RpcCallReq.Args)
}

// -------------------------------------------------------------------

// RpcCallRsp

// int32 Serial = 1;
inline void RpcCallRsp::clear_serial() {
  serial_ = 0;
}
inline ::google::protobuf::int32 RpcCallRsp::serial() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallRsp.Serial)
  return serial_;
}
inline void RpcCallRsp::set_serial(::google::protobuf::int32 value) {
  
  serial_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallRsp.Serial)
}

// int32 Service = 2;
inline void RpcCallRsp::clear_service() {
  service_ = 0;
}
inline ::google::protobuf::int32 RpcCallRsp::service() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallRsp.Service)
  return service_;
}
inline void RpcCallRsp::set_service(::google::protobuf::int32 value) {
  
  service_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallRsp.Service)
}

// int32 Call = 3;
inline void RpcCallRsp::clear_call() {
  call_ = 0;
}
inline ::google::protobuf::int32 RpcCallRsp::call() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallRsp.Call)
  return call_;
}
inline void RpcCallRsp::set_call(::google::protobuf::int32 value) {
  
  call_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallRsp.Call)
}

// bytes Ret = 4;
inline void RpcCallRsp::clear_ret() {
  ret_.ClearToEmptyNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline const ::std::string& RpcCallRsp::ret() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallRsp.Ret)
  return ret_.GetNoArena();
}
inline void RpcCallRsp::set_ret(const ::std::string& value) {
  
  ret_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), value);
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallRsp.Ret)
}
#if LANG_CXX11
inline void RpcCallRsp::set_ret(::std::string&& value) {
  
  ret_.SetNoArena(
    &::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::move(value));
  // @@protoc_insertion_point(field_set_rvalue:Nirge.Core.RpcCallRsp.Ret)
}
#endif
inline void RpcCallRsp::set_ret(const char* value) {
  GOOGLE_DCHECK(value != NULL);
  
  ret_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ::std::string(value));
  // @@protoc_insertion_point(field_set_char:Nirge.Core.RpcCallRsp.Ret)
}
inline void RpcCallRsp::set_ret(const void* value, size_t size) {
  
  ret_.SetNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(),
      ::std::string(reinterpret_cast<const char*>(value), size));
  // @@protoc_insertion_point(field_set_pointer:Nirge.Core.RpcCallRsp.Ret)
}
inline ::std::string* RpcCallRsp::mutable_ret() {
  
  // @@protoc_insertion_point(field_mutable:Nirge.Core.RpcCallRsp.Ret)
  return ret_.MutableNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline ::std::string* RpcCallRsp::release_ret() {
  // @@protoc_insertion_point(field_release:Nirge.Core.RpcCallRsp.Ret)
  
  return ret_.ReleaseNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited());
}
inline void RpcCallRsp::set_allocated_ret(::std::string* ret) {
  if (ret != NULL) {
    
  } else {
    
  }
  ret_.SetAllocatedNoArena(&::google::protobuf::internal::GetEmptyStringAlreadyInited(), ret);
  // @@protoc_insertion_point(field_set_allocated:Nirge.Core.RpcCallRsp.Ret)
}

// -------------------------------------------------------------------

// RpcCallExceptionRsp

// int32 Serial = 1;
inline void RpcCallExceptionRsp::clear_serial() {
  serial_ = 0;
}
inline ::google::protobuf::int32 RpcCallExceptionRsp::serial() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallExceptionRsp.Serial)
  return serial_;
}
inline void RpcCallExceptionRsp::set_serial(::google::protobuf::int32 value) {
  
  serial_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallExceptionRsp.Serial)
}

// int32 Service = 2;
inline void RpcCallExceptionRsp::clear_service() {
  service_ = 0;
}
inline ::google::protobuf::int32 RpcCallExceptionRsp::service() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallExceptionRsp.Service)
  return service_;
}
inline void RpcCallExceptionRsp::set_service(::google::protobuf::int32 value) {
  
  service_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallExceptionRsp.Service)
}

// int32 Call = 3;
inline void RpcCallExceptionRsp::clear_call() {
  call_ = 0;
}
inline ::google::protobuf::int32 RpcCallExceptionRsp::call() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallExceptionRsp.Call)
  return call_;
}
inline void RpcCallExceptionRsp::set_call(::google::protobuf::int32 value) {
  
  call_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallExceptionRsp.Call)
}

// int32 Exception = 4;
inline void RpcCallExceptionRsp::clear_exception() {
  exception_ = 0;
}
inline ::google::protobuf::int32 RpcCallExceptionRsp::exception() const {
  // @@protoc_insertion_point(field_get:Nirge.Core.RpcCallExceptionRsp.Exception)
  return exception_;
}
inline void RpcCallExceptionRsp::set_exception(::google::protobuf::int32 value) {
  
  exception_ = value;
  // @@protoc_insertion_point(field_set:Nirge.Core.RpcCallExceptionRsp.Exception)
}

#endif  // !PROTOBUF_INLINE_NOT_IN_HEADERS
// -------------------------------------------------------------------

// -------------------------------------------------------------------

// -------------------------------------------------------------------

// -------------------------------------------------------------------

// -------------------------------------------------------------------


// @@protoc_insertion_point(namespace_scope)


}  // namespace Core
}  // namespace Nirge

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_RpcProto_2eproto__INCLUDED
