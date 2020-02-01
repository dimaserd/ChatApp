using Croco.Core.Abstractions.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zoo.Core
{
    public class ActionOptimizations
    {
        public ActionOptimizations(ICrocoCacheManager cacheManager)
        {
            CacheManager = cacheManager;
        }

        ICrocoCacheManager CacheManager { get; }

    }
}
