﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fizzi.Applications.Splitter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys> SplitKey {
            get {
                return ((global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys>)(this["SplitKey"]));
            }
            set {
                this["SplitKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys> UnsplitKey {
            get {
                return ((global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys>)(this["UnsplitKey"]));
            }
            set {
                this["UnsplitKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys> SkipKey {
            get {
                return ((global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys>)(this["SkipKey"]));
            }
            set {
                this["SkipKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys> ResetKey {
            get {
                return ((global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys>)(this["ResetKey"]));
            }
            set {
                this["ResetKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys> PauseKey {
            get {
                return ((global::System.Nullable<BondTech.HotKeyManagement.WPF._4.Keys>)(this["PauseKey"]));
            }
            set {
                this["PauseKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsNewVersion {
            get {
                return ((bool)(this["IsNewVersion"]));
            }
            set {
                this["IsNewVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AllowsTransparency {
            get {
                return ((bool)(this["AllowsTransparency"]));
            }
            set {
                this["AllowsTransparency"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ConfigPath {
            get {
                return ((string)(this["ConfigPath"]));
            }
            set {
                this["ConfigPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int MsDecimalCount {
            get {
                return ((int)(this["MsDecimalCount"]));
            }
            set {
                this["MsDecimalCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200")]
        public double HotkeyCooldownTime {
            get {
                return ((double)(this["HotkeyCooldownTime"]));
            }
            set {
                this["HotkeyCooldownTime"] = value;
            }
        }
    }
}
