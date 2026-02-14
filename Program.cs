using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;

namespace MinecraftStatusAgent
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MinecraftStatusForm());
        }
    }

    public class MinecraftStatusForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip contextMenu;
        private System.Timers.Timer statusCheckTimer;
        private Label statusLabel;
        private TextBox logBox;

        // ⚙️ CONFIGURATION - À MODIFIER AU PREMIER LANCEMENT
        private string SERVER_URL = "http://localhost:5000";
        private string USER_ID = "1";
        private int CHECK_INTERVAL = 10000;
        private string AGENT_VERSION = "1.0.0";

        public MinecraftStatusForm()
        {
            InitializeForm();
            InitializeTray();
            InitializeTimer();
            Log($"🚀 Agent démarré v{AGENT_VERSION}");
        }

        private void InitializeForm()
        {
            this.Text = "🎮 Minecraft Status Agent";
            this.Size = new System.Drawing.Size(550, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Title
            var titleLabel = new Label
            {
                Text = "🎮 Minecraft Status Agent v" + AGENT_VERSION,
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.LimeGreen,
                AutoSize = false,
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 45)
            };
            this.Controls.Add(titleLabel);

            // Status Display
            statusLabel = new Label
            {
                Text = "⏳ Chargement...",
                Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Yellow,
                AutoSize = false,
                Height = 100,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                BackColor = System.Drawing.Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(statusLabel);

            // Log Label
            var logLabel = new Label
            {
                Text = "📋 Logs (actifs):",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.LimeGreen,
                AutoSize = false,
                Height = 25,
                Dock = DockStyle.Top,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 45),
                Padding = new Padding(5, 5, 0, 0)
            };
            this.Controls.Add(logLabel);

            // Log Box
            logBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                ForeColor = System.Drawing.Color.LimeGreen,
                Font = new System.Drawing.Font("Courier New", 9),
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(logBox);

            // Events
            this.FormClosing += (s, e) => { e.Cancel = true; this.Hide(); };
            this.Load += (s, e) => this.Hide();
        }

        private void InitializeTray()
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "🎮 Minecraft Status Agent",
                Visible = true
            };

            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Afficher fenêtre", null, ShowForm);
            contextMenu.Items.Add("Configuration", null, ShowConfig);
            contextMenu.Items.Add("Rafraîchir", null, ManualRefresh);
            contextMenu.Items.Add("---");
            contextMenu.Items.Add("Quitter", null, ExitApp);

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += (s, e) => ShowForm(null, null);
        }

        private void InitializeTimer()
        {
            statusCheckTimer = new System.Timers.Timer(CHECK_INTERVAL);
            statusCheckTimer.Elapsed += CheckMinecraftStatus;
            statusCheckTimer.AutoReset = true;
            statusCheckTimer.Start();

            CheckMinecraftStatus(null, null);
        }

        private bool IsMinecraftRunning()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    try
                    {
                        string name = p.ProcessName.ToLower();
                        if (name.Contains("javaw") || 
                            name.Contains("minecraft") || 
                            name.Contains("java") ||
                            name.Contains("launcher"))
                        {
                            // Vérifier que c'est bien Minecraft en regardant le chemin
                            try
                            {
                                string path = p.MainModule.FileName.ToLower();
                                if (path.Contains("minecraft") || path.Contains("appdata") || path.Contains("java"))
                                    return true;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log($"❌ Erreur détection: {ex.Message}");
                return false;
            }
        }

        private async void CheckMinecraftStatus(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool isRunning = IsMinecraftRunning();
            string status = isRunning ? "online" : "offline";
            string emoji = isRunning ? "🟢" : "🔴";
            string text = isRunning ? "EN LIGNE" : "HORS LIGNE";

            this.Invoke(() =>
            {
                statusLabel.Text = $"{emoji}\n{text}\n{DateTime.Now:HH:mm:ss}";
                statusLabel.ForeColor = isRunning ? 
                    System.Drawing.Color.LimeGreen : 
                    System.Drawing.Color.Red;
                trayIcon.Text = $"Minecraft: {text}";
            });

            await SendHeartbeat(status);
        }

        private async Task SendHeartbeat(string status)
        {
            try
            {
                var client = new RestClient(SERVER_URL);
                var request = new RestRequest("/api/minecraft-status", Method.Post);
                
                var payload = new
                {
                    user_id = USER_ID,
                    status = status,
                    timestamp = DateTime.UtcNow.ToString("O"),
                    agent_version = AGENT_VERSION
                };

                request.AddJsonBody(payload);
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                    Log($"✅ Heartbeat envoyé: {status}");
                else
                    Log($"⚠️ Serveur retourna: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Log($"❌ Erreur heartbeat: {ex.Message.Substring(0, Math.Min(50, ex.Message.Length))}");
            }
        }

        private void Log(string message)
        {
            this.Invoke(() =>
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
                logBox.AppendText(logEntry + "\n");
                logBox.ScrollToCaret();

                // Limiter à 100 lignes
                int lineCount = logBox.Lines.Length;
                if (lineCount > 100)
                {
                    string[] lines = logBox.Lines;
                    logBox.Text = string.Join("\n", lines, 10, lineCount - 10);
                    logBox.ScrollToCaret();
                }
            });
        }

        private void ShowForm(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ShowConfig(object sender, EventArgs e)
        {
            MessageBox.Show(
                $"Configuration de l'Agent:\n\n" +
                $"📍 Serveur: {SERVER_URL}\n" +
                $"👤 User ID: {USER_ID}\n" +
                $"⏱️ Check interval: {CHECK_INTERVAL}ms\n" +
                $"📦 Version: {AGENT_VERSION}\n\n" +
                $"Pour modifier:\n" +
                $"1. Ouvrir Program.cs\n" +
                $"2. Modifier les valeurs\n" +
                $"3. Recompiler",
                "Configuration");
        }

        private void ManualRefresh(object sender, EventArgs e)
        {
            Log("🔄 Rafraîchissement manuel...");
            CheckMinecraftStatus(null, null);
        }

        private void ExitApp(object sender, EventArgs e)
        {
            statusCheckTimer.Stop();
            trayIcon.Visible = false;
            Log("👋 Arrêt de l'agent...");
            Application.Exit();
        }
    }
}
