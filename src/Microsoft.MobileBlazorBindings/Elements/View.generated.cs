// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.MobileBlazorBindings.Core;
using Microsoft.MobileBlazorBindings.Elements.Handlers;
using System.Threading.Tasks;
using XF = Xamarin.Forms;

namespace Microsoft.MobileBlazorBindings.Elements
{
    public partial class View : VisualElement
    {

        [Parameter] public XF.LayoutOptions? HorizontalOptions { get; set; }
        [Parameter] public XF.Thickness? Margin { get; set; }
        [Parameter] public XF.LayoutOptions? VerticalOptions { get; set; }

        public new XF.View NativeControl => ((ViewHandler)ElementHandler).ViewControl;

        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            if (HorizontalOptions != null)
            {
                builder.AddAttribute(nameof(HorizontalOptions), AttributeHelper.LayoutOptionsToString(HorizontalOptions.Value));
            }
            if (Margin != null)
            {
                builder.AddAttribute(nameof(Margin), AttributeHelper.ThicknessToString(Margin.Value));
            }
            if (VerticalOptions != null)
            {
                builder.AddAttribute(nameof(VerticalOptions), AttributeHelper.LayoutOptionsToString(VerticalOptions.Value));
            }

            RenderAdditionalAttributes(builder);
        }

        partial void RenderAdditionalAttributes(AttributesBuilder builder);
    }
}
