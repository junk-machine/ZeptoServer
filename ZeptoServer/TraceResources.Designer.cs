﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeptoServer {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TraceResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TraceResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZeptoServer.TraceResources", typeof(TraceResources).Assembly);
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
        ///   Looks up a localized string similar to Data processing complete, start receiving.
        /// </summary>
        internal static string AccumulatedDataProcessed {
            get {
                return ResourceManager.GetString("AccumulatedDataProcessed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Client connected.
        /// </summary>
        internal static string ClientConnected {
            get {
                return ResourceManager.GetString("ClientConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection closed.
        /// </summary>
        internal static string ConnectionClosed {
            get {
                return ResourceManager.GetString("ConnectionClosed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Received {0} bytes, closing connection.
        /// </summary>
        internal static string ConnectionClosingOnReceiveFormat {
            get {
                return ResourceManager.GetString("ConnectionClosingOnReceiveFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server error: {0}.
        /// </summary>
        internal static string CriticalServerErrorFormat {
            get {
                return ResourceManager.GetString("CriticalServerErrorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error receiving data: {0}.
        /// </summary>
        internal static string ErrorReceivingDataFormat {
            get {
                return ResourceManager.GetString("ErrorReceivingDataFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &gt; {0}.
        /// </summary>
        internal static string ReceivedDataFormat {
            get {
                return ResourceManager.GetString("ReceivedDataFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: .
        /// </summary>
        internal static string ScopeNameFormat {
            get {
                return ResourceManager.GetString("ScopeNameFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt; {0}.
        /// </summary>
        internal static string SentDataFormat {
            get {
                return ResourceManager.GetString("SentDataFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server is running!.
        /// </summary>
        internal static string ServerStarted {
            get {
                return ResourceManager.GetString("ServerStarted", resourceCulture);
            }
        }
    }
}
