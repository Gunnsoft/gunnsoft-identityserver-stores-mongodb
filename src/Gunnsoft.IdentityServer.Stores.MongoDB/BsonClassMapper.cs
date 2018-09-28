using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants;
using MongoDB.Bson.Serialization;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static class BsonClassMapper
    {
        public static void MapClient()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(ClientDocument)))
            {
                BsonClassMap.RegisterClassMap<ClientDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }

        public static void MapPeristedGrant()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PersistedGrantDocument)))
            {
                BsonClassMap.RegisterClassMap<PersistedGrantDocument>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}