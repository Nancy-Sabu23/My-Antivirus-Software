using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace MyAntivirusSoftware
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       
        private void btnStartScan_Click(object sender, EventArgs e)
        { 
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedDirectory = folderDialog.SelectedPath;

                // Update progress bar
                progressBarScan.Value = 0;
                progressBarScan.Maximum = Directory.GetFiles(selectedDirectory, "*", SearchOption.AllDirectories).Length;

                // Get all files to scan
                string[] filesToScan = Directory.GetFiles(selectedDirectory, "*", SearchOption.AllDirectories);

                foreach (string file in filesToScan)
                {
                    // Update the progress bar as files are scanned
                    progressBarScan.Value++;

                    // Check for malware
                    if (IsMalicious(file))
                    {
                        lstScanResults.Items.Add($"Malicious file detected: {file}");
                    }
                }

                // Update status
                txtStatus.Text = "Scan Complete!";
            }
        }
    

        private void progressBarScan_Click(object sender, EventArgs e)
        {

        }

        private void txtScanResult_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private FileSystemWatcher fileWatcher;

        private void InitializeFileWatcher(string pathToMonitor)
        {
            fileWatcher = new FileSystemWatcher(pathToMonitor)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };
            fileWatcher.Created += OnFileCreated;
            fileWatcher.Changed += OnFileChanged;
            fileWatcher.Deleted += OnFileDeleted;
            fileWatcher.EnableRaisingEvents = true;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Scan newly created files
            ScanFile(e.FullPath);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Scan changed files
            ScanFile(e.FullPath);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            // Notify file deletion
            lstScanResults.Items.Add($"File deleted: {e.FullPath}");
        }

        private void ScanFile(string filePath)
        {
            if (IsMalicious(filePath))
            {
                lstScanResults.Items.Add($"Malicious file detected: {filePath}");
            }
        }
        public string GetFileHash(string filePath)
    {
        // Create an MD5 hash object
        using (var md5 = MD5.Create())
        {
            // Open the file to read its contents
            using (var stream = File.OpenRead(filePath))
            {
                // Compute the hash of the file's content
                byte[] hash = md5.ComputeHash(stream);

                // Convert the byte array to a string (hex format)
                return BitConverter.ToString(hash).Replace("-", "").ToLower();  // Removing hyphens for easier comparison
            }
        }
    }
        public bool IsMalicious(string filePath)
        {
            // List of known malware hashes (replace with real malware hashes or signatures)
            List<string> malwareHashes = new List<string>
    {
        "e99a18c428cb38d5f260853678922e03", // Example MD5 hash
        "d41d8cd98f00b204e9800998ecf8427e"  // Another example MD5 hash
    };

            try
            {
                // Get the hash of the file being scanned
                string fileHash = GetFileHash(filePath);

                // Check if the file's hash matches any known malware hashes
                return malwareHashes.Contains(fileHash);
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., file access issues)
                MessageBox.Show($"Error reading file {filePath}: {ex.Message}");
                return false; // Assume the file is safe if there's an error
            }
        }
        public void DeleteMaliciousFile(string filePath)
        {
            try
            {
                // Check if file is in use (locked by another process)
                if (IsFileLocked(filePath))
                {
                    MessageBox.Show($"File is currently in use and cannot be deleted: {filePath}");
                    return;
                }

                // Check if the file is marked as read-only and remove the read-only attribute
                if (IsReadOnly(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);  // Remove read-only flag
                }

                // Attempt to delete the file
                File.Delete(filePath);
                MessageBox.Show($"File deleted successfully: {filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Permission denied to delete the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}");
            }
        }


    }
}
