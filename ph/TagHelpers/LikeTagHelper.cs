using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ph.TagHelpers
{
    public class LikeTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "i";
            output.Attributes.SetAttribute("class", "fa fa-heart-o");
        }
    }
}