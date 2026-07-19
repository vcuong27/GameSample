namespace DevelopersHub.RealtimeNetworking.Client
{
    using System.IO;
    using UnityEngine;

    public class Settings : ScriptableObject
    {

        [Header("Credentials")]
        [Tooltip("Server IP address.")]
        [SerializeField] private string _ip = "localhost"; public string ip { get { return _ip; } }

        [Tooltip("Server TCP/UDP port number.")]
        [SerializeField] private int _port = 5555; public int port { get { return _port; } }

        [Header("WebSocket")]
        [Tooltip("Use WebSocket instead of TCP/UDP when RealtimeNetworking.Connect() is called.")]
        [SerializeField] private bool _useWebSocket = false; public bool useWebSocket { get { return _useWebSocket; } }

        [Tooltip("Use wss:// instead of ws://.")]
        [SerializeField] private int _webSocketPort = 5556; public int webSocketPort { get { return _webSocketPort; } }

        [Tooltip("Server WebSocket path. The patched server uses /ws/ by default.")]
        [SerializeField] private string _webSocketPath = "/ws/"; public string webSocketPath { get { return _webSocketPath; } }

        public string webSocketUrl
        {
            get
            {
                string scheme = "ws";
                string host = NormalizeWebSocketHost(_ip);
                string path = string.IsNullOrWhiteSpace(_webSocketPath) ? "/ws/" : _webSocketPath;
                if (!path.StartsWith("/"))
                {
                    path = "/" + path;
                }
                if (!path.EndsWith("/"))
                {
                    path += "/";
                }
                return scheme + "://" + host + ":" + _webSocketPort + path;
            }
        }

        private string NormalizeWebSocketHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return "localhost";
            }

            host = host.Trim();

            // The local server fix uses HttpListener prefix http://localhost:5556/ws/.
            // Normalize loopback IPs to localhost so Unity Editor can connect without Windows URL ACL/admin setup.
            if (host == "127.0.0.1" || host == "::1")
            {
                return "localhost";
            }

            return host;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Developers Hub/Realtime Networking/Settings")]
        public static void CreateSettings()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(DevelopersHub.RealtimeNetworking.Client.Settings).Name);
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                UnityEditor.EditorUtility.FocusProjectWindow();
                Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
                UnityEditor.Selection.activeObject = obj;
            }
            else
            {
                string path = Application.dataPath + "/DevelopersHub/RealtimeNetworking/Resources";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DevelopersHub.RealtimeNetworking.Client.Settings asset = ScriptableObject.CreateInstance<DevelopersHub.RealtimeNetworking.Client.Settings>();
                UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/DevelopersHub/RealtimeNetworking/Resources/Settings.asset");
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.EditorUtility.FocusProjectWindow();
                UnityEditor.Selection.activeObject = asset;
            }
        }
#endif

    }
}