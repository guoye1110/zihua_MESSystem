﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceReference8 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.ewininfo.com", ConfigurationName="ServiceReference8.CraftPramService")]
    public interface CraftPramService {
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        ServiceReference8.addCraftPramResponse addCraftPram(ServiceReference8.addCraftPram request);
        
        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        ServiceReference8.queryByDispatchNoResponse queryByDispatchNo(ServiceReference8.queryByDispatchNo request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1590.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ewininfo.com")]
    public partial class returnCraftPram : object, System.ComponentModel.INotifyPropertyChanged {
        
        private long idField;
        
        private bool idFieldSpecified;
        
        private string nameField;
        
        private string dispatchNoField;
        
        private double btmLmtField;
        
        private double topLmtField;
        
        private double defValField;
        
        private string unitField;
        
        private string actualValField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long Id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
                this.RaisePropertyChanged("Id");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IdSpecified {
            get {
                return this.idFieldSpecified;
            }
            set {
                this.idFieldSpecified = value;
                this.RaisePropertyChanged("IdSpecified");
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dispatchNo {
            get {
                return this.dispatchNoField;
            }
            set {
                this.dispatchNoField = value;
                this.RaisePropertyChanged("dispatchNo");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double btmLmt {
            get {
                return this.btmLmtField;
            }
            set {
                this.btmLmtField = value;
                this.RaisePropertyChanged("btmLmt");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double topLmt {
            get {
                return this.topLmtField;
            }
            set {
                this.topLmtField = value;
                this.RaisePropertyChanged("topLmt");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double defVal {
            get {
                return this.defValField;
            }
            set {
                this.defValField = value;
                this.RaisePropertyChanged("defVal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit {
            get {
                return this.unitField;
            }
            set {
                this.unitField = value;
                this.RaisePropertyChanged("unit");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string actualVal {
            get {
                return this.actualValField;
            }
            set {
                this.actualValField = value;
                this.RaisePropertyChanged("actualVal");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="addCraftPram", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class addCraftPram {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("data", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ServiceReference8.returnCraftPram[] data;
        
        public addCraftPram() {
        }
        
        public addCraftPram(ServiceReference8.returnCraftPram[] data) {
            this.data = data;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="addCraftPramResponse", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class addCraftPramResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string @return;
        
        public addCraftPramResponse() {
        }
        
        public addCraftPramResponse(string @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryByDispatchNo", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryByDispatchNo {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dispatchNo;
        
        public queryByDispatchNo() {
        }
        
        public queryByDispatchNo(string dispatchNo) {
            this.dispatchNo = dispatchNo;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="queryByDispatchNoResponse", WrapperNamespace="http://www.ewininfo.com", IsWrapped=true)]
    public partial class queryByDispatchNoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.ewininfo.com", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ServiceReference8.returnCraftPram[] @return;
        
        public queryByDispatchNoResponse() {
        }
        
        public queryByDispatchNoResponse(ServiceReference8.returnCraftPram[] @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CraftPramServiceChannel : ServiceReference8.CraftPramService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CraftPramServiceClient : System.ServiceModel.ClientBase<ServiceReference8.CraftPramService>, ServiceReference8.CraftPramService {
        
        public CraftPramServiceClient() {
        }
        
        public CraftPramServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CraftPramServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CraftPramServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CraftPramServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServiceReference8.addCraftPramResponse ServiceReference8.CraftPramService.addCraftPram(ServiceReference8.addCraftPram request) {
            return base.Channel.addCraftPram(request);
        }
        
        public string addCraftPram(ServiceReference8.returnCraftPram[] data) {
            ServiceReference8.addCraftPram inValue = new ServiceReference8.addCraftPram();
            inValue.data = data;
            ServiceReference8.addCraftPramResponse retVal = ((ServiceReference8.CraftPramService)(this)).addCraftPram(inValue);
            return retVal.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServiceReference8.queryByDispatchNoResponse ServiceReference8.CraftPramService.queryByDispatchNo(ServiceReference8.queryByDispatchNo request) {
            return base.Channel.queryByDispatchNo(request);
        }
        
        public ServiceReference8.returnCraftPram[] queryByDispatchNo(string dispatchNo) {
            ServiceReference8.queryByDispatchNo inValue = new ServiceReference8.queryByDispatchNo();
            inValue.dispatchNo = dispatchNo;
            ServiceReference8.queryByDispatchNoResponse retVal = ((ServiceReference8.CraftPramService)(this)).queryByDispatchNo(inValue);
            return retVal.@return;
        }
    }
}
