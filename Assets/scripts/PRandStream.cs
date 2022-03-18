using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRandStream
{
    //uses a Linear Congruential Random Number Generator to keep reproducible control over random numbers
    //used to generate spatial views, and allow mulitple independent streams of random numbers to be used
    //https://en.wikipedia.org/wiki/Linear_congruential_generator
    //http://www.habrador.com/tutorials/generate-random-numbers/1-generate-random-numbers-csharp/

    //multiplier
    uint a = 1103515245;
    //increment
    uint c = 622450;
    //modulus m (which is also the maximum possible random value)
    uint m = (uint)Mathf.Pow(2f, 31f);
    public uint seed;

    public PRandStream()
    {

    }

    public PRandStream(uint newseed)
    {
        seed = newseed;
    }

    public void SetSeed(uint newseed)
    {
        seed = newseed;
    }

    public uint  GetSeed()
    {
        return seed;
    }

    public uint Next()
    {
        //return next random value as integer
        seed = (a * seed + c) % m;
        return seed;
    }

    public float Value()
    {
        //return next random value as float between 0 and 1;
        return (float)Next() / (float)m;
    }

    public int Range(int min, int max)
    {
        //return next random value as integer in range min [inclusive]:max [exclusive]
        return Mathf.FloorToInt(Value() * (max - min) + min);
    }

    public float Range(float min, float max)
    {
        //return next random value as integer in range min [inclusive]:max [exclusive]
        return Value() * (max - min) + min;
    }

    public int RangeWithSkip(int min, int max, int skip)
    {
        //return a random number from a range avoiding the integer specified by skip, min [inclusive] max [exclusive]        
        int r = Range(min, max - 1);
        if (r < skip)
        {
            return r;
        }
        else
        {
            return r + 1;
        }
    }
    public int RangeWithListSkip(int min, int max, List<int> skip)
    {
        //return a random number from a range avoiding the integers in list skip, min [inclusive] max [exclusive]      
        int r;
        List<int> fullList = new List<int>(); //create list of all possible values
        for (int i = 0; i<max; i++)
        {
            fullList.Add(i);
            //Debug.Log("full list" + i);
        }

        //remove values you wish to skip
        IEnumerable<int> okList = fullList.Except(skip); // should return list of ints between min and ndistinctelements without values from skip
        //Debug.Log("full list" + fullList.Count);
        //Debug.Log("skip list" + skip.Count);
        //Debug.Log("ok list" + okList.Count());
        if (okList.Count()>0)
        {
            if (okList.Count() == 1)
            {
                r = okList.ElementAt(0);
            }
            else
            {
                //choose 1 value of remaining
                r = okList.ElementAt(Range(min, okList.Count()));
            }
            
        }
        else
        {
            //if your skip list is as big as the num elements
            Debug.Log("not enough distinct elements");
            r = Range(min, max - 1);
        }
        

        return r;

        /*
        int r = Range(min, max - 1);
        
        bool finding = true;

        if (skip.Contains(r))
        {
            return r + 1;
        }
        else
        {
            return r;
        }
        */
    }

}

