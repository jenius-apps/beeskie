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

        #region BackText
        /// <summary>
        ///   Looks up a localized string similar to: Back
        /// </summary>
        public static string BackText
        {
            get
            {
                return _resourceLoader.GetString("BackText");
            }
        }
        #endregion

        #region CancelText
        /// <summary>
        ///   Looks up a localized string similar to: Cancel
        /// </summary>
        public static string CancelText
        {
            get
            {
                return _resourceLoader.GetString("CancelText");
            }
        }
        #endregion

        #region CloseText
        /// <summary>
        ///   Looks up a localized string similar to: Close
        /// </summary>
        public static string CloseText
        {
            get
            {
                return _resourceLoader.GetString("CloseText");
            }
        }
        #endregion

        #region HomeText
        /// <summary>
        ///   Looks up a localized string similar to: Home
        /// </summary>
        public static string HomeText
        {
            get
            {
                return _resourceLoader.GetString("HomeText");
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

        #region ManageAccountText
        /// <summary>
        ///   Looks up a localized string similar to: Manage account
        /// </summary>
        public static string ManageAccountText
        {
            get
            {
                return _resourceLoader.GetString("ManageAccountText");
            }
        }
        #endregion

        #region NewPostText
        /// <summary>
        ///   Looks up a localized string similar to: New Post
        /// </summary>
        public static string NewPostText
        {
            get
            {
                return _resourceLoader.GetString("NewPostText");
            }
        }
        #endregion

        #region NotificationsText
        /// <summary>
        ///   Looks up a localized string similar to: Notifications
        /// </summary>
        public static string NotificationsText
        {
            get
            {
                return _resourceLoader.GetString("NotificationsText");
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

        #region ProfileText
        /// <summary>
        ///   Looks up a localized string similar to: Profile
        /// </summary>
        public static string ProfileText
        {
            get
            {
                return _resourceLoader.GetString("ProfileText");
            }
        }
        #endregion

        #region RepostCaption
        /// <summary>
        ///   Looks up a localized string similar to: Reposted by {0}
        /// </summary>
        public static string RepostCaption
        {
            get
            {
                return _resourceLoader.GetString("RepostCaption");
            }
        }
        #endregion

        #region SendFeedbackText
        /// <summary>
        ///   Looks up a localized string similar to: Send feedback
        /// </summary>
        public static string SendFeedbackText
        {
            get
            {
                return _resourceLoader.GetString("SendFeedbackText");
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

        #region SignOutText
        /// <summary>
        ///   Looks up a localized string similar to: Sign out
        /// </summary>
        public static string SignOutText
        {
            get
            {
                return _resourceLoader.GetString("SignOutText");
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
            BackText,
            CancelText,
            CloseText,
            HomeText,
            IdentifierBoxPlaceholder,
            ManageAccountText,
            NewPostText,
            NotificationsText,
            PasswordBoxPlaceholder,
            ProfileText,
            RepostCaption,
            SendFeedbackText,
            SignInText,
            SignOutText,
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
