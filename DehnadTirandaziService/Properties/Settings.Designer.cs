﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DehnadTirandaziService.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PardisImi")]
        public string AggregatorName {
            get {
                return ((string)(this["AggregatorName"]));
            }
            set {
                this["AggregatorName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("03:15:00")]
        public string InsertAutochargeMessageInQueueTime {
            get {
                return ((string)(this["InsertAutochargeMessageInQueueTime"]));
            }
            set {
                this["InsertAutochargeMessageInQueueTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("03:15:15")]
        public string InsertAutochargeMessageInQueueEndTime {
            get {
                return ((string)(this["InsertAutochargeMessageInQueueEndTime"]));
            }
            set {
                this["InsertAutochargeMessageInQueueEndTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int NumberOfAutochargeMessagesPerDay {
            get {
                return ((int)(this["NumberOfAutochargeMessagesPerDay"]));
            }
            set {
                this["NumberOfAutochargeMessagesPerDay"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int ReadSize {
            get {
                return ((int)(this["ReadSize"]));
            }
            set {
                this["ReadSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int Take {
            get {
                return ((int)(this["Take"]));
            }
            set {
                this["Take"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Tirandazi")]
        public string ServiceCode {
            get {
                return ((string)(this["ServiceCode"]));
            }
            set {
                this["ServiceCode"] = value;
            }
        }
    }
}
