using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Core.Validation;
using Microsoft.VisualStudio.Utilities;

namespace MicrosoftBandTools
{
    [Export(typeof(IJSONItemValidator))]
    [Name("Icon File Validator")]
    class IconFileExistValidator : IJSONItemValidator
    {
        public IEnumerable<Type> ItemTypes
        {
            get { return new[] { typeof(JSONMember) }; }
        }

        public JSONItemValidationResult ValidateItem(JSONParseItem item, IJSONValidationContext context)
        {
            JSONMember member = item as JSONMember;
            JSONMember icons = item?.Parent?.FindType<JSONMember>();

            if (icons != null && icons.UnquotedNameText == "icons")
            {
                string folder = Path.GetDirectoryName(item.JSONDocument.DocumentLocation);
                string file = Path.Combine(folder, member.UnquotedValueText);

                if (!File.Exists(file))
                {
                    JsonErrorTag error = new JsonErrorTag
                    {
                        Flags = JSONErrorFlags.ErrorListError | JSONErrorFlags.UnderlineRed,
                        Item = member.Value,
                        Start = member.Value.Start,
                        AfterEnd = member.Value.AfterEnd,
                        Length = member.Value.Length,
                        Text = $"The file \"{member.UnquotedValueText}\" does not exist"
                    };

                    context.AddError(error);
                }
            }

            return JSONItemValidationResult.Continue;
        }
    }
}
