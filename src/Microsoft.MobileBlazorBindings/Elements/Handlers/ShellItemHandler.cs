// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using XF = Xamarin.Forms;

namespace Microsoft.MobileBlazorBindings.Elements.Handlers
{
    public partial class ShellItemHandler : ShellGroupItemHandler
    {
        public override void AddChild(XF.Element child, int physicalSiblingIndex)
        {
            if (child is null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            switch (child)
            {
                case XF.TemplatedPage childAsTemplatedPage:
                    ShellItemControl.Items.Add(childAsTemplatedPage); // Implicit conversion
                    break;
                case XF.ShellContent childAsShellContent:
                    ShellItemControl.Items.Add(childAsShellContent); // Implicit conversion
                    break;
                case XF.ShellSection childAsShellSection:
                    ShellItemControl.Items.Add(childAsShellSection);
                    break;
                default:
                    throw new NotSupportedException($"Handler of type '{GetType().FullName}' representing element type '{TargetElement?.GetType().FullName ?? "<null>"}' doesn't support adding a child (child type is '{child.GetType().FullName}').");
            }
        }

        public override void SetParent(XF.Element parent)
        {
            if (ElementControl.Parent == null)
            {
                // The Parent should already be set
                throw new InvalidOperationException("Shouldn't need to set parent here...");
            }
        }
    }
}
