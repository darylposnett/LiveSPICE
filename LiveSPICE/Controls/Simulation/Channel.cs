﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
using ComputerAlgebra;
using ComputerAlgebra.LinqCompiler;

namespace LiveSPICE
{
    /// <summary>
    /// Base channel type.
    /// </summary>
    public abstract class Channel : INotifyPropertyChanged
    {
        private string name = "";
        public string Name { get { return name; } set { name = value; NotifyChanged("Name"); } }

        private double v0dB = 1.414;
        public double V0dB { get { return v0dB; } set { v0dB = value; NotifyChanged("V0dB"); } }

        private Brush signalStatus = Brushes.Transparent;
        public Brush SignalStatus { get { return signalStatus; } set { signalStatus = value; NotifyChanged("SignalStatus"); } }
        
        // INotifyPropertyChanged.
        protected void NotifyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Channel of audio data.
    /// </summary>
    public class InputChannel : Channel
    {
        private int index = 0;
        public int Index { get { return index; } }

        public InputChannel(int Index) { index = Index; }
    }

    /// <summary>
    /// Channel generated by a signal expression.
    /// </summary>
    public class SignalChannel : Channel
    {
        private Func<double, double> signal;

        private double[] buffer = null;
        public double[] Buffer(int Count, double t, double dt)
        {
            if (buffer == null || buffer.Length != Count)
                buffer = new double[Count];
            for (int i = 0; i < Count; ++i, t += dt)
                buffer[i] = signal(t);
            return buffer;
        }
        
        public SignalChannel(ComputerAlgebra.Expression Signal)
        {
            signal = Signal.Compile<Func<double, double>>(Circuit.Component.t);
        }
    }

    /// <summary>
    /// Output audio channel.
    /// </summary>
    public class OutputChannel : Channel
    {
        private int index = 0;
        public int Index { get { return index; } }

        private ComputerAlgebra.Expression signal = 0;
        public ComputerAlgebra.Expression Signal { get { return signal; } set { signal = value; NotifyChanged("Signal"); } }

        public OutputChannel(int Index) { index = Index; }
    }
}
