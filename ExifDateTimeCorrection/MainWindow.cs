using ExifLibrary;
using Serilog;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ExifDateTimeCorrection
{
    public partial class MainWindow : Form
    {
        // Date and Time separators
        char[] sepArray = { '_', '-', ' ', '.', ':' };
        private int totalFiles;
        private int totalProcess;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.InitialDirectory = this.sourceFolderTB.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.sourceFolderTB.Text = dialog.SelectedPath;
            }
        }

        private void destFolderBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.InitialDirectory = this.destFolderTB.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.destFolderTB.Text = dialog.SelectedPath;
            }
        }

        private void processBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.sourceFolderTB.Text) || String.IsNullOrEmpty(this.destFolderTB.Text))
            {
                MessageBox.Show("Source and Destination folders must be set");
                return;
            }
            if (String.Compare(this.sourceFolderTB.Text, this.destFolderTB.Text) == 0)
            {
                MessageBox.Show("Source and Destination folders must be different");
                return;
            }
            SaveConfig();
            ProcessState();
            //
            backgroundProcess.RunWorkerAsync();
        }

        private void SaveConfig()
        {
            //
            String setupFile = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            setupFile += ".json";
            //
            var config = new Config
            {
                SourceFolder = this.sourceFolderTB.Text,
                DestFolder = this.destFolderTB.Text
            };
            File.WriteAllText(setupFile, JsonSerializer.Serialize(config));
        }

        private void backgroundSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var allFiles = Directory.GetFiles(this.sourceFolderTB.Text, "*.jpg", SearchOption.AllDirectories);
            this.totalFiles = allFiles.Length;
            this.totalProcess = 0;
            int progress = 0;
            string time;
            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(this.destFolderTB.Text, fileName);
                //
                progress++;
                backgroundProcess.ReportProgress((progress * 100) / totalFiles, fileName);
                if (backgroundProcess.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //
                try
                {
                    DateTime? newDateTime = null;
                    // Load the image
                    ImageFile image = ImageFile.FromFile(file);
                    var originalDateTime = image.Properties.Get<ExifDateTime>(ExifTag.DateTimeOriginal);
                    //
                    var upperFile = fileName.ToUpper();
                    if (upperFile.StartsWith("WP_"))
                    {
                        newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 3);
                    }
                    else if (upperFile.StartsWith("SCREENSHOT_"))
                    {
                        newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 11);
                    }
                    else if (upperFile.StartsWith("IMG-"))
                    {
                        newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 4);
                    }
                    else if (upperFile.StartsWith("IMG_"))
                    {
                        newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 4);
                    }
                    else if (upperFile.Contains("_BURST"))
                    {
                        int start = upperFile.IndexOf("_BURST");
                        if (upperFile.Length > start + 6 + 8) // At least a Date
                            newDateTime = GetFormatted_DateTime(fileName, originalDateTime, start + 6);
                        else // Maybe the Date is at the beginning
                            newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 0);
                    }
                    else
                    {
                        newDateTime = GetFormatted_DateTime(fileName, originalDateTime, 0);
                    }
                    if (newDateTime == null)
                    {
                        Log.Logger.Error("Error : {0} - {1}", fileName, "Null DateTime");
                        continue;
                    }
                    // Change Exif Date Time infos
                    image.Properties.Set(ExifTag.DateTimeOriginal, newDateTime.Value);
                    image.Properties.Set(ExifTag.DateTimeDigitized, newDateTime.Value);
                    image.Properties.Set(ExifTag.DateTime, newDateTime.Value);
                    // Save changes
                    image.Save(destFile);
                    // Now, change File Date Time infos
                    File.SetCreationTime(destFile, (DateTime)newDateTime);
                    File.SetLastWriteTime(destFile, (DateTime)newDateTime);
                    File.SetLastAccessTime(destFile, (DateTime)newDateTime);
                    this.totalProcess++;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("Error : {0} - {1}", fileName, ex.Message);
                }
            }
        }

        private DateTime? GetFormatted_DateTime(string fileName, ExifDateTime originalDateTime, int dStart)
        {
            DateTime? newDateTime = null;
            bool noTime = false;
            // Date starts at dStart
            String data = fileName.Substring(dStart);
            (string date, int dLength) = GetSeparatedDate(data);
            if (!String.IsNullOrEmpty(date))
            {
                // Rest of the info is after the date
                data = fileName.Substring(dStart + dLength);
                var time = String.Empty;
                if (data.Length > 1)
                {
                    // Skip separator ?
                    int offset = 0;
                    String sep = data.Substring(0, 1);

                    if (sepArray.Contains(sep[0]))
                        offset = 1;
                    time = GetSeparatedTime(data.Substring(offset));
                }
                noTime = String.IsNullOrEmpty(time);
                if (noTime)
                {
                    time = "00_00_00";
                }
                try
                {
                    newDateTime = DateTime.ParseExact(date + "_" + time, "yyyyMMdd_HH_mm_ss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    newDateTime = null;
                }
            }
            if (originalDateTime != null)
            {
                if (newDateTime != null)
                {
                    if (newDateTime.Value.Date == originalDateTime.Value.Date)
                    {
                        if (noTime)
                        {
                            newDateTime = originalDateTime.Value;
                        }
                    }
                }
                else
                {
                    newDateTime = originalDateTime.Value;
                }
            }
            return newDateTime;
        }

        private void backgroundSearch_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.labelFileName.Text = e.UserState.ToString();
            this.processBar.Value = e.ProgressPercentage;
        }

        // File : WP_20190101_12_34_56_Pro.jpg
        // Format : 12_34_56_Pro.jpg
        private string GetSeparatedTime(string fileNameTime)
        {
            String hh = String.Empty;
            String mm = String.Empty;
            String ss = String.Empty;
            // Check if the file is in the correct format
            if (fileNameTime.Length >= 3)
            {
                String sep = fileNameTime.Substring(2, 1);
                if (sepArray.Contains(sep[0]) && (fileNameTime.Length >= 8))
                {
                    hh = fileNameTime.Substring(0, 2);
                    mm = fileNameTime.Substring(3, 2);
                    ss = fileNameTime.Substring(6, 2);
                }
                else if (fileNameTime.Length >= 6)
                {
                    hh = fileNameTime.Substring(0, 2);
                    mm = fileNameTime.Substring(2, 2);
                    ss = fileNameTime.Substring(4, 2);
                }
            }
            if (int.TryParse(hh, out int h) && int.TryParse(mm, out int m) && int.TryParse(ss, out int s))
            {
                if (h >= 0 && h <= 23 && m >= 0 && m <= 59 && s >= 0 && s <= 59)
                    return $"{hh}_{mm}_{ss}";
            }
            return String.Empty;
        }


        private (string date, int size) GetSeparatedDate(string fileName)
        {
            String date = String.Empty;
            int offset = 0;
            if (fileName.Length >= 8)
            {
                // With Separator ??
                String sep = fileName.Substring(4, 1);
                if (sepArray.Contains(sep[0]))
                    offset = 1;
                if (fileName.Length >= 8 + 2 * offset)
                {
                    String year = fileName.Substring(0, 4);
                    String month = fileName.Substring(4 + offset, 2);
                    String day = fileName.Substring(6 + 2 * offset, 2);
                    if (int.TryParse(year, out int y) && y >= 1900 && y <= 2100)
                    {
                        if (int.TryParse(month, out int m) && m >= 1 && m <= 12)
                        {
                            if (int.TryParse(day, out int d) && d >= 1 && d <= 31)
                            {
                                date = $"{year}{month}{day}";
                            }
                        }
                    }
                }
            }
            return (date, 8 + 2 * offset);
        }

        private void backgroundSearch_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            LoadState();
            this.processBar.Value = 0;
            Log.Logger.Information("Processed / Errors / Total : {0} / {1} / {2}", this.totalProcess, this.totalFiles - this.totalProcess, this.totalFiles);
            if (!e.Cancelled)
            {
                MessageBox.Show("Process completed", "End", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Logger.Information("Process Completed");
            }
            else
            {
                MessageBox.Show("Process Cancelled", "End", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.Logger.Information("Process Cancelled");
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            LoadConfig();
            LoadState();
        }

        private void LoadConfig()
        {
            String setupFile = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            setupFile += ".json";
            //
            if (File.Exists(setupFile))
            {
                var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(setupFile));
                if (config != null)
                {
                    this.sourceFolderTB.Text = config.SourceFolder;
                    this.destFolderTB.Text = config.DestFolder;
                }
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            if (backgroundProcess.IsBusy)
            {
                backgroundProcess.CancelAsync();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundProcess.IsBusy)
            {
                e.Cancel = true;
                backgroundProcess.CancelAsync();
            }
        }

        private void LoadState()
        {
            this.sourceFolderTB.Enabled = true;
            this.sourceFolderBtn.Enabled = true;
            this.destFolderTB.Enabled = true;
            this.destFolderBtn.Enabled = true;
            this.processBtn.Enabled = true;
            this.cancelBtn.Enabled = false;
        }

        private void ProcessState()
        {
            this.sourceFolderTB.Enabled = false;
            this.sourceFolderBtn.Enabled = false;
            this.destFolderTB.Enabled = false;
            this.destFolderBtn.Enabled = false;
            this.processBtn.Enabled = false;
            this.cancelBtn.Enabled = true;
        }
    }
}
