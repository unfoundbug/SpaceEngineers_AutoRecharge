// <copyright file="ConnectorControlsHelper.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
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
        private static IMyTerminalControlOnOffSwitch includeThrustersToggle;

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
                StorageHandler handler = new StorageHandler(block);
                return handler.AutoSwitch;
            };
            chargeOnConnectToggle.Setter = (block, value) =>
            {
                StorageHandler handler = new StorageHandler(block);
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
                StorageHandler handler = new StorageHandler(block);
                return handler.StaticOnly;
            };
            staticOnlyToggle.Setter = (block, value) =>
            {
                StorageHandler handler = new StorageHandler(block);
                handler.StaticOnly = value;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            };

            staticOnlyToggle.Visible = block => !block.CubeGrid.IsStatic;
            Logging.Debug("FilterListOnOff initialised");

            includeThrustersToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyShipConnector>("autoswitch_thrusters");
            includeThrustersToggle.Title = MyStringId.GetOrCompute("Thruster Management");
            includeThrustersToggle.Tooltip = MyStringId.GetOrCompute("If enabled, thrusters will also Disable on connection, and re-enable on disconnection");
            includeThrustersToggle.OnText = MyStringId.GetOrCompute("Enabled");
            includeThrustersToggle.OffText = MyStringId.GetOrCompute("Disabled");
            includeThrustersToggle.Getter = block =>
            {
                StorageHandler handler = new StorageHandler(block);
                return handler.ThrustersIncluded;
            };
            includeThrustersToggle.Setter = (block, value) =>
            {
                StorageHandler handler = new StorageHandler(block);
                handler.ThrustersIncluded = value;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            };

            includeThrustersToggle.Visible = block => !block.CubeGrid.IsStatic;

            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(separator);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(chargeOnConnectToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(staticOnlyToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyShipConnector>(includeThrustersToggle);
            Logging.Debug("Controls Registered");
        }
    }
}
