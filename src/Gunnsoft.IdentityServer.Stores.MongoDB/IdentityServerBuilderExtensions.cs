using System;
using System.Threading;
using System.Threading.Tasks;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.Clients;
using Gunnsoft.IdentityServer.Stores.MongoDB.Collections.PersistedGrants;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Gunnsoft.IdentityServer.Stores.MongoDB
{
    public static class IdentityServerBuilderExtensions
    {
        public static async Task<IIdentityServerBuilder> AddMongoClientStoreAsync
        (
            this IIdentityServerBuilder extended,
            MongoUrl mongoUrl,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            MongoConfigurator.ConfigureConventions();

            BsonClassMapper.MapClient();

            var mongoClient = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

            await MongoConfigurator.ConfigureClientsCollectionAsync
            (
                mongoDatabase,
                cancellationToken
            );

            var clientsCollection = mongoDatabase.GetCollection<ClientDocument>(CollectionNames.Clients);

            extended.Services.AddTransient<IClientStore>(cc => new MongoClientStore(clientsCollection));

            return extended;
        }

        public static async Task<IIdentityServerBuilder> AddMongoPersistedGrantStoreAsync
        (
            this IIdentityServerBuilder extended,
            MongoUrl mongoUrl,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            MongoConfigurator.ConfigureConventions();

            BsonClassMapper.MapPeristedGrant();

            var mongoClient = new MongoClient(mongoUrl);

            if (mongoUrl.DatabaseName == null)
            {
                throw new ArgumentException("The connection string must contain a database name.", mongoUrl.Url);
            }

            var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);

            await MongoConfigurator.ConfigurePersistedGrantsCollectionAsync
            (
                mongoDatabase,
                cancellationToken
            );

            var persistedGrantsCollection = mongoDatabase.GetCollection<PersistedGrantDocument>(CollectionNames.PersistedGrants);

            extended.Services.AddTransient<IPersistedGrantStore>(cc => new MongoPersistedGrantStore(persistedGrantsCollection));

            return extended;
        }
    }
}
