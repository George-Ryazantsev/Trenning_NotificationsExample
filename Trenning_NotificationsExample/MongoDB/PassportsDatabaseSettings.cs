namespace Trenning_NotificationsExample.MongoDB
{
    public class PassportsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PassportsChangesCollectionName { get; set; } = null!;
        public string InactivePassportsCollectionName { get; set; } = null!;
    }
}
