using Renci.SshNet;

namespace Store_API.RedisConfig
{
    public class SshTunnelManager
    {
        private readonly SshConfig _config;
        private SshClient _sshClient;
        private ForwardedPortLocal _portForward;

        public SshTunnelManager(SshConfig config)
        {
            _config = config;
        }

        // Hàm khởi tạo tunnel SSH
        public void StartTunnel()
        {
            // Kiểm tra xem kết nối đã có sẵn chưa
            if (_sshClient != null && _sshClient.IsConnected)
                return;

            // Khởi tạo SSH client và kết nối
            _sshClient = new SshClient(_config.SshHost, _config.SshPort, _config.SshUsername,
                new PrivateKeyFile(_config.SshKeyFile));

            _sshClient.Connect();
            Console.WriteLine("SSH Connected.");

            // Cấu hình ForwardedPortLocal để tạo tunnel từ localhost đến remote Redis server
            _portForward = new ForwardedPortLocal("localhost", (uint)_config.LocalPort, _config.RemoteHost, (uint)_config.RemotePort);
            _sshClient.AddForwardedPort(_portForward);
            _portForward.Start();
            Console.WriteLine($"SSH Tunnel started: localhost:{_config.LocalPort} -> {_config.RemoteHost}:{_config.RemotePort}");
        }

        // Hàm dừng tunnel SSH
        public void StopTunnel()
        {
            if (_portForward != null && _portForward.IsStarted)
            {
                _portForward.Stop();
                Console.WriteLine("SSH Tunnel stopped.");
            }

            if (_sshClient != null && _sshClient.IsConnected)
            {
                _sshClient.Disconnect();
                Console.WriteLine("SSH Disconnected.");
            }
        }
    }
}
