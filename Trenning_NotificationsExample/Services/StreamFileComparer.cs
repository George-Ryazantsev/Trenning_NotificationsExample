using System.Buffers;
using System.Text.RegularExpressions;
using Trenning_NotificationsExample.Models;
namespace Trenning_NotificationsExample.Services
{
    public class StreamFileComparer
    {
        private readonly PassportChangesService _passportChangesService;
        private const int batchSize = 40000;
        public StreamFileComparer(PassportChangesService passportChangesService)
        {
            _passportChangesService = passportChangesService;
        }
        public StreamFileComparer()
        {
            
        }
        public async Task LoadFileDataIntoDbAsync(string filePath)
        {
            var batch = new List<InactivePassports>();            

            var file1Lines = File.ReadLinesAsync(filePath).GetAsyncEnumerator();
            Console.WriteLine("Запись в MongoDb началась");

            bool file1HasLines = await file1Lines.MoveNextAsync();
            file1HasLines = await file1Lines.MoveNextAsync();

            var pool = ArrayPool<InactivePassports>.Shared;
            var batchp = pool.Rent(batchSize); 
            int count = 0;

            while (file1HasLines)
            {                
                for (int i = 0; i < batchSize & file1HasLines; i++)
                {
                    var line = file1Lines.Current;

                    int commaIndex = line.IndexOf(',');

                    string series = line.Substring(0, commaIndex).Trim();
                    string number = line.Substring(commaIndex + 1).Trim();
                   

                    batchp[count++] = new InactivePassports { Id = $"{series},{number}" };                    

                    file1HasLines = await file1Lines.MoveNextAsync();
                }
              

                await _passportChangesService.WriteFileToDb(batchp.Take(count).ToList(), batchSize);                
                
                Array.Clear(batchp, 0, count);
                count = 0;
                pool.Return(batchp); 

            }

            Console.WriteLine("Запись файла в MongoDb завершена");
        }

        public async Task CompareFilesAsync(string file1Path, string file2Path)
        {        

            var file1Lines = File.ReadLinesAsync(file1Path).GetAsyncEnumerator();
            var file2Lines = File.ReadLinesAsync(file2Path).GetAsyncEnumerator();

            Console.WriteLine("Сравнение файлов начато...");

            var listRemovedPsprts = new List<PassportChanges>();
            var listAddedPsprts = new List<PassportChanges>();

            bool file1HasLines = await file1Lines.MoveNextAsync();
            bool file2HasLines = await file2Lines.MoveNextAsync();

            while (file1HasLines || file2HasLines)
            {
                HashSet<string> batch1 = new HashSet<string>();
                HashSet<string> batch2 = new HashSet<string>();

                for (int i = 0; i < batchSize && file1HasLines; i++)
                {
                    batch1.Add(file1Lines.Current);
                    file1HasLines = await file1Lines.MoveNextAsync();
                }

                for (int i = 0; i < batchSize && file2HasLines; i++)
                {
                    batch2.Add(file2Lines.Current);
                    file2HasLines = await file2Lines.MoveNextAsync();
                }

                var removed = batch1.Except(batch2);       
                foreach (var line in removed)
                {
                    var change = ParsePassportChanges(line, "Removed");
                    listRemovedPsprts.Add(change);
                }
                await _passportChangesService.WriteFileToDb(listRemovedPsprts, batchSize);

                var added = batch2.Except(batch1);
                foreach (var line in added)
                {
                    var change = ParsePassportChanges(line, "Added");
                    listAddedPsprts.Add(change);                  
                }
                 await _passportChangesService.WriteFileToDb(listAddedPsprts, batchSize);

                batch1.Clear();
                batch2.Clear();

                listAddedPsprts.Clear();
                listRemovedPsprts.Clear();

            }
        }
        private PassportChanges ParsePassportChanges(string line, string changeType)
        {
            var parts = line.Split(',');
            return new PassportChanges
            {
                Id = $"{parts[0].Trim()},{parts[1].Trim()}", 
                ChangeType = changeType, 
                ChangeDate = DateTime.Now 
            };

        }        
    }
}


