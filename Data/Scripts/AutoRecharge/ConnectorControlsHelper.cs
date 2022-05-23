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
        private static bool controlsInit = false;
        private static IMyTerminalControlSeparator separator;
        private static IMyTerminalControlOnOffSwitch chargeOnConnectToggle;
        private static IMyTerminalControlOnOffSwitch staticOnlyToggle;
        private static IMyTerminalControlListbox thrusterControl;

        /// <summary>
        /// Attach controls to terminal menus.
        /// </summary>
        public static void AttachControls()
        {
            if (!controlsInit)
            {
                Logging.Debug("Initialising Controls");
                controlsInit = true;
            }
            else
            {
                return;
            }

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
                StorageHandler handler = StorageCache.Instance.GetHandler(block);
                return handler.AutoSwitch;
            };
            chargeOnConnectToggle.Setter = (block, value) =>
            {
                StorageHandler handler = StorageCache.Instance.GetHandler(block);
                handler.AutoSwitch = value;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
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
                StorageHandler handler = StorageCache.Instance.GetHandler(block);
                return handler.StaticOnly;
            };
            staticOnlyToggle.Setter = (block, value) =>
            {
                StorageHandler handler = StorageCache.Instance.GetHandler(block);
                handler.StaticOnly = value;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
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
                StorageHandler handler = StorageCache.Instance.GetHandler(block);
                handler.ThrusterManagament = (ThrusterMode)selected.First().UserData;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            };
            thrusterControl.ListContent = (block, items, selected) =>
            {
                // Logging.Instance.WriteLine("List content building!");
                StorageHandler storage = StorageCache.Instance.GetHandler(block);

                items.Add(new MyTerminalControlListBoxItem(
                    MyStringId.GetOrCompute("None"),
                    MyStringId.GetOrCompute("No thruster management"),
                    ThrusterMode.None));
                if (storage.ThrusterManagament == ThrusterMode.None)
                {
                    selected.Add(items.Last());
                }

                items.Add(new MyTerminalControlListBoxItem(
                    MyStringId.GetOrCompute("Electric"),
                    MyStringId.GetOrCompute("Atmospheric and Ion thrusters"),
                    ThrusterMode.ElectricOnly));
                if (storage.ThrusterManagament == ThrusterMode.ElectricOnly)
                {
                    selected.Add(items.Last());
                }

                items.Add(new MyTerminalControlListBoxItem(
                    MyStringId.GetOrCompute("Hydrogen"),
                    MyStringId.GetOrCompute("H2 thrusters only"),
                    ThrusterMode.HydrogenOnly));
                if (storage.ThrusterManagament == ThrusterMode.HydrogenOnly)
                {
                    selected.Add(items.Last());
                }

                items.Add(new MyTerminalControlListBoxItem(
                    MyStringId.GetOrCompute("All"),
                    MyStringId.GetOrCompute("ALL attached thrusters."),
                    ThrusterMode.All));
                if (storage.ThrusterManagament == ThrusterMode.All)
                {
                    selected.Add(items.Last());
                }
            };
            thrusterControl.Visible = block => !block.CubeGrid.IsStatic;

            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(separator);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(chargeOnConnectToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(staticOnlyToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(thrusterControl);
            Logging.Debug("Controls Registered");
        }
    }
}
