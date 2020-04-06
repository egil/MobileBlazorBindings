// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.MobileBlazorBindings.Core;
using System;
using XF = Xamarin.Forms;

namespace Microsoft.MobileBlazorBindings.Elements.Handlers
{
    public partial class BoxViewHandler : ViewHandler
    {
        public BoxViewHandler(NativeComponentRenderer renderer, XF.BoxView boxViewControl) : base(renderer, boxViewControl)
        {
            BoxViewControl = boxViewControl ?? throw new ArgumentNullException(nameof(boxViewControl));

            Initialize(renderer);
        }

        partial void Initialize(NativeComponentRenderer renderer);

        public XF.BoxView BoxViewControl { get; }

        public override void ApplyAttribute(ulong attributeEventHandlerId, string attributeName, object attributeValue, string attributeEventUpdatesAttributeName)
        {
            switch (attributeName)
            {
                case nameof(XF.BoxView.Color):
                    BoxViewControl.Color = AttributeHelper.StringToColor((string)attributeValue);
                    break;
                case nameof(XF.BoxView.CornerRadius):
                    BoxViewControl.CornerRadius = AttributeHelper.StringToCornerRadius(attributeValue);
                    break;
                default:
                    base.ApplyAttribute(attributeEventHandlerId, attributeName, attributeValue, attributeEventUpdatesAttributeName);
                    break;
            }
        }
    }
}
