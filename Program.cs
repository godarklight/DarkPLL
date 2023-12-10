using System.Configuration.Assemblies;
using System.Numerics;
using System.Runtime.ConstrainedExecution;

namespace DarkPLL;

class Program
{
    static void Main(string[] args)
    {
        StreamWriter sw = new StreamWriter("output.csv");
        WavOutput wav = new WavOutput();
        VCO reference = new VCO();
        VCO pllVCO = new VCO();
        LowpassFilter lpf = new LowpassFilter();
        //PLLAnalog detector = new PLLAnalog();
        //PLLFlipFlop detector = new PLLFlipFlop();
        //PLLType1 detector = new PLLType1();
        //PLLType2 detector = new PLLType2();
        PLLPhaseFreqTracker detector = new PLLPhaseFreqTracker(reference, pllVCO);
        double pllVoltage = 0;
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step(1.0 / 10.0);
            double pllSignal = pllVCO.Step(pllVoltage);
            double phaseSignal = detector.Step(refSignal, pllSignal);
            //Analog
            //pllVoltage = lpf.Step(phaseSignal, 0.995);
            //Digital
            pllVoltage = lpf.Step(phaseSignal, 0.9999);
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVoltage * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
        wav.Close();
    }
}

class VCO
{
    public double phase = 0.0;
    public double Step(double tuning)
    {
        phase += tuning;
        phase = phase % Math.Tau;
        return Math.Cos(phase);
    }
}

//Your average decay IIR filter
class LowpassFilter
{
    private double offset = 0;
    public double Step(double input, double decay)
    {
        offset += (1.0 - decay) * input;
        //VCO lock range 119hz - 7639hz
        if (offset < 1.0 / 64.0)
        {
            offset = 1.0 / 64.0;
        }
        if (offset > 1.0)
        {
            offset = 1.0;
        }
        return offset;
    }
}

//Balanced mixer method
class PLLAnalog
{
    public double Step(double refSignal, double vcoSignal)
    {
        return refSignal * vcoSignal;
    }
}

//Analytical error
class PLLPhaseFreqTracker
{
    VCO reference;
    VCO vco;

    double lastRef = 0.0;
    double lastVco = 0.0;

    public PLLPhaseFreqTracker(VCO reference, VCO vco)
    {
        this.reference = reference;
        this.vco = vco;
    }

    public double Step(double refSignal, double vcoSignal)
    {
        //It's possible to recover the phase information with a hilbert transform, or a "slow" fft of the carrier.
        double refDelta = reference.phase - lastRef;
        double vcoDelta = vco.phase - lastVco;
        lastRef = reference.phase;
        lastVco = vco.phase;
        if (refDelta < -Math.PI)
        {
            refDelta += Math.Tau;
        }
        if (vcoDelta < -Math.PI)
        {
            vcoDelta += Math.Tau;
        }
        double freqError = refDelta - vcoDelta;
        double phaseError = reference.phase - vco.phase;
        if (phaseError > Math.PI)
        {
            phaseError -= Math.Tau;
        }
        if (phaseError < -Math.PI)
        {
            phaseError += Math.Tau;
        }
        //Freq needs to be the dominant term.
        return freqError + phaseError * 0.01;
    }
}

//The design I see online that almost work but doesn't
class PLLFlipFlop
{
    bool lastRefHigh;
    bool lastVcoHigh;
    double output = 0.0;
    public double Step(double refSignal, double vcoSignal)
    {
        bool refRising = refSignal > 0.0 && !lastRefHigh;
        lastRefHigh = refSignal > 0.0;
        bool vcoRising = vcoSignal > 0.0 && !lastVcoHigh;
        lastVcoHigh = vcoSignal > 0.0;
        if (refRising)
        {
            output = 1.0;
        }
        if (vcoRising)
        {
            output = -1.0;
        }
        if (lastRefHigh && lastVcoHigh)
        {
            output = 0.0;
        }
        return output;
    }
}

//CD4046B

class PLLType1
{
    //Exclusive or
    public double Step(double refSignal, double vcoSignal)
    {
        if (refSignal > 0.0 && vcoSignal > 0.0)
        {
            return -1.0;
        }
        if (refSignal < 0.0 && vcoSignal < 0.0)
        {
            return -1.0;
        }
        return 1.0;
    }
}

//Copied from CD4046B data sheet
class PLLType2
{
    int state = 1;
    bool lastRefHigh = false;
    bool lastVcoHigh = false;
    //Exclusive
    public double Step(double refSignal, double vcoSignal)
    {
        //I input reference signal
        //C comparator vco signal
        bool refHigh = refSignal > 0.0;
        bool vcoHigh = vcoSignal > 0.0;
        bool IRise = refHigh && !lastRefHigh;
        bool IFall = !refHigh && lastRefHigh;
        bool CRise = vcoHigh && !lastVcoHigh;
        bool CFall = !vcoHigh && lastVcoHigh;
        lastRefHigh = refHigh;
        lastVcoHigh = vcoHigh;
        if (state == 1)
        {
            if (CRise)
            {
                state = 2;
            }
            if (IRise)
            {
                state = 3;
            }
        }
        if (state == 2)
        {
            if (CFall)
            {
                state = 4;
            }
            if (IRise)
            {
                state = 6;
            }
        }
        if (state == 3)
        {
            if (CRise)
            {
                state = 6;
            }
            if (IFall)
            {
                state = 5;
            }
        }
        if (state == 4)
        {
            if (CRise)
            {
                state = 2;
            }
            if (IRise)
            {
                state = 8;
            }
        }
        if (state == 5)
        {
            if (CRise)
            {
                state = 7;
            }
            if (IRise)
            {
                state = 3;
            }
        }
        if (state == 6)
        {
            if (CFall)
            {
                state = 8;
            }
            if (IFall)
            {
                state = 7;
            }
        }
        if (state == 7)
        {
            if (CFall)
            {
                state = 1;
            }
            if (IRise)
            {
                state = 9;
            }
        }
        if (state == 8)
        {
            if (CRise)
            {
                state = 10;
            }
            if (IFall)
            {
                state = 1;
            }
        }
        if (state == 9)
        {
            if (CFall)
            {
                state = 3;
            }
            if (IFall)
            {
                state = 11;
            }
        }
        if (state == 10)
        {
            if (CFall)
            {
                state = 12;
            }
            if (IFall)
            {
                state = 2;
            }
        }
        if (state == 11)
        {
            if (CFall)
            {
                state = 5;
            }
            if (IRise)
            {
                state = 9;
            }
        }
        if (state == 12)
        {
            if (CRise)
            {
                state = 10;
            }
            if (IFall)
            {
                state = 4;
            }
        }
        if (state == 2 || state == 4 || state == 10 || state == 12)
        {
            return -1.0;
        }
        if (state == 3 || state == 5 || state == 9 || state == 11)
        {
            return 1.0;
        }
        return 0.0;
    }
}

class WavOutput
{
    FileStream fs = new FileStream("output.raw", FileMode.Create);

    public void WriteSample(double left, double right)
    {
        short shortL = (short)(left * 0.1 * short.MaxValue);
        short shortR = (short)(right * 0.1 * short.MaxValue);
        fs.WriteByte((byte)(shortL & 0xFF));
        fs.WriteByte((byte)((shortL >> 8) & 0xFF));
        fs.WriteByte((byte)(shortR & 0xFF));
        fs.WriteByte((byte)((shortR >> 8) & 0xFF));

    }

    public void Close()
    {
        fs.Close();
    }
}
