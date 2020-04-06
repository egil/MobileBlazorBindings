// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.MobileBlazorBindings.Core;
using Microsoft.MobileBlazorBindings.Elements.Handlers;
using System.Threading.Tasks;
using XF = Xamarin.Forms;

namespace Microsoft.MobileBlazorBindings.Elements
{
    public partial class MenuItem : BaseMenuItem
    {
        static MenuItem()
        {
            ElementHandlerRegistry.RegisterElementHandler<MenuItem>(
                renderer => new MenuItemHandler(renderer, new XF.MenuItem()));
        }

        [Parameter] public XF.ImageSource IconImageSource { get; set; }
        [Parameter] public bool? IsDestructive { get; set; }
        [Parameter] public bool? IsEnabled { get; set; }
        [Parameter] public string Text { get; set; }

        public new XF.MenuItem NativeControl => ((MenuItemHandler)ElementHandler).MenuItemControl;

        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            if (IconImageSource != null)
            {
                builder.AddAttribute(nameof(IconImageSource), AttributeHelper.ImageSourceToString(IconImageSource));
            }
            if (IsDestructive != null)
            {
                builder.AddAttribute(nameof(IsDestructive), IsDestructive.Value);
            }
            if (IsEnabled != null)
            {
                builder.AddAttribute(nameof(IsEnabled), IsEnabled.Value);
            }
            if (Text != null)
            {
                builder.AddAttribute(nameof(Text), Text);
            }

            RenderAdditionalAttributes(builder);
        }

        partial void RenderAdditionalAttributes(AttributesBuilder builder);
    }
}
