// <copyright file="BaseLightHooks.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.LightLink
{
    using System.Collections.Generic;
    using System.Linq;
    using Sandbox.ModAPI;
    using VRage.Game.Components;
    using VRage.Game.ModAPI;
    using VRage.ModAPI;
    using VRage.ObjectBuilders;

    /// <summary>
    /// Base handling class for common behaviour across MyObjectBuilder_ types.
    /// </summary>
    public class BaseHooks : MyGameLogicComponent
    {
        private IMyConnector typedEntity = null;
        private bool attached = false;
        private StorageHandler sHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHooks"/> class.
        /// </summary>
        public BaseHooks()
        {
            ConnectorControlsHelper.AttachControls();
        }


        /// <inheritdoc/>
        public override void Close()
        {
            this.DetachEvents();
        }

        /// <inheritdoc/>
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            this.sHandler = new StorageHandler(this.Entity);
        }



        private void TargetBlock_OnMarkForClose(IMyEntity obj)
        {
            this.DetachEvents();
        }

        private void AttachEvents()
        {

        }

        private void DetachEvents()
        {

        }

    }
}
