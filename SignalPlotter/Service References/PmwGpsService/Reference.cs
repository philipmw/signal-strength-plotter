﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SignalPlotter.PmwGpsService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="LatestGpsData", Namespace="http://schemas.datacontract.org/2004/07/GpsService.Model")]
    [System.SerializableAttribute()]
    public partial struct LatestGpsData : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private DotSpatial.Positioning.Position3D positionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ushort satellitesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private DotSpatial.Positioning.Speed speed5secField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime timeField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public DotSpatial.Positioning.Position3D position {
            get {
                return this.positionField;
            }
            set {
                if ((this.positionField.Equals(value) != true)) {
                    this.positionField = value;
                    this.RaisePropertyChanged("position");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ushort satellites {
            get {
                return this.satellitesField;
            }
            set {
                if ((this.satellitesField.Equals(value) != true)) {
                    this.satellitesField = value;
                    this.RaisePropertyChanged("satellites");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public DotSpatial.Positioning.Speed speed5sec {
            get {
                return this.speed5secField;
            }
            set {
                if ((this.speed5secField.Equals(value) != true)) {
                    this.speed5secField = value;
                    this.RaisePropertyChanged("speed5sec");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime time {
            get {
                return this.timeField;
            }
            set {
                if ((this.timeField.Equals(value) != true)) {
                    this.timeField = value;
                    this.RaisePropertyChanged("time");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PmwGpsService.IGpsServiceContract")]
    public interface IGpsServiceContract {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/IsConnected", ReplyAction="http://tempuri.org/IGpsServiceContract/IsConnectedResponse")]
        bool IsConnected();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/IsConnected", ReplyAction="http://tempuri.org/IGpsServiceContract/IsConnectedResponse")]
        System.Threading.Tasks.Task<bool> IsConnectedAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GetLatest", ReplyAction="http://tempuri.org/IGpsServiceContract/GetLatestResponse")]
        SignalPlotter.PmwGpsService.LatestGpsData GetLatest();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GetLatest", ReplyAction="http://tempuri.org/IGpsServiceContract/GetLatestResponse")]
        System.Threading.Tasks.Task<SignalPlotter.PmwGpsService.LatestGpsData> GetLatestAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GpsTime", ReplyAction="http://tempuri.org/IGpsServiceContract/GpsTimeResponse")]
        System.DateTime GpsTime();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GpsTime", ReplyAction="http://tempuri.org/IGpsServiceContract/GpsTimeResponse")]
        System.Threading.Tasks.Task<System.DateTime> GpsTimeAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GpsPosition", ReplyAction="http://tempuri.org/IGpsServiceContract/GpsPositionResponse")]
        DotSpatial.Positioning.Position GpsPosition();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/GpsPosition", ReplyAction="http://tempuri.org/IGpsServiceContract/GpsPositionResponse")]
        System.Threading.Tasks.Task<DotSpatial.Positioning.Position> GpsPositionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Elevation", ReplyAction="http://tempuri.org/IGpsServiceContract/ElevationResponse")]
        DotSpatial.Positioning.Distance Elevation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Elevation", ReplyAction="http://tempuri.org/IGpsServiceContract/ElevationResponse")]
        System.Threading.Tasks.Task<DotSpatial.Positioning.Distance> ElevationAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Speed5Sec", ReplyAction="http://tempuri.org/IGpsServiceContract/Speed5SecResponse")]
        DotSpatial.Positioning.Speed Speed5Sec();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Speed5Sec", ReplyAction="http://tempuri.org/IGpsServiceContract/Speed5SecResponse")]
        System.Threading.Tasks.Task<DotSpatial.Positioning.Speed> Speed5SecAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Satellites", ReplyAction="http://tempuri.org/IGpsServiceContract/SatellitesResponse")]
        ushort Satellites();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IGpsServiceContract/Satellites", ReplyAction="http://tempuri.org/IGpsServiceContract/SatellitesResponse")]
        System.Threading.Tasks.Task<ushort> SatellitesAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IGpsServiceContractChannel : SignalPlotter.PmwGpsService.IGpsServiceContract, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GpsServiceContractClient : System.ServiceModel.ClientBase<SignalPlotter.PmwGpsService.IGpsServiceContract>, SignalPlotter.PmwGpsService.IGpsServiceContract {
        
        public GpsServiceContractClient() {
        }
        
        public GpsServiceContractClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public GpsServiceContractClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsServiceContractClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public GpsServiceContractClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool IsConnected() {
            return base.Channel.IsConnected();
        }
        
        public System.Threading.Tasks.Task<bool> IsConnectedAsync() {
            return base.Channel.IsConnectedAsync();
        }
        
        public SignalPlotter.PmwGpsService.LatestGpsData GetLatest() {
            return base.Channel.GetLatest();
        }
        
        public System.Threading.Tasks.Task<SignalPlotter.PmwGpsService.LatestGpsData> GetLatestAsync() {
            return base.Channel.GetLatestAsync();
        }
        
        public System.DateTime GpsTime() {
            return base.Channel.GpsTime();
        }
        
        public System.Threading.Tasks.Task<System.DateTime> GpsTimeAsync() {
            return base.Channel.GpsTimeAsync();
        }
        
        public DotSpatial.Positioning.Position GpsPosition() {
            return base.Channel.GpsPosition();
        }
        
        public System.Threading.Tasks.Task<DotSpatial.Positioning.Position> GpsPositionAsync() {
            return base.Channel.GpsPositionAsync();
        }
        
        public DotSpatial.Positioning.Distance Elevation() {
            return base.Channel.Elevation();
        }
        
        public System.Threading.Tasks.Task<DotSpatial.Positioning.Distance> ElevationAsync() {
            return base.Channel.ElevationAsync();
        }
        
        public DotSpatial.Positioning.Speed Speed5Sec() {
            return base.Channel.Speed5Sec();
        }
        
        public System.Threading.Tasks.Task<DotSpatial.Positioning.Speed> Speed5SecAsync() {
            return base.Channel.Speed5SecAsync();
        }
        
        public ushort Satellites() {
            return base.Channel.Satellites();
        }
        
        public System.Threading.Tasks.Task<ushort> SatellitesAsync() {
            return base.Channel.SatellitesAsync();
        }
    }
}
