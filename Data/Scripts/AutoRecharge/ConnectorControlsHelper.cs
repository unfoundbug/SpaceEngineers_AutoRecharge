// <copyright file="LightHookHelper.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.LightLink
{
    using System.Collections.Generic;
    using System.Linq;
    using Sandbox.ModAPI;
    using Sandbox.ModAPI.Interfaces.Terminal;
    using VRage.Game.ModAPI;
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

        /// <summary>
        /// Attach controls to light terminal menus.
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

            // Logging.Instance.WriteLine("Light hook controls for " + typeof(IMyLightingBlock).Name + " started.");
            separator = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyLightingBlock>("lightlink_seperator");
            separator.Enabled = (lb) => true;
            separator.Visible = (lb) => true;

            Logging.Debug("Seperator initialised");

            chargeOnConnectToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyLightingBlock>("lightlink_scanSubGrid");
            chargeOnConnectToggle.Title = MyStringId.GetOrCompute("Scan Subgrids");
            chargeOnConnectToggle.OnText = MyStringId.GetOrCompute("Enabled");
            chargeOnConnectToggle.OffText = MyStringId.GetOrCompute("Disabled");
            chargeOnConnectToggle.Tooltip = MyStringId.GetOrCompute("WARNING: Can cause alot of server load!");
            chargeOnConnectToggle.Getter = block =>
            {
                StorageHandler handler = new StorageHandler(block);
                return handler.SubGridScanningEnable;
            };
            chargeOnConnectToggle.Setter = (block, value) =>
            {
                StorageHandler handler = new StorageHandler(block);
                handler.SubGridScanningEnable = value;
                listControl.VisibleRowsCount = listControl.VisibleRowsCount;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            };
            chargeOnConnectToggle.Visible = block => true;
            Logging.Debug("SubGridOnOff initialised");

            staticOnlyToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyLightingBlock>("lightlink_scanSubGrid");
            staticOnlyToggle.Title = MyStringId.GetOrCompute("Filter Available blocks");
            staticOnlyToggle.Tooltip = MyStringId.GetOrCompute("Filters less interesting blocks from appearing in the list, re-enter the menu for this to take effect.");
            staticOnlyToggle.OnText = MyStringId.GetOrCompute("Enabled");
            staticOnlyToggle.OffText = MyStringId.GetOrCompute("Disabled");
            staticOnlyToggle.Getter = block =>
            {
                StorageHandler handler = new StorageHandler(block);
                return handler.BlockFiltering;
            };
            staticOnlyToggle.Setter = (block, value) =>
            {
                StorageHandler handler = new StorageHandler(block);
                handler.BlockFiltering = value;
                block.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            };
            staticOnlyToggle.Visible = block => true;
            Logging.Debug("FilterListOnOff initialised");
            
            MyAPIGateway.TerminalControls.AddControl<IMyLightingBlock>(separator);
            MyAPIGateway.TerminalControls.AddControl<IMyLightingBlock>(chargeOnConnectToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyLightingBlock>(filterListCB);
            MyAPIGateway.TerminalControls.AddControl<IMyLightingBlock>(listControl);
            MyAPIGateway.TerminalControls.AddControl<IMyLightingBlock>(flagControl);
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(separator);
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(chargeOnConnectToggle);
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(filterListCB);
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(listControl);
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(flagControl);
            Logging.Debug("Controls Registered");
        }
    }
}
