// <copyright file="BaseHooks.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System.Linq;
    using Sandbox.ModAPI.Ingame;
    using VRage.Game.Components;
    using VRage.Game.ModAPI;
    using VRage.ModAPI;
    using VRage.ObjectBuilders;

    /// <summary>
    /// Base handling class for common behaviour across MyObjectBuilder_ types.
    /// </summary>
    public class BaseHooks : MyGameLogicComponent
    {
        private bool attached = false;
        private StorageHandler sHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHooks"/> class.
        /// </summary>
        public BaseHooks()
        {
            ConnectorControlsHelper.AttachControls();
        }

        private Sandbox.ModAPI.IMyShipConnector TypedEntity => (Sandbox.ModAPI.IMyShipConnector)this.Entity;

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
            this.AttachEvents();
        }

        /// <inheritdoc/>
        public override void UpdateOnceBeforeFrame()
        {
            this.sHandler = new StorageHandler(this.Entity);
            this.TypedEntity_IsWorkingChanged(null);
            base.UpdateOnceBeforeFrame();
        }

        private void TargetBlock_OnMarkForClose(IMyEntity obj)
        {
            this.DetachEvents();
        }

        private void AttachEvents()
        {
            this.TypedEntity.IsWorkingChanged += this.TypedEntity_IsWorkingChanged;
        }

        private void TypedEntity_IsWorkingChanged(IMyCubeBlock obj)
        {
            if (this.TypedEntity.Status == MyShipConnectorStatus.Connected)
            {
                this.HandlePotentialNewConnection();
            }
            else
            {
                this.HandlePotentialDisconnection();
            }
        }

        private void DetachEvents()
        {
            this.TypedEntity.IsWorkingChanged -= this.TypedEntity_IsWorkingChanged;
        }

        private void HandlePotentialNewConnection()
        {
            if (this.sHandler.AutoSwitch)
            {
                var source = this.TypedEntity.CubeGrid;
                bool shouldRecharge = true;

                if (this.sHandler.StaticOnly)
                {
                    if (!this.TypedEntity.OtherConnector.CubeGrid.IsStatic)
                    {
                        Logging.Debug("Ignoring connection, non-static grid");
                        shouldRecharge = false;
                    }
                }

                if (shouldRecharge)
                {
                    var batteries = source.GetFatBlocks<Sandbox.ModAPI.IMyBatteryBlock>();
                    foreach (var battery in batteries)
                    {
                        battery.ChargeMode = ChargeMode.Recharge;
                    }

                    if (this.sHandler.ThrustersIncluded)
                    {
                        var thrusters = source.GetFatBlocks<Sandbox.ModAPI.IMyThrust>();
                        Logging.Debug("Attempting to disable " + thrusters.Count() + " thrusters.");
                        foreach (var thrust in thrusters)
                        {
                            thrust.Enabled = false;
                        }
                    }
                }
            }
        }

        private void HandlePotentialDisconnection()
        {
            if (this.sHandler.AutoSwitch)
            {
                var batteries = this.TypedEntity.CubeGrid.GetFatBlocks<Sandbox.ModAPI.IMyBatteryBlock>();
                foreach (var battery in batteries)
                {
                    battery.ChargeMode = ChargeMode.Auto;
                }

                if (this.sHandler.ThrustersIncluded)
                {
                    var thrusters = this.TypedEntity.CubeGrid.GetFatBlocks<Sandbox.ModAPI.IMyThrust>();
                    Logging.Debug("Attempting to enable " + thrusters.Count() + " thrusters.");
                    foreach (var thrust in thrusters)
                    {
                        thrust.Enabled = true;
                    }
                }
            }
        }
    }
}
