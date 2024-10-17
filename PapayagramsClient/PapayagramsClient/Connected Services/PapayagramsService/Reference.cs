﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PapayagramsClient.PapayagramsService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Message", Namespace="http://schemas.datacontract.org/2004/07/Contracts")]
    [System.SerializableAttribute()]
    public partial class Message : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AuthorUsernameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GameRoomCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime TimeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AuthorUsername {
            get {
                return this.AuthorUsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.AuthorUsernameField, value) != true)) {
                    this.AuthorUsernameField = value;
                    this.RaisePropertyChanged("AuthorUsername");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GameRoomCode {
            get {
                return this.GameRoomCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.GameRoomCodeField, value) != true)) {
                    this.GameRoomCodeField = value;
                    this.RaisePropertyChanged("GameRoomCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Time {
            get {
                return this.TimeField;
            }
            set {
                if ((this.TimeField.Equals(value) != true)) {
                    this.TimeField = value;
                    this.RaisePropertyChanged("Time");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PapayagramsService.IPregameService", CallbackContract=typeof(PapayagramsClient.PapayagramsService.IPregameServiceCallback))]
    public interface IPregameService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/CreateGame")]
        void CreateGame(string username);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/CreateGame")]
        System.Threading.Tasks.Task CreateGameAsync(string username);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/JoinGame")]
        void JoinGame(string username, string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/JoinGame")]
        System.Threading.Tasks.Task JoinGameAsync(string username, string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPregameService/LeaveGame", ReplyAction="http://tempuri.org/IPregameService/LeaveGameResponse")]
        int LeaveGame(string username, string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPregameService/LeaveGame", ReplyAction="http://tempuri.org/IPregameService/LeaveGameResponse")]
        System.Threading.Tasks.Task<int> LeaveGameAsync(string username, string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/SendMessage")]
        void SendMessage(PapayagramsClient.PapayagramsService.Message message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/SendMessage")]
        System.Threading.Tasks.Task SendMessageAsync(PapayagramsClient.PapayagramsService.Message message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPregameServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/JoinGameResponse")]
        void JoinGameResponse(string roomCode);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IPregameService/ReceiveMessage")]
        void ReceiveMessage(PapayagramsClient.PapayagramsService.Message message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPregameServiceChannel : PapayagramsClient.PapayagramsService.IPregameService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PregameServiceClient : System.ServiceModel.DuplexClientBase<PapayagramsClient.PapayagramsService.IPregameService>, PapayagramsClient.PapayagramsService.IPregameService {
        
        public PregameServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public PregameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public PregameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PregameServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public PregameServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void CreateGame(string username) {
            base.Channel.CreateGame(username);
        }
        
        public System.Threading.Tasks.Task CreateGameAsync(string username) {
            return base.Channel.CreateGameAsync(username);
        }
        
        public void JoinGame(string username, string roomCode) {
            base.Channel.JoinGame(username, roomCode);
        }
        
        public System.Threading.Tasks.Task JoinGameAsync(string username, string roomCode) {
            return base.Channel.JoinGameAsync(username, roomCode);
        }
        
        public int LeaveGame(string username, string roomCode) {
            return base.Channel.LeaveGame(username, roomCode);
        }
        
        public System.Threading.Tasks.Task<int> LeaveGameAsync(string username, string roomCode) {
            return base.Channel.LeaveGameAsync(username, roomCode);
        }
        
        public void SendMessage(PapayagramsClient.PapayagramsService.Message message) {
            base.Channel.SendMessage(message);
        }
        
        public System.Threading.Tasks.Task SendMessageAsync(PapayagramsClient.PapayagramsService.Message message) {
            return base.Channel.SendMessageAsync(message);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PapayagramsService.ILoginService")]
    public interface ILoginService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILoginService/Login", ReplyAction="http://tempuri.org/ILoginService/LoginResponse")]
        int Login(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILoginService/Login", ReplyAction="http://tempuri.org/ILoginService/LoginResponse")]
        System.Threading.Tasks.Task<int> LoginAsync(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILoginService/Logout", ReplyAction="http://tempuri.org/ILoginService/LogoutResponse")]
        int Logout(string username);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILoginService/Logout", ReplyAction="http://tempuri.org/ILoginService/LogoutResponse")]
        System.Threading.Tasks.Task<int> LogoutAsync(string username);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILoginServiceChannel : PapayagramsClient.PapayagramsService.ILoginService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LoginServiceClient : System.ServiceModel.ClientBase<PapayagramsClient.PapayagramsService.ILoginService>, PapayagramsClient.PapayagramsService.ILoginService {
        
        public LoginServiceClient() {
        }
        
        public LoginServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LoginServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int Login(string username, string password) {
            return base.Channel.Login(username, password);
        }
        
        public System.Threading.Tasks.Task<int> LoginAsync(string username, string password) {
            return base.Channel.LoginAsync(username, password);
        }
        
        public int Logout(string username) {
            return base.Channel.Logout(username);
        }
        
        public System.Threading.Tasks.Task<int> LogoutAsync(string username) {
            return base.Channel.LogoutAsync(username);
        }
    }
}