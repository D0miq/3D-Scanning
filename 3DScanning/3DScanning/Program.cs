namespace _3DScanning
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The <see cref="Program"/> class is the main class of the application and serves as an entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form());
        }
    }
}
