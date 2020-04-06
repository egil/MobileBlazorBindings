// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.MobileBlazorBindings.Core;
using Microsoft.MobileBlazorBindings.Elements.Handlers;
using System.Threading.Tasks;
using XF = Xamarin.Forms;

namespace Microsoft.MobileBlazorBindings.Elements
{
    public partial class TabbedPage : Page
    {
        static TabbedPage()
        {
            ElementHandlerRegistry.RegisterElementHandler<TabbedPage>(
                renderer => new TabbedPageHandler(renderer, new XF.TabbedPage()));
        }

        [Parameter] public XF.Color? BarBackgroundColor { get; set; }
        [Parameter] public XF.Color? BarTextColor { get; set; }
        [Parameter] public XF.Color? SelectedTabColor { get; set; }
        [Parameter] public XF.Color? UnselectedTabColor { get; set; }

        public new XF.TabbedPage NativeControl => ((TabbedPageHandler)ElementHandler).TabbedPageControl;

        protected override void RenderAttributes(AttributesBuilder builder)
        {
            base.RenderAttributes(builder);

            if (BarBackgroundColor != null)
            {
                builder.AddAttribute(nameof(BarBackgroundColor), AttributeHelper.ColorToString(BarBackgroundColor.Value));
            }
            if (BarTextColor != null)
            {
                builder.AddAttribute(nameof(BarTextColor), AttributeHelper.ColorToString(BarTextColor.Value));
            }
            if (SelectedTabColor != null)
            {
                builder.AddAttribute(nameof(SelectedTabColor), AttributeHelper.ColorToString(SelectedTabColor.Value));
            }
            if (UnselectedTabColor != null)
            {
                builder.AddAttribute(nameof(UnselectedTabColor), AttributeHelper.ColorToString(UnselectedTabColor.Value));
            }

            RenderAdditionalAttributes(builder);
        }

        partial void RenderAdditionalAttributes(AttributesBuilder builder);
    }
}
