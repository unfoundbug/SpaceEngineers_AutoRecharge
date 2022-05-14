// <copyright file="StorageHandler.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.LightLink
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

        /*
         *  V0
         *      EntityId
         *
         *  V1
         *      EntityId
         *      SubGridScanning
         *      BlockFiltering
         *      ActiveFlags
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
        /// Gets or sets a value representing the flags for light enable source.
        /// </summary>
        public bool AutoSwitch
        {
            get
            {
                return this.enableAutoSwitch;
            }

            set
            {
                if (enableAutoSwitch != value)
                {
                    this.enableAutoSwitch = value;
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
        /// <returns>Returns true if the loaded settings should be saved back to mod storage.</returns>
        public bool Deserialise()
        {
            bool newSettingsRequired = true;
            if (this.source.Storage != null)
            {
                if (this.source.Storage.ContainsKey(StorageGuid))
                {
                    string dataSource = this.source.Storage.GetValue(StorageGuid);
                    if (!dataSource.Contains(','))
                    {
                        // V0 Processing, contains only target entity ID
                        this.targetEntity = long.Parse(dataSource);
                    }
                    else
                    {
                        string[] components = dataSource.Split(',');
                        int versionId = int.Parse(components[0]);
                        switch (versionId)
                        {
                            case 1:
                                {
                                    this.targetEntity = long.Parse(components[1]);
                                    this.subGridScanning = bool.Parse(components[2]);
                                    this.blockFiltering = bool.Parse(components[3]);
                                    var rawFlag = int.Parse(components[4]);
                                    this.flags = (LightEnableOptions)rawFlag;

                                    newSettingsRequired = false;
                                    break;
                                }
                        }
                    }
                }
            }

            Logging.Warn("Settings Upgrade required for " + this.source.DisplayName);
            return newSettingsRequired;
        }

        private void Serialise()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("1,");
            sb.Append(this.enableAutoSwitch.ToString());
            sb.Append(",");
            sb.Append(this.staticOnly);

            if (this.source.Storage == null)
            {
                this.source.Storage = new MyModStorageComponent();
            }

            this.source.Storage.SetValue(StorageGuid, sb.ToString());
        }
    }
}
