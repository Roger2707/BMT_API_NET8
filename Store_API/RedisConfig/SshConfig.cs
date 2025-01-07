namespace Store_API.RedisConfig
{
    public class SshConfig
    {
        public string SshHost { get; set; }
        public int SshPort { get; set; }
        public string SshUsername { get; set; }
        public string SshKeyFile { get; set; }
        public int LocalPort { get; set; }
        public string RemoteHost { get; set; }
        public int RemotePort { get; set; }
    }
}
