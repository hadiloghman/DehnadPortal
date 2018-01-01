﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApp1.MobinOneMapfaChargingServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://services.mapfa.net", ConfigurationName="MobinOneMapfaChargingServiceReference.Charging")]
    public interface Charging {
        
        // CODEGEN: Generating message contract since element name username from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/singleChargeRequest", ReplyAction="http://services.mapfa.net/Charging/singleChargeResponse")]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse singleCharge(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/singleChargeRequest", ReplyAction="http://services.mapfa.net/Charging/singleChargeResponse")]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse> singleChargeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest request);
        
        // CODEGEN: Generating message contract since element name username from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/dynamicChargeRequest", ReplyAction="http://services.mapfa.net/Charging/dynamicChargeResponse")]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse dynamicCharge(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/dynamicChargeRequest", ReplyAction="http://services.mapfa.net/Charging/dynamicChargeResponse")]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse> dynamicChargeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest request);
        
        // CODEGEN: Generating message contract since element name username from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/sendVerificationCodeRequest", ReplyAction="http://services.mapfa.net/Charging/sendVerificationCodeResponse")]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse sendVerificationCode(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/sendVerificationCodeRequest", ReplyAction="http://services.mapfa.net/Charging/sendVerificationCodeResponse")]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse> sendVerificationCodeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest request);
        
        // CODEGEN: Generating message contract since element name username from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/verifySubscriberRequest", ReplyAction="http://services.mapfa.net/Charging/verifySubscriberResponse")]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse verifySubscriber(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://services.mapfa.net/Charging/verifySubscriberRequest", ReplyAction="http://services.mapfa.net/Charging/verifySubscriberResponse")]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse> verifySubscriberAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class singleChargeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="singleCharge", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequestBody Body;
        
        public singleChargeRequest() {
        }
        
        public singleChargeRequest(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class singleChargeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string domain;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public int channel;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string mobilenum;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string serviceId;
        
        public singleChargeRequestBody() {
        }
        
        public singleChargeRequestBody(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.channel = channel;
            this.mobilenum = mobilenum;
            this.serviceId = serviceId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class singleChargeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="singleChargeResponse", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponseBody Body;
        
        public singleChargeResponse() {
        }
        
        public singleChargeResponse(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class singleChargeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long @return;
        
        public singleChargeResponseBody() {
        }
        
        public singleChargeResponseBody(long @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class dynamicChargeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="dynamicCharge", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequestBody Body;
        
        public dynamicChargeRequest() {
        }
        
        public dynamicChargeRequest(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class dynamicChargeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string domain;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public int channel;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string mobilenum;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string serviceId;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=6)]
        public long price;
        
        public dynamicChargeRequestBody() {
        }
        
        public dynamicChargeRequestBody(string username, string password, string domain, int channel, string mobilenum, string serviceId, long price) {
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.channel = channel;
            this.mobilenum = mobilenum;
            this.serviceId = serviceId;
            this.price = price;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class dynamicChargeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="dynamicChargeResponse", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponseBody Body;
        
        public dynamicChargeResponse() {
        }
        
        public dynamicChargeResponse(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class dynamicChargeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long @return;
        
        public dynamicChargeResponseBody() {
        }
        
        public dynamicChargeResponseBody(long @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class sendVerificationCodeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="sendVerificationCode", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequestBody Body;
        
        public sendVerificationCodeRequest() {
        }
        
        public sendVerificationCodeRequest(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class sendVerificationCodeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string domain;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public int channel;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string mobilenum;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string serviceId;
        
        public sendVerificationCodeRequestBody() {
        }
        
        public sendVerificationCodeRequestBody(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.channel = channel;
            this.mobilenum = mobilenum;
            this.serviceId = serviceId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class sendVerificationCodeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="sendVerificationCodeResponse", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponseBody Body;
        
        public sendVerificationCodeResponse() {
        }
        
        public sendVerificationCodeResponse(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class sendVerificationCodeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long @return;
        
        public sendVerificationCodeResponseBody() {
        }
        
        public sendVerificationCodeResponseBody(long @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class verifySubscriberRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="verifySubscriber", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequestBody Body;
        
        public verifySubscriberRequest() {
        }
        
        public verifySubscriberRequest(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class verifySubscriberRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string domain;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public int channel;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string mobilenum;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string serviceId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string token;
        
        public verifySubscriberRequestBody() {
        }
        
        public verifySubscriberRequestBody(string username, string password, string domain, int channel, string mobilenum, string serviceId, string token) {
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.channel = channel;
            this.mobilenum = mobilenum;
            this.serviceId = serviceId;
            this.token = token;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class verifySubscriberResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="verifySubscriberResponse", Namespace="http://services.mapfa.net", Order=0)]
        public ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponseBody Body;
        
        public verifySubscriberResponse() {
        }
        
        public verifySubscriberResponse(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class verifySubscriberResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long @return;
        
        public verifySubscriberResponseBody() {
        }
        
        public verifySubscriberResponseBody(long @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ChargingChannel : ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ChargingClient : System.ServiceModel.ClientBase<ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging>, ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging {
        
        public ChargingClient() {
        }
        
        public ChargingClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ChargingClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChargingClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ChargingClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.singleCharge(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest request) {
            return base.Channel.singleCharge(request);
        }
        
        public long singleCharge(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse retVal = ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).singleCharge(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse> ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.singleChargeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest request) {
            return base.Channel.singleChargeAsync(request);
        }
        
        public System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeResponse> singleChargeAsync(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.singleChargeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            return ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).singleChargeAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.dynamicCharge(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest request) {
            return base.Channel.dynamicCharge(request);
        }
        
        public long dynamicCharge(string username, string password, string domain, int channel, string mobilenum, string serviceId, long price) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            inValue.Body.price = price;
            ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse retVal = ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).dynamicCharge(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse> ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.dynamicChargeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest request) {
            return base.Channel.dynamicChargeAsync(request);
        }
        
        public System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeResponse> dynamicChargeAsync(string username, string password, string domain, int channel, string mobilenum, string serviceId, long price) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.dynamicChargeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            inValue.Body.price = price;
            return ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).dynamicChargeAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.sendVerificationCode(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest request) {
            return base.Channel.sendVerificationCode(request);
        }
        
        public long sendVerificationCode(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse retVal = ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).sendVerificationCode(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse> ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.sendVerificationCodeAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest request) {
            return base.Channel.sendVerificationCodeAsync(request);
        }
        
        public System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeResponse> sendVerificationCodeAsync(string username, string password, string domain, int channel, string mobilenum, string serviceId) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.sendVerificationCodeRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            return ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).sendVerificationCodeAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.verifySubscriber(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest request) {
            return base.Channel.verifySubscriber(request);
        }
        
        public long verifySubscriber(string username, string password, string domain, int channel, string mobilenum, string serviceId, string token) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            inValue.Body.token = token;
            ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse retVal = ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).verifySubscriber(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse> ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging.verifySubscriberAsync(ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest request) {
            return base.Channel.verifySubscriberAsync(request);
        }
        
        public System.Threading.Tasks.Task<ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberResponse> verifySubscriberAsync(string username, string password, string domain, int channel, string mobilenum, string serviceId, string token) {
            ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest inValue = new ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequest();
            inValue.Body = new ConsoleApp1.MobinOneMapfaChargingServiceReference.verifySubscriberRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.domain = domain;
            inValue.Body.channel = channel;
            inValue.Body.mobilenum = mobilenum;
            inValue.Body.serviceId = serviceId;
            inValue.Body.token = token;
            return ((ConsoleApp1.MobinOneMapfaChargingServiceReference.Charging)(this)).verifySubscriberAsync(inValue);
        }
    }
}
