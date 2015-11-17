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

            var visitor = new JSONItemCollector<JSONMember>(true);

            if (!member.JSONDocument.Accept(visitor))
                yield break;

            var icons = visitor.Items.FirstOrDefault(m => m.UnquotedNameText == "icons");
            var value = icons?.Value as JSONObject;

            if (value == null)
                yield break;

            string folder = Path.GetDirectoryName(member.JSONDocument.DocumentLocation);

            foreach (JSONMember icon in value.Children.OfType<JSONMember>())
            {
                string path = Path.Combine(folder, icon.UnquotedValueText);

                if (File.Exists(path))
                {
                    var image = BitmapFrame.Create(new Uri(path, UriKind.Absolute));
                    yield return new SimpleCompletionEntry(icon.UnquotedNameText, icon.Name.Text, image, context.Session);
                }
                else
                {
                    yield return new SimpleCompletionEntry(icon.UnquotedNameText, icon.Name.Text, icon.Value.Text, context.Session);
                }
            }
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
