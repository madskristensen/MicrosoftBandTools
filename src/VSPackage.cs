using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MicrosoftBandTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid("67ca5344-cb3f-47f8-aca5-666f9b149cef")]
    public sealed class VSPackage : Package
    {
        protected override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name, Vsix.Version, "c04811f5-2c73-460b-996e-225918fa8ba0");
            base.Initialize();
        }
    }
}
