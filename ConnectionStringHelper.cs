using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigSecurity
{
    public static class ConnectionStringHelper
    {
        public static void ProtectConnectionStrings(string pathName)
        {
            ToggleProtectConnectionStrings(pathName, true);
        }

        public static void UnprotectConnectionStrings(string pathName)
        {
            ToggleProtectConnectionStrings(pathName, false);
        }

        public static void ProtectApplicationSettings(string pathName)
        {
            ToggleProtectApplicationSettings(pathName, true);
        }

        public static void UnprotectApplicationSettings(string pathName)
        {
            ToggleProtectApplicationSettings(pathName, false);
        }

        private static void ToggleProtectConnectionStrings(string pathName, bool protect)
        {
            // Define the Dpapi provider name.
            string strProvider = "DataProtectionConfigurationProvider";
            // string strProvider = "RSAProtectedConfigurationProvider";

            Configuration config = null;

            // For Web!
            // oConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

            // For Windows!
            // Takes the executable file name without the config extension.
            config = ConfigurationManager.OpenExeConfiguration(pathName);

            if (config != null)
            {
                var configSection = config.GetSection("connectionStrings") as ConnectionStringsSection;
                if (configSection != null)
                {
                    var hasChanged = false;

                    if ((!(configSection.ElementInformation.IsLocked)) && (!(configSection.SectionInformation.IsLocked)))
                    {
                        if (protect)
                        {
                            // Encrypt the section.
                            if (!(configSection.SectionInformation.IsProtected))
                            {
                                hasChanged = true;
                                configSection.SectionInformation.ProtectSection(strProvider);
                            }
                        }
                        else
                        {
                            // Remove encryption.
                            if (configSection.SectionInformation.IsProtected)
                            {
                                hasChanged = true;
                                configSection.SectionInformation.UnprotectSection();
                            }
                        }
                    }

                    if (hasChanged)
                    {
                        // Save current configuration.
                        configSection.SectionInformation.ForceSave = true;
                        config.Save();
                    }
                }
            }
        }

        private static void ToggleProtectApplicationSettings(string pathName, bool protect)
        {
            // Define the Dpapi provider name.
            string strProvider = "DataProtectionConfigurationProvider";
            // string strProvider = "RSAProtectedConfigurationProvider";

            Configuration oConfiguration = null;

            // For Web!
            // oConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

            // For Windows!
            // Takes the executable file name without the config extension.
            oConfiguration = ConfigurationManager.OpenExeConfiguration(pathName);

            if (oConfiguration != null)
            {
                // Retrieve the applicationSettings section.
                ConfigurationSectionGroup applicationSettings = oConfiguration.GetSectionGroup("applicationSettings");
                if (applicationSettings != null && applicationSettings.Sections != null)
                {
                    ConfigurationSectionCollection configSections = applicationSettings.Sections;
                    foreach (ConfigurationSection configSection in configSections)
                    {
                        var changed = false;

                        if ((!(configSection.ElementInformation.IsLocked)) && (!(configSection.SectionInformation.IsLocked)))
                        {
                            if (protect)
                            {
                                // Encrypt the section.
                                if (!(configSection.SectionInformation.IsProtected))
                                {
                                    changed = true;
                                    configSection.SectionInformation.ProtectSection(strProvider);
                                }
                            }
                            else
                            {
                                // Remove encryption.
                                if (configSection.SectionInformation.IsProtected)
                                {
                                    changed = true;
                                    configSection.SectionInformation.UnprotectSection();
                                }
                            }
                        }

                        if (changed)
                        {
                            // Save current configuration.
                            configSection.SectionInformation.ForceSave = true;
                            oConfiguration.Save();
                        }
                    }

                }
            }
        }

    }
}
