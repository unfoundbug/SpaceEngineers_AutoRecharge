// <copyright file="ConnectorHooks.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using Sandbox.Common.ObjectBuilders;
    using VRage.Game.Components;

    /// <summary>
    /// Hooks for MyObjectBuilder_InteriorLight, also impacts LightPanel.
    /// </summary>
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ShipConnector), true)]
    public class ConnectorHooks : BaseHooks
    {
    }
}