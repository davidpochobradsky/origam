﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Origam.Licensing {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Origam.Licensing.strings", typeof(strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your license is valid for a different computer. Please log in and a new license will be generated for you..
        /// </summary>
        internal static string ComputerUUIDMismatch_HowToResolve {
            get {
                return ResourceManager.GetString("ComputerUUIDMismatch_HowToResolve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to License is valid for another computer..
        /// </summary>
        internal static string ComputerUUIDMismatch_Message {
            get {
                return ResourceManager.GetString("ComputerUUIDMismatch_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your license is expired. Please log-in and then a new licence will be automatically generated for you..
        /// </summary>
        internal static string LicenseExpired_HowToResolve {
            get {
                return ResourceManager.GetString("LicenseExpired_HowToResolve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to License signature validation error.
        /// </summary>
        internal static string LicenseExpired_Message {
            get {
                return ResourceManager.GetString("LicenseExpired_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The license signature and data does not match. This usually happens when a license file is corrupted or has been altered..
        /// </summary>
        internal static string SignatureValidationError_HowToResolve {
            get {
                return ResourceManager.GetString("SignatureValidationError_HowToResolve", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to License signature validation error.
        /// </summary>
        internal static string SignatureValidationError_Message {
            get {
                return ResourceManager.GetString("SignatureValidationError_Message", resourceCulture);
            }
        }
    }
}
