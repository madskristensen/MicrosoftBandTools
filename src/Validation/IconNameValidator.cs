using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Core.Validation;
using Microsoft.VisualStudio.Utilities;

namespace MicrosoftBandTools
{
    [Export(typeof(IJSONItemValidator))]
    [Name("Icon Name Validator")]
    class IconNameValidator : IJSONItemValidator
    {
        public IEnumerable<Type> ItemTypes
        {
            get { return new[] { typeof(JSONMember) }; }
        }

        public JSONItemValidationResult ValidateItem(JSONParseItem item, IJSONValidationContext context)
        {
            JSONMember member = item as JSONMember;

            if (member != null && member.UnquotedNameText == "icon")
            {
                var icons = IconCompletionProvider.GetIcons(member);

                if (icons != null && !icons.ContainsKey(member.UnquotedValueText))
                {
                    JsonErrorTag error = new JsonErrorTag
                    {
                        Flags = JSONErrorFlags.ErrorListError | JSONErrorFlags.UnderlineRed,
                        Item = member.Value,
                        Start = member.Value.Start,
                        AfterEnd = member.Value.AfterEnd,
                        Length = member.Value.Length,
                        Text = $"The icon \"{member.UnquotedValueText}\" has not been declared"
                    };

                    context.AddError(error);
                }
            }

            return JSONItemValidationResult.Continue;
        }
    }
}
