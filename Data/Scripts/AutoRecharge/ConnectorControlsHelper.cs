// <copyright file="ConnectorControlsHelper.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System.Linq;
    using Sandbox.ModAPI;
    using Sandbox.ModAPI.Interfaces.Terminal;
    using VRage.ModAPI;
    using VRage.Utils;

    /// <summary>
    /// Static container of helper functions.
    /// </summary>
    public static class ConnectorControlsHelper
    {
        private static IMyTerminalControlSeparator separator;
        private static IMyTerminalControlOnOffSwitch chargeOnConnectToggle;
        private static IMyTerminalControlOnOffSwitch staticOnlyToggle;
        private static IMyTerminalControlListbox thrusterControl;
        private static IMyTerminalControlListbox tankControl;

        /// <summary>
        /// Attach controls to terminal menus.
        /// </summary>
        public static void AttachControls()
        {
            separator = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyShipConnector>("autoswitch_seperator");
            separator.Enabled = (lb) => true;
            separator.Visible = block => !block.CubeGrid.IsStatic;

            Logging.Debug("Seperator initialised");

            chargeOnConnectToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyShipConnector>("autoswitch_autoswitch");
            chargeOnConnectToggle.Title = MyStringId.GetOrCompute("Auto Recharge");
            chargeOnConnectToggle.OnText = MyStringId.GetOrCompute("Enabled");
            chargeOnConnectToggle.OffText = MyStringId.GetOrCompute("Disabled");
            chargeOnConnectToggle.Tooltip = MyStringId.GetOrCompute("When enabled, connecting will cause ALL bateries on this grid to change to recharge");
            chargeOnConnectToggle.Getter = block =>
            {
                if (block == null || block.GameLogic == null)
                {
                    return false;
                }

                var logic = block.GameLogic.GetAs<BaseHooks>();
                return logic?.SwitchingEnabled ?? false;
            };
            chargeOnConnectToggle.Setter = (block, value) =>
            {
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    logic.SwitchingEnabled = value;
                }
            };
            chargeOnConnectToggle.Visible = block => !block.CubeGrid.IsStatic;
            Logging.Debug("SubGridOnOff initialised");

            staticOnlyToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyShipConnector>("autoswitch_scanSubGrid");
            staticOnlyToggle.Title = MyStringId.GetOrCompute("Draw From Static Only");
            staticOnlyToggle.Tooltip = MyStringId.GetOrCompute("Batteries will only change to recharge if the other grid is a station.");
            staticOnlyToggle.OnText = MyStringId.GetOrCompute("Enabled");
            staticOnlyToggle.OffText = MyStringId.GetOrCompute("Disabled");
            staticOnlyToggle.Getter = block =>
            {
                if (block == null || block.GameLogic == null)
                {
                    return false;
                }

                var logic = block.GameLogic.GetAs<BaseHooks>();
                return logic?.StaticOnly ?? false;
            };
            staticOnlyToggle.Setter = (block, value) =>
            {
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    logic.StaticOnly = value;
                }
            };

            staticOnlyToggle.Visible = block => !block.CubeGrid.IsStatic;
            Logging.Debug("FilterListOnOff initialised");

            thrusterControl = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, IMyShipConnector>("autocharge_thrustmode");
            thrusterControl.Title = MyStringId.GetOrCompute("Thruster Control");
            thrusterControl.Tooltip = MyStringId.GetOrCompute("Should the connector also control thrusters?");
            thrusterControl.Multiselect = false;
            thrusterControl.VisibleRowsCount = 4;
            thrusterControl.ItemSelected = (block, selected) =>
            {
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    logic.ThrustMode = (ThrusterMode)selected.First().UserData;
                }
            };
            thrusterControl.ListContent = (block, items, selected) =>
            {
                // Logging.Instance.WriteLine("List content building!");
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("None"),
                        MyStringId.GetOrCompute("No thruster management"),
                        ThrusterMode.None));
                    if (logic.ThrustMode == ThrusterMode.None)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("Electric"),
                        MyStringId.GetOrCompute("Atmospheric and Ion thrusters"),
                        ThrusterMode.ElectricOnly));
                    if (logic.ThrustMode == ThrusterMode.ElectricOnly)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("Hydrogen"),
                        MyStringId.GetOrCompute("H2 thrusters only"),
                        ThrusterMode.HydrogenOnly));
                    if (logic.ThrustMode == ThrusterMode.HydrogenOnly)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("All"),
                        MyStringId.GetOrCompute("ALL attached thrusters."),
                        ThrusterMode.All));
                    if (logic.ThrustMode == ThrusterMode.All)
                    {
                        selected.Add(items.Last());
                    }
                }
            };
            thrusterControl.Visible = block => !block.CubeGrid.IsStatic;

            tankControl = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, IMyShipConnector>("autocharge_tanktmode");
            tankControl.Title = MyStringId.GetOrCompute("Tank Control");
            tankControl.Tooltip = MyStringId.GetOrCompute("Should the connector also control tanks?");
            tankControl.Multiselect = false;
            tankControl.VisibleRowsCount = 4;
            tankControl.ItemSelected = (block, selected) =>
            {
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    logic.TankSetting = (TankMode)selected.First().UserData;
                }
            };
            tankControl.ListContent = (block, items, selected) =>
            {
                var logic = block.GameLogic.GetAs<BaseHooks>();
                if (logic != null)
                {
                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("None"),
                        MyStringId.GetOrCompute("No tank management"),
                        TankMode.None));
                    if (logic.TankSetting == TankMode.None)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("Oxygen"),
                        MyStringId.GetOrCompute("O2 tanks only"),
                        TankMode.OxygenOnly));
                    if (logic.TankSetting == TankMode.OxygenOnly)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("Hydrogen"),
                        MyStringId.GetOrCompute("H2 tanks only"),
                        TankMode.HydrogenOnly));
                    if (logic.TankSetting == TankMode.HydrogenOnly)
                    {
                        selected.Add(items.Last());
                    }

                    items.Add(new MyTerminalControlListBoxItem(
                        MyStringId.GetOrCompute("All"),
                        MyStringId.GetOrCompute("All tanks"),
                        TankMode.All));
                    if (logic.TankSetting == TankMode.All)
                    {
                        selected.Add(items.Last());
                    }
                }
            };
            tankControl.Visible = block => !block.CubeGrid.IsStatic;

            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(separator);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(chargeOnConnectToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(staticOnlyToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(thrusterControl);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(tankControl);
            Logging.Debug("Controls Registered");
        }
    }
}
