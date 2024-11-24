// File generated automatically by ReswPlus. https://github.com/DotNetPlus/ReswPlus
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Data;

namespace BlueskyClient.Strings{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("DotNetPlus.ReswPlus", "2.1.3")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public static class Resources {
        private static ResourceLoader _resourceLoader;
        static Resources()
        {
            _resourceLoader = ResourceLoader.GetForViewIndependentUse("Resources");
        }

        #region AppPasswordHelpText
        /// <summary>
        ///   Looks up a localized string similar to: What's an App Password?
        /// </summary>
        public static string AppPasswordHelpText
        {
            get
            {
                return _resourceLoader.GetString("AppPasswordHelpText");
            }
        }
        #endregion

        #region IdentifierBoxPlaceholder
        /// <summary>
        ///   Looks up a localized string similar to: Bluesky email or username
        /// </summary>
        public static string IdentifierBoxPlaceholder
        {
            get
            {
                return _resourceLoader.GetString("IdentifierBoxPlaceholder");
            }
        }
        #endregion

        #region PasswordBoxPlaceholder
        /// <summary>
        ///   Looks up a localized string similar to: Your special App Password
        /// </summary>
        public static string PasswordBoxPlaceholder
        {
            get
            {
                return _resourceLoader.GetString("PasswordBoxPlaceholder");
            }
        }
        #endregion

        #region SignInText
        /// <summary>
        ///   Looks up a localized string similar to: Sign in
        /// </summary>
        public static string SignInText
        {
            get
            {
                return _resourceLoader.GetString("SignInText");
            }
        }
        #endregion
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("DotNetPlus.ReswPlus", "2.1.3")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class ResourcesExtension: MarkupExtension
    {
        public enum KeyEnum
        {
            __Undefined = 0,
            AppPasswordHelpText,
            IdentifierBoxPlaceholder,
            PasswordBoxPlaceholder,
            SignInText,
        }

        private static ResourceLoader _resourceLoader;
        static ResourcesExtension()
        {
            _resourceLoader = ResourceLoader.GetForViewIndependentUse("Resources");
        }
        public KeyEnum Key { get; set;}
        public IValueConverter Converter { get; set;}
        public object ConverterParameter { get; set;}
        protected override object ProvideValue()
        {
            string res;
            if(Key == KeyEnum.__Undefined)
            {
                res = "";
            }
            else
            {
                res = _resourceLoader.GetString(Key.ToString());
            }
            return Converter == null ? res : Converter.Convert(res, typeof(String), ConverterParameter, null);
        }
    }
} //BlueskyClient.Strings
