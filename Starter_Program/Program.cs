using System;
using System.Windows.Forms;
using EnerlifeCN;

namespace ConsoleApplication1
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var mainProgram = new ProgramIModule();
            mainProgram.PreInstall();
            mainProgram.Install();
            mainProgram.CreateMenu(null);
            mainProgram.Run();
            Application.Run();
        }
    }
}