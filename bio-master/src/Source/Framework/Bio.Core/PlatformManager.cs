using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Bio.Platform.Helpers;

namespace Bio
{
    /// <summary>
    /// Platform manager - this holds all the platform specific services.
    /// </summary>
    public static class PlatformManager
    {
        static readonly Lazy<PlatformServices> services = 
            new Lazy<PlatformServices>(() => new PlatformServices()); 

        /// <summary>
        /// Platform services (specific to platform)
        /// </summary>
        public static PlatformServices Services
        {
            get
            {
                return services.Value;
            }
        }
    }
}
