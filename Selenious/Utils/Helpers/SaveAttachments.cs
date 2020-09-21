namespace Selenious.Utils.Helpers
{
    using NUnit.Framework;

    public static class SaveAttachments
    {
        /// <summary>
        /// Attaches a file to the current test - attribute of Nunit.
        /// </summary>
        public static void SaveAttachmentsToTestContext(string[] filePaths)
        {
            if (filePaths != null)
            {
                foreach (var filePath in filePaths)
                {
                    TestContext.AddTestAttachment(filePath);
                }
            }
        }
    }
}