namespace Trenning_NotificationsExample.Services
{
    public class PassportUpdateHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        PassportChangesService _passportChangesService;
        FileUpdateService _fileUpdateService;
        public PassportUpdateHostedService(PassportChangesService passportChangesService, FileUpdateService fileUpdateService)
        {
            _passportChangesService = passportChangesService;
            _fileUpdateService = fileUpdateService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdatePassportsAsync, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }
        private async void UpdatePassportsAsync(object state)
        {
            string UnzippedFile1Path, UnzippedFile2Path;

            string fileDownloadDir = "ArchivedFiles\\";
            string unZipFilesPath = "UnZipFiles\\";

            Directory.CreateDirectory(fileDownloadDir);

            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), fileDownloadDir);

            string fileUrl = "https://www.dropbox.com/scl/fi/148rfn5fewyl605q0m4lh/DataZ.zip?rlkey=p4t8etu7d1wy2yav2i3u2pzq0&st=3a6jyzbc&dl=1";
            //string fileUrl = "https://www.dropbox.com/scl/fi/el4itqnexz09jov2fjv06/DataZ.zip?rlkey=n6ivk5epo1e9ym8s7tzzgtatp&st=nmfzrntq&dl=1";
             string UpdatedfileUrl = "https://www.dropbox.com/scl/fi/8hz7275w44lnivw7gl7b6/DataU.zip?rlkey=yx0wtbo2ph49z9bi64ifi0pyu&st=a0nisa5a&dl=1";

            string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
            string UpdatedFileName = Path.GetFileName(new Uri(UpdatedfileUrl).LocalPath);

            if (Directory.GetFiles(destinationPath).Length > 0)
            {
                StreamFileComparer comparer = new StreamFileComparer(_passportChangesService);
                var sortedFiles = Directory.GetFiles(unZipFilesPath).OrderBy(file => file.Length).ToArray();

                UnzippedFile1Path = sortedFiles[1];
                UnzippedFile2Path = sortedFiles[0];

                await comparer.LoadFileDataIntoDbAsync(UnzippedFile1Path);
                await comparer.CompareFilesAsync(UnzippedFile1Path, UnzippedFile2Path);
            }
            else
            {
                UnzippedFile1Path = await _fileUpdateService.UpdateFileAsync(fileUrl, destinationPath);

                StreamFileComparer comparer = new StreamFileComparer(_passportChangesService);
              // unZipFilesPath = Path.Combine(Directory.GetCurrentDirectory(), unZipFilesPath);

                await comparer.LoadFileDataIntoDbAsync(UnzippedFile1Path);                

                UnzippedFile2Path = await _fileUpdateService.UpdateFileAsync(UpdatedfileUrl, destinationPath);

                await comparer.CompareFilesAsync(UnzippedFile1Path, UnzippedFile2Path);
            }

           // _fileUpdateService.DeleteUnnecessaryFile(UnzippedFile1Path);

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
