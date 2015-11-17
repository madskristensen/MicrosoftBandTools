using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.JSON.Core.Schema;

namespace MicrosoftBandTools
{
    [Export(typeof(IJSONSchemaSelector))]
    class SchemaSelector : IJSONSchemaSelector
    {
        private const string URL = "http://json.schemastore.org/band-manifest";

        public async Task<IEnumerable<string>> GetAvailableSchemasAsync()
        {
            return await Task.FromResult(new[] { URL });
        }

        public string GetSchemaFor(string fileLocation)
        {
            if (IsBandManifest(fileLocation))
            {
                Telemetry.TrackEvent("Schema applied");
                return URL;
            }

            return null;
        }

        private static bool IsBandManifest(string file)
        {
            string fileName = Path.GetFileName(file);

            if (string.IsNullOrEmpty(fileName) || !fileName.Equals("manifest.json", StringComparison.OrdinalIgnoreCase))
                return false;

            if (File.Exists(file))
            {
                string content = File.ReadAllText(file);

                if (content.Contains("\"manifestVersion\"") ||
                    content.Contains("\"versionString\"") ||
                    content.Contains("\"tileIcon\""))
                {
                    return true;
                }
            }

            return false;
        }

        public event EventHandler AvailableSchemasChanged
        {
            add { }
            remove { }
        }
    }
}
