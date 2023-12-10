namespace DarkPLL;

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