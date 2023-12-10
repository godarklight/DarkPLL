using System.Numerics;

namespace DarkPLL;

class Program
{
    static void Main(string[] args)
    {
        TestAnalog();
        TestFlipFlop();
        TestType1();
        TestType2();
        TestDirect();
        TestPhase();
    }

    static void TestAnalog()
    {
        StreamWriter sw = new StreamWriter("analog.csv");
        WavOutput wav = new WavOutput("analog.raw");
        LowpassFilter lpf = new LowpassFilter();
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLAnalog detector = new PLLAnalog();
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step();
            double pllSignal = pllVCO.Step();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            double smoothed = lpf.Step(phaseSignal, 0.995);
            pllVCO.tuning = smoothed;
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
    }

    static void TestFlipFlop()
    {
        StreamWriter sw = new StreamWriter("flipflop.csv");
        WavOutput wav = new WavOutput("flipflop.raw");
        LowpassFilter lpf = new LowpassFilter();
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLFlipFlop detector = new PLLFlipFlop();
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step();
            double pllSignal = pllVCO.Step();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            double smoothed = lpf.Step(phaseSignal, 0.999);
            pllVCO.tuning = smoothed;
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
    }

    static void TestType1()
    {
        StreamWriter sw = new StreamWriter("type1.csv");
        WavOutput wav = new WavOutput("type1.raw");
        LowpassFilter lpf = new LowpassFilter();
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLType1 detector = new PLLType1();
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step();
            double pllSignal = pllVCO.Step();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            double smoothed = lpf.Step(phaseSignal, 0.9985);
            pllVCO.tuning = smoothed;
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
    }

    static void TestType2()
    {
        StreamWriter sw = new StreamWriter("type2.csv");
        WavOutput wav = new WavOutput("type2.raw");
        LowpassFilterIntegrator lpfi = new LowpassFilterIntegrator();
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLType2 detector = new PLLType2();
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step();
            double pllSignal = pllVCO.Step();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            double smoothed = lpfi.Step(phaseSignal, 0.99995);
            pllVCO.tuning = smoothed;
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
    }

    static void TestDirect()
    {
        StreamWriter sw = new StreamWriter("direct.csv");
        WavOutput wav = new WavOutput("direct.raw");
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLDirect detector = new PLLDirect(pllVCO);
        for (int i = 0; i < 480000; i++)
        {
            double refSignal = reference.Step();
            double pllSignal = pllVCO.Step();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            pllVCO.tuning = phaseSignal;
            sw.WriteLine($"{i},{refSignal},{pllSignal},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal, pllSignal);
        }
    }

    static void TestPhase()
    {
        StreamWriter sw = new StreamWriter("phase.csv");
        WavOutput wav = new WavOutput("phase.raw");
        LowpassFilterIntegrator lpfi = new LowpassFilterIntegrator();
        VCO reference = new VCO();
        reference.tuning = 1.0 / 10.0;
        VCO pllVCO = new VCO();
        PLLPhase detector = new PLLPhase();

        //We need to run FFT's to recover the phase information
        Complex[] referenceChunkIn = new Complex[1024];
        Complex[] referenceChunk = new Complex[1024];
        int referencePos = 512;

        //FFT has a delay...
        for (int i = 0; i < 1024; i++)
        {
            referenceChunkIn[i] = reference.Step();
        }

        for (int i = 0; i < 480000; i++)
        {
            //Hilbert on reference for no cheating
            if (referencePos == 512)
            {
                referencePos = 0;
                for (int j = 0; j < 512; j++)
                {
                    referenceChunkIn[j] = referenceChunkIn[j + 512];
                    referenceChunkIn[j + 512] = reference.Step();
                }
                Complex[] fft = FFT.CalcFFT(referenceChunkIn);
                //Hilbert. Leave DC/Nyquist alone, Positive = Positive * 2, Negative = 0
                for (int k = 1; k < 512; k++)
                {
                    fft[k] = fft[k] * 2.0;
                }
                for (int k = 513; k < 1024; k++)
                {
                    fft[k] = 0.0;
                }
                //This leaves the analytical signal
                referenceChunk = FFT.CalcIFFT(fft);
            }

            //I have a bug in my FFT library - it spins backwards.
            Complex refSignal = Complex.Conjugate(referenceChunk[256 + referencePos++]);
            Complex pllSignal = pllVCO.StepComplex();
            double phaseSignal = detector.Step(refSignal, pllSignal);
            double smoothed = lpfi.Step(phaseSignal, 0.99);
            pllVCO.tuning = smoothed;
            sw.WriteLine($"{i},{refSignal.Real},{pllSignal.Real},{phaseSignal * 0.2},{pllVCO.tuning * 10.0}");
            wav.WriteSample(refSignal.Real, pllSignal.Real);
        }
    }

}

//Best values: Analog 0.995
//Flip flop: 0.999 LPF
//Type 1: 0.9985 LPF
//Type 2: 0.99995 LPF-I
//PhaseFreq .99 LPF
//Direct: Disabled