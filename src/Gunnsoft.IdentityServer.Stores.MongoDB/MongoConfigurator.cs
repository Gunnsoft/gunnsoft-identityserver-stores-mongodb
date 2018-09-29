using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static class MongoConfigurator
    {
        public static void ConfigureConventions()
        {
            const string conventionName = "gunnsoft-identityserver-stores-mongodb";
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Remove(conventionName);
            ConventionRegistry.Register(conventionName, conventionPack, t => true);
        }

        public static async Task ConfigureClientsCollectionAsync
        (
            IMongoDatabase mongoDatabase,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            if (!await CollectionExistsAsync(mongoDatabase, CollectionNames.Clients, cancellationToken))
            {
                await mongoDatabase.CreateCollectionAsync(CollectionNames.Clients);
            }

            var clientsCollection = mongoDatabase.GetCollection<ClientDocument>(CollectionNames.Clients);
            var clientIndexes = new[]
            {
                new CreateIndexModel<ClientDocument>
                (
                    Builders<ClientDocument>.IndexKeys.Ascending(u => u.ClientId),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }
                )
            };

            await clientsCollection.Indexes.CreateManyAsync
            (
                clientIndexes,
                cancellationToken: cancellationToken
            );
        }

        public static void ConfigureClientMapping()
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

        public static async Task ConfigurePersistedGrantsCollectionAsync
        (
            IMongoDatabase mongoDatabase,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            if (!await CollectionExistsAsync(mongoDatabase, CollectionNames.PersistedGrants, cancellationToken))
            {
                await mongoDatabase.CreateCollectionAsync(CollectionNames.PersistedGrants);
            }
        }

        public static void ConfigurePeristedGrantMapping()
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

        private static async Task<bool> CollectionExistsAsync
        (
            IMongoDatabase mongoDatabase,
            string collectionName,
            CancellationToken cancellationToken
        )
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await mongoDatabase.ListCollectionsAsync
            (
                new ListCollectionsOptions
                {
                    Filter = filter
                }
            );

            return await collections.AnyAsync(cancellationToken);
        }
    }
}