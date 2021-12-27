using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Synthesis;

namespace BenchMark
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SpeechSynthesizer debugger;

        public MainWindow()
        {
            InitializeComponent();

            debugger = new SpeechSynthesizer();
            GetCPUTemperature();
            GetCPUVoltage();
            GetRAMFrequency();
            GetGPUFrequency();

        }

        private void GetCPUTemperature()
        {
            double temperature = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject obj in searcher.Get())
            {
                temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                // Convert the value to celsius degrees
                temperature = (temperature - 2732) / 10.0;
            }
            cpuTemp.Text = temperature.ToString() + " ℃";

        }

        private void GetCPUVoltage()
        {
            ManagementClass mgt = new ManagementClass("Win32_Processor");
            ManagementObjectCollection procs = mgt.GetInstances();
            string voltage = "0 mV";
            foreach (ManagementObject item in procs)
            {
                //debugger.Speak(item.Properties["CurrentVoltage"].Value.ToString());
                voltage = item.Properties["CurrentVoltage"].Value.ToString();
            }
            cpuVolt.Text = voltage.ToString() + " mV";
        }

        private void GetRAMFrequency()
        {
            ManagementObjectSearcher search = new ManagementObjectSearcher("Select * From Win32_PhysicalMemory");

            foreach (ManagementObject ram in search.Get())
            {
                ramFreq.Text = Convert.ToDouble(ram.GetPropertyValue("MinVoltage")) + "MHz";
            }
        }

        private void GetGPUFrequency()
        {
            ManagementObjectSearcher search = new ManagementObjectSearcher("Select * From Win32_VideoController");

            foreach (ManagementObject gpu in search.Get())
            {
                gpuFreq.Text = Convert.ToDouble(gpu.GetPropertyValue("CurrentRefreshRate")) + "MHz";
            }
        }

    }

    class UpdateVisitor: IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse((IVisitor)this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept((IVisitor)this);
        }

        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }

    }

}
