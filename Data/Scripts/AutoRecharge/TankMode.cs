// <copyright file="TankMode.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Gas tank mode.
    /// </summary>
    public enum TankMode
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None,

        /// <summary>
        /// Only stockpile h2.
        /// </summary>
        HydrogenOnly,

        /// <summary>
        /// Only stockpile o2.
        /// </summary>
        OxygenOnly,

        /// <summary>
        /// Stockpile all tanks.
        /// </summary>
        All,
    }
}
