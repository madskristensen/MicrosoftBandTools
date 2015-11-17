using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Core.Validation;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;

namespace MicrosoftBandTools
{
    [Export(typeof(IJSONItemValidator))]
    [Name("Variable Name Validator")]
    class VariableNameValidator : IJSONItemValidator
    {
        public IEnumerable<Type> ItemTypes
        {
            get { return new[] { typeof(JSONMember) }; }
        }

        public JSONItemValidationResult ValidateItem(JSONParseItem item, IJSONValidationContext context)
        {
            JSONMember member = item as JSONMember;

            if (member == null || !member.UnquotedValueText.Contains("{{"))
                return JSONItemValidationResult.Continue;

            var variables = GetVariables(member);
            var regex = new Regex("{{(?<name>[^}{]+)}}", RegexOptions.Compiled);

            foreach (Match match in regex.Matches(member.UnquotedValueText))
            {
                Group group = match.Groups["name"];
                string name = group.Value;

                if (!variables.Contains(name))
                {
                    JsonErrorTag error = new JsonErrorTag
                    {
                        Flags = JSONErrorFlags.ErrorListError | JSONErrorFlags.UnderlineRed,
                        Item = member.Value,
                        Start = member.Value.Start + group.Index + 1, // 1 is for the quotation mark
                        AfterEnd = member.Value.Start + group.Index + group.Length,
                        Length = group.Length,
                        Text = $"The variable \"{name}\" has not been declared"
                    };

                    Telemetry.TrackEvent("Icon no declared");
                    context.AddError(error);
                }
            }

            return JSONItemValidationResult.Continue;
        }

        public static IEnumerable<string> GetVariables(JSONParseItem item)
        {
            var visitor = new JSONItemCollector<JSONMember>(true);

            if (!item.JSONDocument.Accept(visitor))
                return null;

            var contents = visitor.Items.Where(m => m.UnquotedNameText == "content" && m.IsValid);
            List<string> list = new List<string>();

            foreach (JSONMember content in contents)
            {
                var value = content?.Value as JSONObject;

                if (value == null)
                    continue;

                var names = value.Children.OfType<JSONMember>().Select(s => s.UnquotedNameText);

                list.AddRange(names.Where(n => !list.Contains(n)));
            }

            return list;
        }
    }
}
