// <copyright file="ThrusterMode.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    /// <summary>
    /// Thruster mode configuration modes.
    /// </summary>
    public enum ThrusterMode
    {
        /// <summary>
        /// No control.
        /// </summary>
        None,

        /// <summary>
        /// Atmo and ion engine control only.
        /// </summary>
        ElectricOnly,

        /// <summary>
        /// H2 engines only.
        /// </summary>
        HydrogenOnly,

        /// <summary>
        /// All thrusters.
        /// </summary>
        All,
    }
}
