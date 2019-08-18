using Existek.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Existek.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Existek
{

    public partial class MainWindow : Window
    {
        [DllImport("User32.dll")]
        static extern IntPtr WindowFromPoint(POINT p);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetWindowText(IntPtr hwnd, StringBuilder lpString, uint cch);

        [DllImport("User32.dll")]
        static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("User32.dll")]
        static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT p);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, uint nMaxCount);

        [DllImport("User32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, uint nSize);

        MainViewModel MVM;
        public MainWindow()
        {
            InitializeComponent();
            MVM= new MainViewModel();
            DataContext = MVM;
            MouseHook.MouseAction += new EventHandler<MousePosArgs>(OnMouseEvent);
        }


        bool found = false;
        private dto GetProcessInfo(IntPtr hWnd)
        {
            uint processID = 0;
            Process process = Process.GetProcessById((int)processID);

            dto temp = new dto();
            temp.name = process.MainWindowTitle;
            temp.value = process.ProcessName;


            return temp;

        }

        bool mouseLBtnDown = false;
        bool mouseRBtnDown = false;
        private int getWindow = 0;
        private string w;
        void OnMouseEvent(object sender, MousePosArgs e)
        {
            if (IsClosing) return;

            if (e.Message == MousePosArgs.MouseMessages.WM_LBUTTONDOWN)
            {
                mouseLBtnDown = true;
                found = false;
            }
            if (e.Message == MousePosArgs.MouseMessages.WM_LBUTTONUP)
            {
                mouseLBtnDown = false;
                dto temp = new dto();
                IntPtr hwnd = WindowFromPoint(new POINT(e.X, e.Y));
                w = "";
                if (hwnd.ToInt64() > 0)
                {
                    w += " Caption: " + GetCaptionOfWindow(hwnd);
                    w += " ClassName: " + GetClassNameOfWindow(hwnd);
                    temp.name = GetClassNameOfWindow(hwnd);
                    temp.value = GetCaptionOfWindow(hwnd);
                    temp.flag = true;
                }
                if (temp.flag) MVM.MouseUpCommand.Execute(temp);
                found = true;
            }
            if (e.Message == MousePosArgs.MouseMessages.WM_RBUTTONDOWN)
            {
                mouseRBtnDown = true;
                found = true;
            }
            if (e.Message == MousePosArgs.MouseMessages.WM_RBUTTONUP)
            {
                mouseRBtnDown = false;
                found = true;
            }

           
        }


        private string GetCaptionOfWindow(IntPtr hwnd)
        {
            string caption = "";
            StringBuilder windowText = null;
            try
            {
                uint max_length = GetWindowTextLength(hwnd);
                windowText = new StringBuilder("", (int)(max_length + 5));
                GetWindowText(hwnd, windowText, max_length + 2);

                if (!String.IsNullOrEmpty(windowText.ToString()) && !String.IsNullOrWhiteSpace(windowText.ToString()))
                    caption = windowText.ToString();
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            finally
            {
                windowText = null;
            }
            return caption;
        }

        private string GetClassNameOfWindow(IntPtr hwnd)
        {
            string className = "";
            StringBuilder classText = null;
            try
            {
                uint cls_max_length = 1000;
                classText = new StringBuilder("", (int)(cls_max_length + 5));
                GetClassName(hwnd, classText, cls_max_length + 2);

                if (!String.IsNullOrEmpty(classText.ToString()) && !String.IsNullOrWhiteSpace(classText.ToString()))
                    className = classText.ToString();
            }
            catch (Exception ex)
            {
                className = ex.Message;
            }
            finally
            {
                classText = null;
            }
            return className;
        }

        private bool IsClosing = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsClosing = true;

        }


    }
}
