//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------
// <auto-generatedInfo>
// 	This code was generated by ResW File Code Generator (http://reswcodegen.codeplex.com)
// 	ResW File Code Generator was written by Christian Resma Helle
// 	and is under GNU General Public License version 2 (GPLv2)
// 
// 	This code contains a helper class exposing property representations
// 	of the string resources defined in the specified .ResW file
// 
// 	Generated: 02/21/2015 19:15:33
// </auto-generatedInfo>
// --------------------------------------------------------------------------------------------------
namespace Inoreader.Strings
{
    using Windows.ApplicationModel.Resources;
    
    
    public partial class Resources
    {
        
        private static ResourceLoader resourceLoader;
        
        static Resources()
        {
            string executingAssemblyName;
            executingAssemblyName = Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName;
            string[] executingAssemblySplit;
            executingAssemblySplit = executingAssemblyName.Split(',');
            executingAssemblyName = executingAssemblySplit[1];
            string currentAssemblyName;
            currentAssemblyName = typeof(Resources).AssemblyQualifiedName;
            string[] currentAssemblySplit;
            currentAssemblySplit = currentAssemblyName.Split(',');
            currentAssemblyName = currentAssemblySplit[1];
            if (executingAssemblyName.Equals(currentAssemblyName))
            {
                resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            }
            else
            {
                resourceLoader = ResourceLoader.GetForCurrentView(currentAssemblyName + "/Resources");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Free client for RSS aggregator www.inoreader.com"
        /// </summary>
        public static string AppDescription
        {
            get
            {
                return resourceLoader.GetString("AppDescription");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Inoreader Free"
        /// </summary>
        public static string AppDisplayName
        {
            get
            {
                return resourceLoader.GetString("AppDisplayName");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Error"
        /// </summary>
        public static string ErrorDialogTitle
        {
            get
            {
                return resourceLoader.GetString("ErrorDialogTitle");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Nothing to share"
        /// </summary>
        public static string ErrorShareMessage
        {
            get
            {
                return resourceLoader.GetString("ErrorShareMessage");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "bytes"
        /// </summary>
        public static string FileSizeBytes
        {
            get
            {
                return resourceLoader.GetString("FileSizeBytes");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "newest first"
        /// </summary>
        public static string NewestFirstShowOrder
        {
            get
            {
                return resourceLoader.GetString("NewestFirstShowOrder");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "oldest first"
        /// </summary>
        public static string OldestFirstShowOrder
        {
            get
            {
                return resourceLoader.GetString("OldestFirstShowOrder");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Read all"
        /// </summary>
        public static string ReadAllSubscriptionItem
        {
            get
            {
                return resourceLoader.GetString("ReadAllSubscriptionItem");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "system language"
        /// </summary>
        public static string SettingsSystemLanguage
        {
            get
            {
                return resourceLoader.GetString("SettingsSystemLanguage");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Subscriptions"
        /// </summary>
        public static string SubscriptionsSectionHeader
        {
            get
            {
                return resourceLoader.GetString("SubscriptionsSectionHeader");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "expanded view"
        /// </summary>
        public static string StreamViewExpanded
        {
            get
            {
                return resourceLoader.GetString("StreamViewExpanded");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "list view"
        /// </summary>
        public static string StreamViewList
        {
            get
            {
                return resourceLoader.GetString("StreamViewList");
            }
        }
    }
}
