// <copyright file="StorageHandler.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System;
    using System.Linq;
    using System.Text;
    using Sandbox.Game.EntityComponents;
    using Sandbox.ModAPI;
    using VRage.ModAPI;

    /// <summary>
    /// Serialises and Deserialises entity storage.
    /// </summary>
    public class StorageHandler
    {
        private static readonly Guid StorageGuid = new Guid("{D249368C-3008-4BA0-934E-90F66A146061}");
        private readonly IMyEntity source;

        private bool enableAutoSwitch = false;

        private bool staticOnly = true;

        private bool thrusters = false;

        /*
         *  V1
         *      enableAutoSwitch
         *      staticOnly
         *  V2
         *      enableAutoSwitch
         *      staticOnly
         *      thrusters
         */

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageHandler"/> class.
        /// </summary>
        /// <param name="source">Source entity to load from.</param>
        public StorageHandler(IMyEntity source)
        {
            this.source = source;
            this.Deserialise();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector should manage batteries.
        /// </summary>
        public bool AutoSwitch
        {
            get
            {
                return this.enableAutoSwitch;
            }

            set
            {
                if (this.enableAutoSwitch != value)
                {
                    this.enableAutoSwitch = value;
                    this.Serialise();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connector should manage batteries.
        /// </summary>
        public bool ThrustersIncluded
        {
            get
            {
                return this.thrusters;
            }

            set
            {
                if (this.thrusters != value)
                {
                    this.thrusters = value;
                    this.Serialise();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sub-grids should be scanned for the target entity.
        /// </summary>
        public bool StaticOnly
        {
            get
            {
                return this.staticOnly;
            }

            set
            {
                if (this.staticOnly != value)
                {
                    this.staticOnly = value;
                    this.Serialise();
                }
            }
        }

        /// <summary>
        /// Deserialises the entity mod storage into current properties.
        /// </summary>
        public void Deserialise()
        {
            if (this.source.Storage != null)
            {
                if (this.source.Storage.ContainsKey(StorageGuid))
                {
                    string dataSource = this.source.Storage.GetValue(StorageGuid);
                    string[] components = dataSource.Split(',');
                    int versionId = int.Parse(components[0]);
                    switch (versionId)
                    {
                        case 1:
                        {
                            this.enableAutoSwitch = bool.Parse(components[1]);
                            this.staticOnly = bool.Parse(components[2]);
                            break;
                        }

                        case 2:
                        {
                            this.enableAutoSwitch = bool.Parse(components[1]);
                            this.staticOnly = bool.Parse(components[2]);
                            this.thrusters = bool.Parse(components[3]);
                            break;
                        }
                    }
                }
            }
        }

        private void Serialise()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("2,");
            sb.Append(this.enableAutoSwitch.ToString());
            sb.Append(",");
            sb.Append(this.staticOnly);
            sb.Append(",");
            sb.Append(this.thrusters);

            if (this.source.Storage == null)
            {
                this.source.Storage = new MyModStorageComponent();
            }

            this.source.Storage.SetValue(StorageGuid, sb.ToString());
        }
    }
}
