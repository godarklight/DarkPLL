using System.Numerics;

namespace DarkPLL;

class VCO
{
    public double phase = 0.0;
    public double tuning = 1.0 / 64.0;
    public double Step()
    {
        phase += tuning;
        phase = phase % Math.Tau;
        return Math.Cos(phase);
    }

    public Complex StepComplex()
    {
        Step();
        return new Complex(Math.Cos(phase), Math.Sin(phase));
    }
}