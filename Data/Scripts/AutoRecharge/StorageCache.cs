// <copyright file="StorageCache.cs" company="UnFoundBug">
// Copyright (c) UnFoundBug. All rights reserved.
// </copyright>

namespace UnFoundBug.AutoSwitch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnFoundBug.AutoSwitch;
    using VRage.ModAPI;

    /// <summary>
    /// Caches instances of the storage handler to prevent repeated deserialisation errors.
    /// </summary>
    public class StorageCache
    {
        private static StorageCache instance;
        private Dictionary<long, StorageHandler> internalStore = new Dictionary<long, StorageHandler>();

        /// <summary>
        /// Gets or creates an instance of the storage handler for the requested entity.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <returns>A storage wrapper for the selected entity.</returns>
        public StorageHandler GetHandler(IMyEntity entity)
        {
            if (!this.internalStore.Keys.Contains(entity.EntityId))
            {
                this.internalStore.Add(entity.EntityId, new StorageHandler(entity));
            }

            return this.internalStore[entity.EntityId];
        }

        public static StorageCache Instance 
        {
            get
            {
                if (instance == null)
                    instance = new StorageCache();
                return instance;
            }
        }
    }
}
