
namespace MinecraftResourceExtractor.Class
{
    public class JsonConfig
    {
        public class Config
        {
            public class Root
            {
                public bool isFirstUse { get; set; }
            }
        }

        public class IndexFile
        {
            public class Root
            {
                public Dictionary<string,JsonConfig.IndexFile.FileInfo>? objects { get; set; }
            }

            public class FileInfo
            {
                public string? hash { get; set; }
                public long size { get; set; }
            }
        }
    }
}
