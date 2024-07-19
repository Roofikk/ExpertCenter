using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExpertCenter.MvcApp.TagHelpers;

[HtmlTargetElement("label", Attributes = "required")]
public class SpanRequiredForLabel : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output.Attributes["required"].Value.ToString() == "true")
        {
            output.PostContent.AppendHtml(" <span class=\"text-muted\">*</span>");
        }
        else
        {
            output.PostContent.AppendHtml(" <span class=\"text-muted\" style=\"font-weight: normal; font-size: 0.8em\"> (не обязательно)</span>");
        }

        output.Attributes.RemoveAll("required");
        base.Process(context, output);
    }
}
