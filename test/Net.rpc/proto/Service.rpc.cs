// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: service.proto
#pragma warning disable 1591, 0612, 3021

#region Designer generated code

using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using pb = global::Google.Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nirge.Core;
using log4net;
using System;

namespace Nirge.Core {
  #region CGameRpcService
  public interface IGameRpcService : IRpcService {
    void f(int channel);
    void g(int channel, Nirge.Core.gargs args);
    void h(int channel);
    void p(int channel, Nirge.Core.pargs args);
    Nirge.Core.qret q(int channel, Nirge.Core.qargs args);
    Nirge.Core.mret m(int channel, Nirge.Core.margs args);
  }
  public class CGameRpcCaller : CRpcCaller {
    public CGameRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs)
    	: base(args, log, stream, communicator, stubs, global::Nirge.Core.ServiceReflection.Descriptor.Services[0], 0) {}
    public Task<Nirge.Core.RpcCallArgsEmpty> f(int channel = 0){
      return CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, 0, ArgsEmpty);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> g(Nirge.Core.gargs args, int channel = 0){
      return CallAsync<Nirge.Core.gargs, Nirge.Core.RpcCallArgsEmpty>(channel, 0, args);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> h(int channel = 0){
      return CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, 0, ArgsEmpty);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> p(Nirge.Core.pargs args, int channel = 0){
      return CallAsync<Nirge.Core.pargs, Nirge.Core.RpcCallArgsEmpty>(channel, 0, args);
    }
    public Task<Nirge.Core.qret> q(Nirge.Core.qargs args, int channel = 0){
      return CallAsync<Nirge.Core.qargs, Nirge.Core.qret>(channel, 0, args);
    }
    public Task<Nirge.Core.mret> m(Nirge.Core.margs args, int channel = 0){
      return CallAsync<Nirge.Core.margs, Nirge.Core.mret>(channel, 0, args);
    }
  }
  [CRpcService(0)]
  public class CGameRpcCallee : CRpcCallee<IGameRpcService> {
    public CGameRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, IGameRpcService service)
    	: base(args, log, stream, communicator, global::Nirge.Core.ServiceReflection.Descriptor.Services[0], service) {}
    public override void Call(int channel, Nirge.Core.RpcCallReq req) {
      switch (req.Call) {
      case 0:
        CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.f(channel);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.gargs, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.g(channel, args);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.h(channel);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.pargs, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.p(channel, args);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.qargs, Nirge.Core.qret>(channel, req, (_, args) => {
          return _service.q(channel, args);
        });
        break;
      case 0:
        CallAsync<Nirge.Core.margs, Nirge.Core.mret>(channel, req, (_, args) => {
          return _service.m(channel, args);
        });
        break;
      default:
        base.Call(channel, req);
        break;
      }
    }
  }
  #endregion
  #region CWebRpcService
  public interface IWebRpcService : IRpcService {
    void f(int channel);
    void g(int channel, Nirge.Core.gargs args);
    void h(int channel);
    void p(int channel, Nirge.Core.pargs args);
    Nirge.Core.qret q(int channel, Nirge.Core.qargs args);
    Nirge.Core.mret m(int channel, Nirge.Core.margs args);
  }
  public class CWebRpcCaller : CRpcCaller {
    public CWebRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs)
    	: base(args, log, stream, communicator, stubs, global::Nirge.Core.ServiceReflection.Descriptor.Services[1], 0) {}
    public Task<Nirge.Core.RpcCallArgsEmpty> f(int channel = 0){
      return CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, 0, ArgsEmpty);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> g(Nirge.Core.gargs args, int channel = 0){
      return CallAsync<Nirge.Core.gargs, Nirge.Core.RpcCallArgsEmpty>(channel, 0, args);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> h(int channel = 0){
      return CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, 0, ArgsEmpty);
    }
    public Task<Nirge.Core.RpcCallArgsEmpty> p(Nirge.Core.pargs args, int channel = 0){
      return CallAsync<Nirge.Core.pargs, Nirge.Core.RpcCallArgsEmpty>(channel, 0, args);
    }
    public Task<Nirge.Core.qret> q(Nirge.Core.qargs args, int channel = 0){
      return CallAsync<Nirge.Core.qargs, Nirge.Core.qret>(channel, 0, args);
    }
    public Task<Nirge.Core.mret> m(Nirge.Core.margs args, int channel = 0){
      return CallAsync<Nirge.Core.margs, Nirge.Core.mret>(channel, 0, args);
    }
  }
  [CRpcService(0)]
  public class CWebRpcCallee : CRpcCallee<IWebRpcService> {
    public CWebRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, IWebRpcService service)
    	: base(args, log, stream, communicator, global::Nirge.Core.ServiceReflection.Descriptor.Services[1], service) {}
    public override void Call(int channel, Nirge.Core.RpcCallReq req) {
      switch (req.Call) {
      case 0:
        CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.f(channel);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.gargs, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.g(channel, args);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.RpcCallArgsEmpty, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.h(channel);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.pargs, Nirge.Core.RpcCallArgsEmpty>(channel, req, (_, args) => {
          _service.p(channel, args);
          return ArgsEmpty;
        });
        break;
      case 0:
        CallAsync<Nirge.Core.qargs, Nirge.Core.qret>(channel, req, (_, args) => {
          return _service.q(channel, args);
        });
        break;
      case 0:
        CallAsync<Nirge.Core.margs, Nirge.Core.mret>(channel, req, (_, args) => {
          return _service.m(channel, args);
        });
        break;
      default:
        base.Call(channel, req);
        break;
      }
    }
  }
  #endregion
}

#endregion
