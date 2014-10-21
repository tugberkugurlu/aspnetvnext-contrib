using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNet.Http.ETag
{
    public class EntityTag
    {
        public EntityTag(string resourceKey) 
        {
            if(resourceKey == null)
            {
                throw new ArgumentNullException("resourceKey");
            }
            
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; private set; }
        public string Value { get; set; }
        public DateTimeOffset LastModified { get; set; }

        public bool IsValid(DateTimeOffset modifiedSince) 
        {
            DateTimeOffset lastModified = LastModified.UtcDateTime;
            return (lastModified.AddSeconds(-1) < modifiedSince.UtcDateTime);
        }
    }
}