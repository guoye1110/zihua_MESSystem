﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceReference4 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.ewininfo.com", ConfigurationName="ServiceReference4.DeviceAndonService")]
    public interface DeviceAndonService {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        ServiceReference4.queryDeviceAndonStatusResponse queryDeviceAndonStatus(ServiceReference4.queryDeviceAndonStatus request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        ServiceReference4.disposedDeviceAndonResponse disposedDeviceAndon(ServiceReference4.disposedDeviceAndon request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        ServiceReference4.queryDeviceAndonInfoResponse queryDeviceAndonInfo(ServiceReference4.queryDeviceAndonInfo request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1590.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ewininfo.com")]
    public partial class deviceAndonStatus : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string noField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string no {
            get {
                return this.noField;
            }
            set {
                this.noField = value;
                this.RaisePropertyChanged("no");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("name");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1590.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ewininfo.com")]
    public partial class returnDeviceAndon : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string faultNoField;
        
        private string deviceNoField;
        
        private string deviceNameField;
        
        private string workshopField;
        
        private string faultDescField;
        
        private long tsUserField;
        
        private bool tsUserFieldSpecified;
        
        private System.DateTime timeField;
        
        private bool timeFieldSpecified;
        
        private string statusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string faultNo {
            get {
                return this.faultNoField;
            }
            set {
                this.faultNoField = value;
                this.RaisePropertyChanged("faultNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string deviceNo {
            get {
                return this.deviceNoField;
            }
            set {
                this.deviceNoField = value;
                this.RaisePropertyChanged("deviceNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string deviceName {
            get {
                return this.deviceNameField;
            }
            set {
                this.deviceNameField = value;
                this.RaisePropertyChanged("deviceName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string workshop {
            get {
                return this.workshopField;
            }
            set {
                this.workshopField = value;
                this.RaisePropertyChanged("workshop");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string faultDesc {
            get {
                return this.faultDescField;
            }
            set {
                this.faultDescField = value;
                this.RaisePropertyChanged("faultDesc");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long tsUser {
            get {
                return this.tsUserField;
            }
            set {
                this.tsUserField = value;
                this.RaisePropertyChanged("tsUser");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool tsUserSpecified {
            get {
                return this.tsUserFieldSpecified;
            }
            set {
                this.tsUserFieldSpecified = value;
                this.RaisePropertyChanged("tsUserSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time {
            get {
                return this.timeField;
            }
            set {
                this.timeField = value;
                this.RaisePropertyChanged("time");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool timeSpecified {
            get {
                return this.timeFieldSpecified;
            }
            set {
                this.timeFieldSpecified = value;
                this.RaisePropertyChanged("timeSpecified");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("status");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryDeviceAndonStatus", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryDeviceAndonStatus {
        
        public queryDeviceAndonStatus() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryDeviceAndonStatusResponse", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryDeviceAndonStatusResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ServiceReference4.deviceAndonStatus[] @return;
        
        public queryDeviceAndonStatusResponse() {
        }
        
        public queryDeviceAndonStatusResponse(ServiceReference4.deviceAndonStatus[] @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="disposedDeviceAndon", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class disposedDeviceAndon {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string userId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string faultNo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string status;
        
        public disposedDeviceAndon() {
        }
        
        public disposedDeviceAndon(string userId, string faultNo, string status) {
            this.userId = userId;
            this.faultNo = faultNo;
            this.status = status;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="disposedDeviceAndonResponse", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class disposedDeviceAndonResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string @return;
        
        public disposedDeviceAndonResponse() {
        }
        
        public disposedDeviceAndonResponse(string @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryDeviceAndonInfo", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryDeviceAndonInfo {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string deviceNo;
        
        public queryDeviceAndonInfo() {
        }
        
        public queryDeviceAndonInfo(string deviceNo) {
            this.deviceNo = deviceNo;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryDeviceAndonInfoResponse", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryDeviceAndonInfoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ServiceReference4.returnDeviceAndon[] @return;
        
        public queryDeviceAndonInfoResponse() {
        }
        
        public queryDeviceAndonInfoResponse(ServiceReference4.returnDeviceAndon[] @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DeviceAndonServiceChannel : ServiceReference4.DeviceAndonService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DeviceAndonServiceClient : System.ServiceModel.ClientBase<ServiceReference4.DeviceAndonService>, ServiceReference4.DeviceAndonService {
        
        public DeviceAndonServiceClient() {
        }
        
        public DeviceAndonServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DeviceAndonServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeviceAndonServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeviceAndonServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServiceReference4.queryDeviceAndonStatusResponse ServiceReference4.DeviceAndonService.queryDeviceAndonStatus(ServiceReference4.queryDeviceAndonStatus request) {
            return base.Channel.queryDeviceAndonStatus(request);
        }
        
        public ServiceReference4.deviceAndonStatus[] queryDeviceAndonStatus() {
            ServiceReference4.queryDeviceAndonStatus inValue = new ServiceReference4.queryDeviceAndonStatus();
            ServiceReference4.queryDeviceAndonStatusResponse retVal = ((ServiceReference4.DeviceAndonService)(this)).queryDeviceAndonStatus(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServiceReference4.disposedDeviceAndonResponse ServiceReference4.DeviceAndonService.disposedDeviceAndon(ServiceReference4.disposedDeviceAndon request) {
            return base.Channel.disposedDeviceAndon(request);
        }
        
        public string disposedDeviceAndon(string userId, string faultNo, string status) {
            ServiceReference4.disposedDeviceAndon inValue = new ServiceReference4.disposedDeviceAndon();
            inValue.userId = userId;
            inValue.faultNo = faultNo;
            inValue.status = status;
            ServiceReference4.disposedDeviceAndonResponse retVal = ((ServiceReference4.DeviceAndonService)(this)).disposedDeviceAndon(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServiceReference4.queryDeviceAndonInfoResponse ServiceReference4.DeviceAndonService.queryDeviceAndonInfo(ServiceReference4.queryDeviceAndonInfo request) {
            return base.Channel.queryDeviceAndonInfo(request);
        }
        
        public ServiceReference4.returnDeviceAndon[] queryDeviceAndonInfo(string deviceNo) {
            ServiceReference4.queryDeviceAndonInfo inValue = new ServiceReference4.queryDeviceAndonInfo();
            inValue.deviceNo = deviceNo;
            ServiceReference4.queryDeviceAndonInfoResponse retVal = ((ServiceReference4.DeviceAndonService)(this)).queryDeviceAndonInfo(inValue);
            return retVal.@return;
        }
    }
}
