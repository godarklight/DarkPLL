//CD4046B Type 1

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