using System;
using System.Drawing;
using System.Windows.Forms;
using System.Management;
using System.Threading;

namespace HDDLED
{
    public partial class InvisibleForm : Form
    {
        #region Global Variables
        NotifyIcon hddLedNotifyIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedThread;
        AboutForm aboutForm;
        #endregion

        #region Main Function
        public InvisibleForm()
        {
            InitializeComponent();

            // Load the .ico files into the Icons
            activeIcon = new Icon("Hard_Disk_Icon_Red.ico");
            idleIcon = new Icon("Hard_Disk_Icon.ico");

            // Create the notify icon and set the current icon as the idle icon
            hddLedNotifyIcon = new NotifyIcon();
            hddLedNotifyIcon.Icon = idleIcon;
            hddLedNotifyIcon.Visible = true;
            hddLedNotifyIcon.Text = "HDD LED v0.1";

            // Set up the context menu for the icon
            MenuItem progNameMenuItem = new MenuItem("About");
            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(progNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            hddLedNotifyIcon.ContextMenu = contextMenu;

            // Wire up the menu items
            progNameMenuItem.Click += ProgNameMenuItem_Click;
            quitMenuItem.Click += QuitMenuItem_Click;

            // Hide the form, don't need a window
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            // Start worker thread
            hddLedThread = new Thread(new ThreadStart(HddActivityThread));
            hddLedThread.Start();
        }
        #endregion

        #region Event handlers
        private void ProgNameMenuItem_Click(object sender, EventArgs e)
        {
            // If the AboutForm hasn't been created or is already disposed, create a new one
            if (aboutForm == null || aboutForm.IsDisposed)
                aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            // Clean up and close out program
            hddLedThread.Abort();
            hddLedNotifyIcon.Dispose();
            this.Close();
        }
        #endregion

        #region Threads
        public void HddActivityThread()
        {
            try
            {
                ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
                ManagementObjectCollection driveDataClassCollection;

                while (true)
                {
                    driveDataClassCollection = driveDataClass.GetInstances();
                    foreach(ManagementObject obj in driveDataClassCollection)
                    {
                        if(obj["Name"].ToString() == "_Total")
                        {
                            if (Convert.ToUInt64(obj["DiskBytesPerSec"]) > 0)
                            {
                                for(int i=0; i<5; i++)
                                {
                                    hddLedNotifyIcon.Icon = activeIcon;
                                    Thread.Sleep(60);
                                    hddLedNotifyIcon.Icon = idleIcon;
                                    Thread.Sleep(60);
                                }
                            }
                            else
                            {
                                hddLedNotifyIcon.Icon = idleIcon;
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            } catch(ThreadAbortException tbe)
            {

            }
        }
        #endregion
    }
}
