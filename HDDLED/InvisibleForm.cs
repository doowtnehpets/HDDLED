using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;

namespace HDDLED
{
    public partial class InvisibleForm : Form
    {
        #region Global Variables
        Random random;
        NotifyIcon hddLedNotifyIcon;
        Icon activeIcon;
        Icon idleIcon;
        Thread hddLedThread;
        #endregion

        public InvisibleForm()
        {
            InitializeComponent();

            random = new Random();

            // Load the .ico files into the Icons
            activeIcon = new Icon("Hard_Disk_Icon_Red.ico");
            idleIcon = new Icon("Hard_Disk_Icon.ico");

            // Create the notify icon and set the current icon as the idle icon
            hddLedNotifyIcon = new NotifyIcon();
            hddLedNotifyIcon.Icon = idleIcon;
            hddLedNotifyIcon.Visible = true;

            // Set up the context menu for the icon
            MenuItem progNameMenuItem = new MenuItem("HDD LED v0.1");
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

        private void ProgNameMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("HDD LED v0.1\n\rby Stephen Wood",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            hddLedThread.Abort();
            hddLedNotifyIcon.Dispose();
            this.Close();
        }

        public void HddActivityThread()
        {
            try
            {
                ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

                while (true)
                {
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach(ManagementObject obj in driveDataClassCollection)
                    {
                        if(obj["Name"].ToString() == "_Total")
                        {
                            if (Convert.ToUInt64(obj["DiskBytesPerSec"]) > 0)
                            {
                                for(int i=0; i<10; i++)
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
    }
}
