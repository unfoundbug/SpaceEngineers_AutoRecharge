// <copyright file="BaseHooks.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System;
    using System.Linq;
    using System.Text;
    using Sandbox.Game.Entities;
    using Sandbox.Game.EntityComponents;
    using Sandbox.ModAPI.Ingame;
    using VRage.Game.Components;
    using VRage.Game.ModAPI;
    using VRage.Game.ModAPI.Network;
    using VRage.ModAPI;
    using VRage.Network;
    using VRage.ObjectBuilders;
    using VRage.Sync;
    using VRage.Utils;

    /// <summary>
    /// Base handling class for common behaviour across MyObjectBuilder_ types.
    /// </summary>
    public class BaseHooks : MyGameLogicComponent, IMyEventProxy
    {
        private static readonly Guid StorageGuid = new Guid("{D249368C-3008-4BA0-934E-90F66A146061}");
        private bool responsibleForUpdates = true;
        private bool valueChanged = false;
        private bool? wasConnected = null;

        private MySync<bool, SyncDirection.BothWays> syncAutoSwitch = null;
        private MySync<bool, SyncDirection.BothWays> syncStaticOnly = null;
        private MySync<ThrusterMode, SyncDirection.BothWays> syncThrusters = null;
        private MySync<TankMode, SyncDirection.BothWays> syncTankMode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHooks"/> class.
        /// </summary>
        public BaseHooks()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector auto-switches relevant blocks.
        /// </summary>
        public bool SwitchingEnabled
        {
            get
            {
                return this.syncAutoSwitch.Value;
            }

            set
            {
                if (this.syncAutoSwitch.Value != value)
                {
                    this.syncAutoSwitch.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector should only switch to recharge when connected to a static grid.
        /// </summary>
        public bool StaticOnly
        {
            get
            {
                return this.syncStaticOnly.Value;
            }

            set
            {
                if (this.syncStaticOnly.Value != value)
                {
                    this.syncStaticOnly.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector should also enable/disable thrusters.
        /// </summary>
        public ThrusterMode ThrustMode
        {
            get
            {
                return this.syncThrusters.Value;
            }

            set
            {
                if (this.syncThrusters.Value != value)
                {
                    this.syncThrusters.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value representing which tanks should be changed to refill when connected to another grid.
        /// </summary>
        public TankMode TankSetting
        {
            get
            {
                return this.syncTankMode.Value;
            }

            set
            {
                if (this.syncTankMode.Value != value)
                {
                    this.syncTankMode.Value = value;
                }
            }
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
            this.AttachEvents();
            this.Deserialise();
            SessionShim.Instance.AttemptControlsInit();
            this.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME | MyEntityUpdateEnum.EACH_FRAME;
        }

        /// <inheritdoc/>
        public override bool IsSerialized()
        {
            return this.valueChanged | base.IsSerialized();
        }

        /// <inheritdoc/>
        public override void UpdateOnceBeforeFrame()
        {
            this.ProcessConnector();
            base.UpdateOnceBeforeFrame();
        }

        /// <inheritdoc/>
        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();
            this.ProcessConnector();
        }

        private void AttachEvents()
        {
            this.syncTankMode.ValueChanged += this.SyncTankMode_ValueChanged;
            this.syncAutoSwitch.ValueChanged += this.SyncAutoSwitch_ValueChanged;
            this.syncStaticOnly.ValueChanged += this.SyncStaticOnly_ValueChanged;
            this.syncThrusters.ValueChanged += this.SyncThrusters_ValueChanged;
        }

        private void SyncThrusters_ValueChanged(MySync<ThrusterMode, SyncDirection.BothWays> obj)
        {
            MyLog.Default.WriteLineAndConsole($"AutoRecharge SyncThrusters_ValueChanged: {this.Entity.EntityId} is now {obj.Value}");
            this.ProcessConnector();
            this.Serialise();
        }

        private void SyncStaticOnly_ValueChanged(MySync<bool, SyncDirection.BothWays> obj)
        {
            MyLog.Default.WriteLineAndConsole($"AutoRecharge SyncStaticOnly_ValueChanged: {this.Entity.EntityId} is now {obj.Value}");
            this.ProcessConnector();
            this.Serialise();
        }

        private void SyncAutoSwitch_ValueChanged(MySync<bool, SyncDirection.BothWays> obj)
        {
            MyLog.Default.WriteLineAndConsole($"AutoRecharge SyncAutoSwitch_ValueChanged: {this.Entity.EntityId} is now {obj.Value}");
            this.ProcessConnector();
            this.Serialise();
        }

        private void SyncTankMode_ValueChanged(MySync<TankMode, SyncDirection.BothWays> obj)
        {
            MyLog.Default.WriteLineAndConsole($"AutoRecharge SyncTankMode_ValueChanged: {this.Entity.EntityId} is now {obj.Value}");
            this.ProcessConnector();
            this.Serialise();
        }

        private void ProcessConnector()
        {
            if (this.wasConnected.HasValue)
            {
                if (this.wasConnected.Value)
                {
                    if (this.TypedEntity.Status != MyShipConnectorStatus.Connected)
                    {
                        this.HandleDisconnection();
                        this.wasConnected = false;
                    }
                }
                else
                {
                    if (this.TypedEntity.Status == MyShipConnectorStatus.Connected)
                    {
                        this.HandleConnection();
                        this.wasConnected = true;
                    }
                }
            }
            else
            {
                if (this.TypedEntity.Status == MyShipConnectorStatus.Connected)
                {
                    this.HandleConnection();
                    this.wasConnected = true;
                }
                else
                {
                    this.HandleDisconnection();
                    this.wasConnected = false;
                }
            }
        }

        private void DetachEvents()
        {
            this.syncTankMode.ValueChanged += this.SyncTankMode_ValueChanged;
            this.syncAutoSwitch.ValueChanged += this.SyncAutoSwitch_ValueChanged;
            this.syncStaticOnly.ValueChanged += this.SyncStaticOnly_ValueChanged;
            this.syncThrusters.ValueChanged += this.SyncThrusters_ValueChanged;
        }

        private void HandleConnection()
        {
            if (!this.responsibleForUpdates)
            {
                return;
            }

            if (this.SwitchingEnabled)
            {
                MyLog.Default.WriteLineAndConsole($"AutoRecharge SwitchMode for {this.Entity.EntityId} running.");

                var source = this.TypedEntity.CubeGrid;
                bool shouldRecharge = true;

                if (this.StaticOnly)
                {
                    if (!this.TypedEntity.OtherConnector.CubeGrid.IsStatic)
                    {
                        MyLog.Default.WriteLineAndConsole($"  Skipping, connected grid is not static.");
                        shouldRecharge = false;
                    }
                }

                if (shouldRecharge)
                {
                    var batteries = source.GetFatBlocks<Sandbox.ModAPI.IMyBatteryBlock>();
                    MyLog.Default.WriteLineAndConsole($"  switching {batteries.Count()} batteries to recharge");
                    foreach (var battery in batteries)
                    {
                        battery.ChargeMode = ChargeMode.Recharge;
                    }

                    if (this.ThrustMode != ThrusterMode.None)
                    {
                        var thrusters = source.GetFatBlocks<Sandbox.ModAPI.IMyThrust>();
                        MyLog.Default.WriteLineAndConsole($"  Attempting to manage {thrusters.Count()} thrusters.");
                        foreach (var thrust in thrusters)
                        {
                            var castThrust = (MyThrust)thrust;
                            bool isH2 = castThrust.FuelConverterDefinition.FuelId.SubtypeId == "Hydrogen";

                            if (this.ThrustMode == ThrusterMode.ElectricOnly && isH2)
                            {
                                continue;
                            }

                            if (this.ThrustMode == ThrusterMode.HydrogenOnly && !isH2)
                            {
                                continue;
                            }

                            thrust.Enabled = false;
                        }
                    }

                    if (this.TankSetting != TankMode.None)
                    {
                        var tanks = source.GetFatBlocks<Sandbox.ModAPI.IMyGasTank>();
                        MyLog.Default.WriteLineAndConsole($"  Attempting to manage {tanks.Count()} tanks.");
                        foreach (var tank in tanks)
                        {
                            if (tank.BlockDefinition.SubtypeId.Contains("Hydro")
                            &&
                            this.TankSetting != TankMode.OxygenOnly)
                            {
                                tank.Stockpile = true;
                            }

                            if (!tank.BlockDefinition.SubtypeId.Contains("Hydro")
                                &&
                                this.TankSetting != TankMode.HydrogenOnly)
                            {
                                tank.Stockpile = true;
                            }
                        }
                    }
                }

                MyLog.Default.WriteLineAndConsole($"  Done");
            }
        }

        private void HandleDisconnection()
        {
            if (!this.responsibleForUpdates)
            {
                return;
            }

            if (this.SwitchingEnabled)
            {
                var batteries = this.TypedEntity.CubeGrid.GetFatBlocks<Sandbox.ModAPI.IMyBatteryBlock>();
                foreach (var battery in batteries)
                {
                    battery.ChargeMode = ChargeMode.Auto;
                }

                if (this.ThrustMode != ThrusterMode.None)
                {
                    var thrusters = this.TypedEntity.CubeGrid.GetFatBlocks<Sandbox.ModAPI.IMyThrust>();
                    Logging.Debug("Attempting to manage " + thrusters.Count() + " thrusters.");
                    foreach (var thrust in thrusters)
                    {
                        var castThrust = (MyThrust)thrust;
                        bool isH2 = castThrust.FuelConverterDefinition.FuelId.SubtypeId == "Hydrogen";

                        if (this.ThrustMode == ThrusterMode.ElectricOnly && isH2)
                        {
                            continue;
                        }

                        if (this.ThrustMode == ThrusterMode.HydrogenOnly && !isH2)
                        {
                            continue;
                        }

                        thrust.Enabled = true;
                    }
                }

                if (this.TankSetting != TankMode.None)
                {
                    var tanks = this.TypedEntity.CubeGrid.GetFatBlocks<Sandbox.ModAPI.IMyGasTank>();
                    Logging.Debug("Attempting to manage " + tanks.Count() + " tanks.");
                    foreach (var tank in tanks)
                    {
                        if (tank.BlockDefinition.SubtypeId.Contains("Hydro")
                            &&
                            this.TankSetting != TankMode.OxygenOnly)
                        {
                            tank.Stockpile = false;
                        }

                        if (!tank.BlockDefinition.SubtypeId.Contains("Hydro")
                            &&
                            this.TankSetting != TankMode.HydrogenOnly)
                        {
                            tank.Stockpile = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserialises the entity mod storage into current properties.
        /// </summary>
        private void Deserialise()
        {
            if (this.Entity.Storage != null)
            {
                if (this.Entity.Storage.ContainsKey(StorageGuid))
                {
                    string dataSource = this.Entity.Storage.GetValue(StorageGuid);
                    string[] components = dataSource.Split(',');
                    int versionId = int.Parse(components[0]);
                    switch (versionId)
                    {
                        case 1:
                            {
                                this.SwitchingEnabled = bool.Parse(components[1]);
                                this.StaticOnly = bool.Parse(components[2]);
                                break;
                            }

                        case 2:
                            {
                                this.SwitchingEnabled = bool.Parse(components[1]);
                                this.StaticOnly = bool.Parse(components[2]);
                                if (bool.Parse(components[3]))
                                {
                                    this.ThrustMode = ThrusterMode.None;
                                }
                                else
                                {
                                    this.ThrustMode = ThrusterMode.ElectricOnly;
                                }

                                break;
                            }

                        case 3:
                            {
                                this.SwitchingEnabled = bool.Parse(components[1]);
                                this.StaticOnly = bool.Parse(components[2]);
                                this.ThrustMode = (ThrusterMode)Enum.Parse(typeof(ThrusterMode), components[3]);
                                break;
                            }

                        case 4:
                            {
                                this.SwitchingEnabled = bool.Parse(components[1]);
                                this.StaticOnly = bool.Parse(components[2]);
                                this.ThrustMode = (ThrusterMode)Enum.Parse(typeof(ThrusterMode), components[3]);
                                this.TankSetting = (TankMode)Enum.Parse(typeof(TankMode), components[4]);
                                break;
                            }
                    }
                }
            }
        }

        private void Serialise()
        {
            this.valueChanged = true;

            StringBuilder sb = new StringBuilder();
            sb.Append("4,");
            sb.Append(this.SwitchingEnabled.ToString());
            sb.Append(",");
            sb.Append(this.StaticOnly);
            sb.Append(",");
            sb.Append(this.ThrustMode);
            sb.Append(",");
            sb.Append(this.TankSetting);

            if (this.Entity.Storage == null)
            {
                this.Entity.Storage = new MyModStorageComponent();
            }

            this.Entity.Storage.SetValue(StorageGuid, sb.ToString());
            MyLog.Default.WriteLineAndConsole($"AutoRecharge Entity {this.Entity.EntityId} saved.");
        }
    }
}
