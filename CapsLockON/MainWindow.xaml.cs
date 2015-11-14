﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;          // -- For Threading
using System.Windows.Forms;      // -- For NotifyIcon Class
using System.Drawing;            // -- For Icon Class


/// <summary>
/// This is a tray application app.
/// It will sit in the tray with 2 icons - 1 for capslock and 1 for numlock.
/// The icons will switch between ON and OFF indicating if the buttons are toggled.
/// .
/// The Program runs in an infinite while-loop and is on a 5 second 'cooldown' (threadSleep)
/// .
/// Made because my new laptop DOESN'T have an indication wether those are on or off. Seriously. Why? -.-
/// 
/// You can add the program to your startup menu, so it starts up when you turn on your PC
/// </summary>

namespace CapsLockON
{
    public partial class MainWindow : Window
    {
        #region Global variables
        NotifyIcon capsNumIcon;
        Icon capsONnumON;
        Icon capsOFFnumOFF;
        Icon capsONnumOFF;
        Icon capsOFFnumON;

        Thread updateIconThread;
        #endregion

        #region Initialize and create - Point of Entry
        public MainWindow()
        {
            //No WPF window -This is a notification tray application
            //Will minimize and hide from taskbar
            InitializeComponent();
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();

            //Load icons into objecs
            capsOFFnumOFF = new Icon("capsOFFnumOFF.ico");
            capsONnumON = new Icon("capsONnumON.ico");
            capsONnumOFF = new Icon("capsONnumOFF.ico");
            capsOFFnumON = new Icon("capsOFFnumON.ico");

            //Create notifyIcons and set their visibility to true.
            capsNumIcon = new NotifyIcon();
            capsNumIcon.Visible = true;

            //Create all contextMenu items and add them to tray Icons
            MenuItem quitMenuItem = new MenuItem("Quit");
            MenuItem infoMenuItem = new MenuItem("CapsLockON! -- Release Version 1.0.0");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(quitMenuItem);
            contextMenu.MenuItems.Add(infoMenuItem);

            capsNumIcon.ContextMenu = contextMenu;

            //Reference to CLick Method - Will Quit the program
            quitMenuItem.Click += QuitMenuItem_Click;

            //Start Thread
            updateIconThread = new Thread(new ThreadStart(updateToggle));
            updateIconThread.SetApartmentState(ApartmentState.STA); //This because of exception, don't know why.
            updateIconThread.Start();
        }
        #endregion

        #region Menu Item Even handlers - Only 1 as of right now
        //Clean up and close the application on click of 'Quit' MenuItem
        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            //Clean - GARBAGE TRUCK, TRUCK, TRUCK! OHNO! I'LL TAKE YOU FOR A RIDE, IN MY GARBAGE TRUCK!
            updateIconThread.Abort();
            capsNumIcon.Dispose();

            //Close
            this.Close();
        }
        #endregion

        #region Thread Functions - Only 1 as of right now
        private void updateToggle()
        {
            try // Try so we can catch an threadabort exception and do some final cleanup. (plottwist, there is none)
            {
                while (true) //Infinite MAIN LOOP so we can make the thread go FOREVER.
                {
                    //Caps ON - Num ON
                    if (Keyboard.IsKeyToggled(Key.CapsLock) && Keyboard.IsKeyToggled(Key.NumLock))
                    {
                        capsNumIcon.Icon = capsONnumON;
                    }

                    //Caps OFF - Num OFF
                    else if (!Keyboard.IsKeyToggled(Key.CapsLock) && !Keyboard.IsKeyToggled(Key.NumLock))
                    {
                        capsNumIcon.Icon = capsOFFnumOFF;
                    }

                    //Caps ON - Num OFF
                    else if (Keyboard.IsKeyToggled(Key.CapsLock) && !Keyboard.IsKeyToggled(Key.NumLock))
                    {
                        capsNumIcon.Icon = capsONnumOFF;
                    }

                    //Caps OFF - Num ON
                    else if (!Keyboard.IsKeyToggled(Key.CapsLock) && Keyboard.IsKeyToggled(Key.NumLock))
                    {
                        capsNumIcon.Icon = capsOFFnumON;
                    }

                    Thread.Sleep(1000); //Make the loop wait - We dont want an infinite crazy dog farm loop. Or do we?
                }
            }
            catch (ThreadAbortException tbe)
            {
                //Thread was aborted
                //Nothing really to clean, but points for good try/catch style
            }
        }
        #endregion
    }
}
