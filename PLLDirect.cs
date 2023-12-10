//Frequency counting approach followed by a phase error detector.
using System.Numerics;

namespace DarkPLL;

class PLLDirect
{
    VCO vco;

    double lastRef = 0.0;
    double lastError = 0.0;
    bool lockState = false;
    bool countState = false;
    int countSamples = 0;
    double frequency = 1.0 / 64.0;

    //Setting the VCO phase isn't cheating as we are trying to sync an external reference to our internal digital VCO.
    public PLLDirect(VCO vco)
    {
        this.vco = vco;
    }

    public double Step(double refSignal, double vcoSignal)
    {
        bool risingEdge = lastRef < 0.0 && refSignal > 0.0;
        double refDelta = refSignal - lastRef;
        lastRef = refSignal;
        if (lockState)
        {
            double error = refSignal - vcoSignal;
            //This checks if we are leading or lagging.
            //If the reference is more positive on rising edge, VCO is lagging
            //If the reference is more positive on the falling edge, VCO is leading.
            if (refDelta < 0.0)
            {
                error = -error;
            }
            //Integrator Term
            frequency += 0.001 * error;
            //Derivative Term - damping integrator.
            double errorDelta = error - lastError;
            frequency += 0.01 * errorDelta;
            vco.tuning = frequency;
            lastError = error;
        }
        if (countState)
        {
            if (risingEdge)
            {
                frequency = Math.Tau / (double)countSamples;
                vco.tuning = frequency;
                vco.phase = Math.Tau * 0.75;
                lockState = true;
                countState = false;
                countSamples = 0;
            }
            else
            {
                countSamples++;
            }
        }
        if (!lockState && !countState)
        {
            if (risingEdge)
            {
                countState = true;
                countSamples = 0;
            }
        }
        return frequency;
    }
}