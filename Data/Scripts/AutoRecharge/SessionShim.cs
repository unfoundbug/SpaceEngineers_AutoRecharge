// <copyright file="SessionShim.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using UnFoundBug.AutoSwitch;
    using VRage.Game.Components;

    /// <summary>
    /// Session Based instance.
    /// </summary>
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation | MyUpdateOrder.AfterSimulation)]
    public class SessionShim : MySessionComponentBase
    {
        /// <summary>
        /// Instance for session static container.
        /// </summary>
        public static SessionShim Instance;

        private readonly StorageCache sCache = new StorageCache();

        /// <summary>
        /// Gets the storage cache.
        /// </summary>
        public StorageCache Cache => this.sCache;

        /// <inheritdoc/>
        public override void LoadData()
        {
            Instance = this;
        }

        /// <inheritdoc/>
        public override void BeforeStart()
        {
            base.BeforeStart();
            ConnectorControlsHelper.AttachControls();
        }

        /// <inheritdoc/>
        protected override void UnloadData()
        {
            this.sCache.Dispose();
            Instance = null;
        }
    }
}
