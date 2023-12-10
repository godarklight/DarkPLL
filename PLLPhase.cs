//This uses phase information from the reference.
//For this to work we need complex valued signals, so a hilbert transform is required for the incoming reference signal.

//When the only tool you have is a hammer every nail looks like a thumb.

using System.Numerics;

namespace DarkPLL;

class PLLPhase
{
    Complex lastRef = 0.0;
    Complex lastVco = 0.0;

    public double Step(Complex refSignal, Complex vcoSignal)
    {
        double refDelta = refSignal.Phase - lastRef.Phase;
        double vcoDelta = vcoSignal.Phase - lastVco.Phase;
        if (refDelta > Math.PI)
        {
            refDelta -= Math.Tau;
        }
        if (refDelta < -Math.PI)
        {
            refDelta += Math.Tau;
        }
        if (vcoDelta > Math.PI)
        {
            vcoDelta -= Math.Tau;
        }
        if (vcoDelta < -Math.PI)
        {
            vcoDelta += Math.Tau;
        }
        double phaseError = refSignal.Phase - vcoSignal.Phase;
        lastRef = refSignal;
        lastVco = vcoSignal;
        double freqError = refDelta - vcoDelta;
        if (phaseError > Math.PI)
        {
            phaseError -= Math.Tau;
        }
        if (phaseError < -Math.PI)
        {
            phaseError += Math.Tau;
        }
        //Freq needs to be the dominant term.
        return freqError + 0.01 * phaseError;
    }
}