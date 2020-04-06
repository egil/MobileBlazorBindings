// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.MobileBlazorBindings.Core
{
    public abstract class NativeComponentRenderer : Renderer
    {
        private readonly Dictionary<int, NativeComponentAdapter> _componentIdToAdapter = new Dictionary<int, NativeComponentAdapter>();
        private ElementManager _elementManager;
        private readonly Dictionary<ulong, Action> _eventRegistrations = new Dictionary<ulong, Action>();


        public NativeComponentRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
        }

        protected abstract ElementManager CreateNativeControlManager();

        internal ElementManager ElementManager
        {
            get
            {
                return _elementManager ?? (_elementManager = CreateNativeControlManager());
            }
        }

        public override Dispatcher Dispatcher { get; }
             = Dispatcher.CreateDefault();

        /// <summary>
        /// Creates a component of type <typeparamref name="TComponent"/> and adds it as a child of <paramref name="parent"/>.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Task AddComponent<TComponent>(IElementHandler parent) where TComponent : IComponent 
            => AddComponent(typeof(TComponent), parent, ParameterView.Empty);

        /// <summary>
        /// Creates a component of type <paramref name="componentType"/> and adds it as a child of <paramref name="parent"/>.
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Task AddComponent(Type componentType, IElementHandler parent)
            => AddComponent(componentType, parent, ParameterView.Empty);
        
        /// <summary>
        /// Creates a component of type <paramref name="componentType"/> and adds it as a child 
        /// of <paramref name="parent"/>.
        /// </summary>
        /// <param name="componentType">The type of component to create.</param>
        /// <param name="parent">The parent to add the component to.</param>
        /// <param name="initialParameters">The <see cref="ParameterView"/> with the initial parameters.</param>
        /// <returns>A tuple of (<see cref="int"/> ComponentId, <see cref="IComponent"/> Component).</returns>
        protected async Task<(int ComponentId, IComponent Component)> AddComponent(Type componentType, IElementHandler parent, ParameterView initialParameters)
        {
            var result = await Dispatcher.InvokeAsync(() =>
            {
                var component = InstantiateComponent(componentType);
                var componentId = AssignRootComponentId(component);

                var rootAdapter = new NativeComponentAdapter(this, closestPhysicalParent: parent, knownTargetElement: parent)
                {
                    Name = $"RootAdapter attached to {parent.GetType().FullName}",
                };

                _componentIdToAdapter[componentId] = rootAdapter;

                return (componentId, component);
            }).ConfigureAwait(false);

            await RenderRootComponentAsync(result.componentId, initialParameters).ConfigureAwait(false);

            return result;
        }

        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            foreach (var updatedComponent in renderBatch.UpdatedComponents.Array.Take(renderBatch.UpdatedComponents.Count))
            {
                var adapter = _componentIdToAdapter[updatedComponent.ComponentId];
                adapter.ApplyEdits(updatedComponent.ComponentId, updatedComponent.Edits, renderBatch.ReferenceFrames, renderBatch);
            }

            var numDisposedComponents = renderBatch.DisposedComponentIDs.Count;
            for (var i = 0; i < numDisposedComponents; i++)
            {
                var disposedComponentId = renderBatch.DisposedComponentIDs.Array[i];
                if (_componentIdToAdapter.TryGetValue(disposedComponentId, out var adapter))
                {
                    _componentIdToAdapter.Remove(disposedComponentId);
                    (adapter as IDisposable)?.Dispose();
                }
            }

            var numDisposeEventHandlers = renderBatch.DisposedEventHandlerIDs.Count;
            if (numDisposeEventHandlers != 0)
            {
                for (var i = 0; i < numDisposeEventHandlers; i++)
                {
                    DisposeEvent(renderBatch.DisposedEventHandlerIDs.Array[i]);
                }
            }

            return Task.CompletedTask;
        }

        public void RegisterEvent(ulong eventHandlerId, Action unregisterCallback)
        {
            if (eventHandlerId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventHandlerId), "Event handler ID must not be 0.");
            }
            if (unregisterCallback == null)
            {
                throw new ArgumentNullException(nameof(unregisterCallback));
            }
            _eventRegistrations.Add(eventHandlerId, unregisterCallback);
        }

        private void DisposeEvent(ulong eventHandlerId)
        {
            if (!_eventRegistrations.TryGetValue(eventHandlerId, out var unregisterCallback))
            {
                throw new InvalidOperationException($"Attempting to dispose unknown event handler id '{eventHandlerId}'.");
            }
            unregisterCallback();
        }

        internal NativeComponentAdapter CreateAdapterForChildComponent(IElementHandler physicalParent, int componentId)
        {
            var result = new NativeComponentAdapter(this, physicalParent);
            _componentIdToAdapter[componentId] = result;
            return result;
        }
    }
}
