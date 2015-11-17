using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace MicrosoftBandTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Version, IconResourceID = 400)]
    [Guid(PackageGuidString)]
    public sealed class VSPackage : Package
    {
        public const string PackageGuidString = "67ca5344-cb3f-47f8-aca5-666f9b149cef";
        public const string Version = "1.0";

        protected override void Initialize()
        {
            base.Initialize();
        }
    }
}
