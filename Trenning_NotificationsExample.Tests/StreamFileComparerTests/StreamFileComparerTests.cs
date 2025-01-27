using Trenning_NotificationsExample.Services;

namespace Trenning_NotificationsExample.Tests.StreamFileComparerTests
{
    [TestFixture]
    public class StreamFileComparerTests
    {        
        [Test]
        public async Task LoadFileDataIntoDbAsync_DoesNotThrowException()
        {            
            var comparer = new StreamFileComparer();
           
            var testFilePath = "test_passports.txt";
            File.WriteAllLines(testFilePath, new[] { "1234,567890" });

            Assert.DoesNotThrowAsync(async () =>
            await comparer.LoadFileDataIntoDbAsync(testFilePath));

            File.Delete(testFilePath);
        }
    }
}
