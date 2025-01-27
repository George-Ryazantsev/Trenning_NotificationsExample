using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Buffers;
using Trenning_NotificationsExample.Models;
using Trenning_NotificationsExample.MongoDB;

namespace Trenning_NotificationsExample.Services
{
    public class PassportChangesService
    {
        private readonly IMongoCollection<PassportChanges> _passportChangesCollection;
        private readonly IMongoCollection<InactivePassports> _inactivePassportCollection;

        public PassportChangesService(
            IOptions<PassportsDatabaseSettings> passportsDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                passportsDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                passportsDatabaseSettings.Value.DatabaseName);

            _inactivePassportCollection = mongoDatabase.GetCollection<InactivePassports>(
                passportsDatabaseSettings.Value.InactivePassportsCollectionName);

            _passportChangesCollection = mongoDatabase.GetCollection<PassportChanges>(
                passportsDatabaseSettings.Value.PassportsChangesCollectionName);
        }
        public async Task<IEnumerable<PassportChanges>> GetAllChangesAsync()
        {
            return await _passportChangesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<InactivePassports> GetInactivePassportAsync(string series, string number)
        {            
            var id = $"{series},{number}";

            return await _inactivePassportCollection.Find(x => x.Id == id).FirstOrDefaultAsync();                  
        }

        public async Task<IEnumerable<PassportChanges>> GetChangesByDateAsync(DateTime date)
        {

            return await _passportChangesCollection
                .Find(c => c.ChangeDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<PassportChanges>> GetHistoryAsync(string series, string number)
        {
            return await _passportChangesCollection
                .Find(c => c.Series == series && c.Number == number)
                .SortBy(c => c.ChangeDate)
                .ToListAsync();
        }

        public async Task WriteFileToDb(List<InactivePassports> batch, int batchSize)
        {            
        
            batchSize = batchSize / 8;           
            var batches = batch
                .Select((doc, index) => new { doc, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.doc).ToList())
                .ToList();


            var tasks = batches
           .Select(batch => Task.Run(async () =>
           {
               await _inactivePassportCollection.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = false });               
           }))
             .ToArray();
            
            await Task.WhenAll(tasks);            
            
        }
        public async Task WriteFileToDb(List<PassportChanges> batch, int batchSize)
        {
            batchSize = batchSize / 8;            
            var batches = batch
                .Select((doc, index) => new { doc, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.doc).ToList())
                .ToList();

            var tasks = batches
           .Select(batch => Task.Run(async () =>
           {
               await _passportChangesCollection.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = false });
           }))
             .ToArray();

            await Task.WhenAll(tasks);
        }

    }



}
