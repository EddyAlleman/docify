﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xarial.Docify.Lib.Plugins.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xarial.Docify.Lib.Plugins.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to pre.code-snippet {
        ///    border: none;
        ///    overflow: auto;
        ///    background: #eff0f1;
        ///}
        ///
        ///pre.jagged-top {
        ///    background-color: #eff0f1;
        ///    background-image: linear-gradient(135deg, rgba(255,255,255,1) 0%, rgba(255,255,255,1) 50%, rgba(255,255,255,0) 50%, rgba(255,255,255,0) 100%), linear-gradient(-135deg, rgba(255,255,255,1) 0%, rgba(255,255,255,1) 50%, rgba(255,255,255,0) 50%, rgba(255,255,255,0) 100%), linear-gradient(to top, #eff0f1 0%, #eff0f1 100%);
        ///    background-position: top center;
        ///    back [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string code_snippet {
            get {
                return ResourceManager.GetString("code_snippet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;figure&gt;
        ///   &lt;a href=&quot;{1}&quot; imageanchor=&quot;1&quot;&gt;
        ///   {0}
        ///   &lt;figcaption&gt;{2}&lt;/figcaption&gt;
        ///   &lt;/a&gt;
        ///&lt;/figure&gt;.
        /// </summary>
        internal static string img_figure {
            get {
                return ResourceManager.GetString("img_figure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .responsive {
        ///    height: auto;
        ///    max-height: 100%;
        ///    max-width: 100%;
        ///    display: block;
        ///    margin-left: auto;
        ///    margin-right: auto;
        ///}
        ///
        ///figure {
        ///    text-align: center;
        ///}.
        /// </summary>
        internal static string responsive_image {
            get {
                return ResourceManager.GetString("responsive_image", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Byte[].
        /// </summary>
        internal static byte[] tipue_search {
            get {
                object obj = ResourceManager.GetObject("tipue_search", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
