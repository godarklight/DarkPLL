namespace DarkPLL;

//Balanced mixer method
class PLLAnalog
{
    public double Step(double refSignal, double vcoSignal)
    {
        return refSignal * vcoSignal;
    }
}