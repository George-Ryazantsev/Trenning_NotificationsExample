using Trenning_NotificationsExample.Services;

namespace Trenning_NotificationsExample.Tests.FileUpdateServiceTests
{
    public class FileUpdateServiceTests
    {
      [Test]
        public async Task UpdateFileAsync_ShouldDownloadAndUnzipFile_WhenNoFilesExist()
        {           
            var testFileUrl = "https://www.dropbox.com/scl/fi/pvx75x4vqlv6qzjlcu9r5/File-For-test.zip?rlkey=yphtkchtda7jz7gw6nuh2yw6f&st=86hn2bwm&dl=1";
            string fileName = Path.GetFileName(new Uri(testFileUrl).LocalPath);
            var destinationPath = "ArchivedTestFiles\\"; 
            
            var fileUpdateService = new FileUpdateService(new HttpClient());                        

            Directory.CreateDirectory(destinationPath);            

            var zipFilePath = Path.Combine(destinationPath, fileName);

            var extractedFilePath = Path.Combine("UnZipFiles", "File For test.csv");
            
            var result = await fileUpdateService.UpdateFileAsync(testFileUrl, destinationPath);            
            
            Assert.That(File.Exists(result));

            fileUpdateService.DeleteUnnecessaryFile(zipFilePath);
            fileUpdateService.DeleteUnnecessaryFile(extractedFilePath);

        }

          [Test]
        public async Task UpdateFileAsync_ShouldReplaceOldFile_WhenFileAlreadyExists()
        {          
            var fileUrl = "http://example.com/test.zip";
            var destinationPath = "TestReplaceFiles\\";
            Directory.CreateDirectory(destinationPath);
            
            var fileUpdateService = new FileUpdateService(new HttpClient());

            var oldFilePath = Path.Combine("UnZipFiles", "Outdated Data.csv");
            File.WriteAllText(oldFilePath, "Old data");
            
            var zipFilePath = Path.Combine(destinationPath, "test.zip");            
            
            var result = await fileUpdateService.UpdateFileAsync(fileUrl, destinationPath);
            
            Assert.That(File.Exists(result));
            Assert.That(File.Exists(oldFilePath));

            fileUpdateService.DeleteUnnecessaryFile(oldFilePath);
            fileUpdateService.DeleteUnnecessaryFile(zipFilePath);
        }
                      
        [Test]
        public void DeleteUnnecessaryFile_ShouldDeleteFile()
        {            
            var filePath = "TestUnZipFiles\\";
            Directory.CreateDirectory(filePath);

            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            var fileToDelete = Path.Combine(destinationPath, "test.csv");
            File.WriteAllText(fileToDelete, "Test data");

            var fileUpdateService = new FileUpdateService(new HttpClient());
            
            fileUpdateService.DeleteUnnecessaryFile(fileToDelete);
            
            Assert.That(!File.Exists(fileToDelete));            
        }
    }
}
