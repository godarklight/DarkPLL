namespace DarkPLL;

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