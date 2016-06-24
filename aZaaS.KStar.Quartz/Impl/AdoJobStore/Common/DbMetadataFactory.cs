﻿using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Quartz.Impl.AdoJobStore.Common
{
    /// <summary>
    /// Base class for the DbMetadata Factory implentations
    /// </summary>
    public abstract class DbMetadataFactory
    {
        /// <summary>
        /// Gets the supported provider names.
        /// </summary>
        /// <returns>The enumeration of the supported provider names</returns>
        public abstract IEnumerable<string> GetProviderNames();

        /// <summary>
        /// Gets the database metadata assotiated to the specified provider name.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>The metadata instance for the requested provider</returns>
        public abstract DbMetadata GetDbMetadata(string providerName);
    }
}
