using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ph.TagHelpers
{
    public class LikeTagHelper : TagHelper
    {
        public bool Liked { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "i";
            
            if (Liked)
            {
                output.Attributes.SetAttribute("class", "fa fa-heart liketag");       
            }
            else
            {
                output.Attributes.SetAttribute("class", "fa fa-heart-o liketag");
            }
        }
    }
}
