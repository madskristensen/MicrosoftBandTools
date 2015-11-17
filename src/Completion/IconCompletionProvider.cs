using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Utilities;

namespace MicrosoftBandTools
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("Icon Completion Provider")]
    class IconCompletionProvider : IJSONCompletionListProvider
    {
        public JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyValue; }
        }

        public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            if (!IsBandManifest(context.ContextItem))
                yield break;

            JSONMember member = context.ContextItem as JSONMember;

            if (member == null || member.UnquotedNameText != "icon")
                yield break;

            string folder = Path.GetDirectoryName(member.JSONDocument.DocumentLocation);
            Telemetry.TrackEvent("Icon completion");

            foreach (var icon in GetIcons(member))
            {
                string path = Path.Combine(folder, icon.Value);

                if (File.Exists(path))
                {
                    var image = BitmapFrame.Create(new Uri(path, UriKind.Absolute));
                    yield return new SimpleCompletionEntry(icon.Key, "\"" + icon.Key + "\"", image, context.Session);
                }
                else
                {
                    yield return new SimpleCompletionEntry(icon.Key, "\"" + icon.Key + "\"", "\"" + icon.Value + "\"", context.Session);
                }
            }
        }

        public static IDictionary<string, string> GetIcons(JSONParseItem item)
        {
            var visitor = new JSONItemCollector<JSONMember>(true);
            var dic = new Dictionary<string, string>();

            if (!item.JSONDocument.Accept(visitor))
                return null;

            var icons = visitor.Items.FirstOrDefault(m => m.UnquotedNameText == "icons");
            var value = icons?.Value as JSONObject;

            if (value == null)
                return null;

            string folder = Path.GetDirectoryName(item.JSONDocument.DocumentLocation);

            foreach (JSONMember icon in value.Children.OfType<JSONMember>())
            {
                dic.Add(icon.UnquotedNameText, icon.UnquotedValueText);
            }

            return dic;
        }

        private static bool IsBandManifest(JSONParseItem item)
        {
            string fileName = Path.GetFileName(item.JSONDocument?.DocumentLocation);

            if (string.IsNullOrEmpty(fileName) || !fileName.Equals("manifest.json", StringComparison.OrdinalIgnoreCase))
                return false;

            if (item.JSONDocument.Text.Contains("\"manifestVersion\"") ||
                item.JSONDocument.Text.Contains("\"versionString\"") ||
                item.JSONDocument.Text.Contains("\"tileIcon\""))
                return true;

            return false;
        }
    }
}
