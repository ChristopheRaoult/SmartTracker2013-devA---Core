using System.Windows.Forms;
using smartTracker.Properties;

namespace smartTracker
{
    public partial class UpdateDownload : Form
    {
        private Conversive.AutoUpdater.AutoUpdater autoUpdater;        
        public UpdateDownload(Conversive.AutoUpdater.AutoUpdater autoUpdater)
        {
            InitializeComponent();
            this.autoUpdater = autoUpdater;
            this.autoUpdater.updateProgressBar += new Conversive.AutoUpdater.AutoUpdater.UpdateProgressbar(autoUpdater_updateProgressBar);
        }

        private void autoUpdater_updateProgressBar(int iPourcent, int currentSize, int totalSize)
        {
            try
            {
                Invoke((MethodInvoker)delegate
                {

                    progressBar.Value = iPourcent;
                    double curSizeKo = currentSize / 1024;
                    double totSizeKo = totalSize / 1024;
                    labelDownload.Text = string.Format(ResStrings.str_Download, curSizeKo, totSizeKo);
                    Refresh();
                    Application.DoEvents();

                });

                if (currentSize == totalSize)
                {
                    progressBar.Value = 100;
                    double curSizeKo = currentSize / 1024;
                    double totSizeKo = totalSize / 1024;
                    labelDownload.Text = string.Format(ResStrings.str_Download, curSizeKo, totSizeKo);
                    Refresh();
                    System.Threading.Thread.Sleep(1000);
                    BeginInvoke((MethodInvoker)delegate { Close(); });
                }
            }
            catch
            {
            }
        }
       
    }
}
