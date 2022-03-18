using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Security.Cryptography;

public class NoiseBuffer : MonoBehaviour {

    public NoiseType noisestyle;
    public float noisezoom = 1f;
    public float noiseoffset = 0f;
    public float noiseturbulence = 0f;
    public int noiseperlinOctaves = 6;
    public double noisedisplacement = 4;
    public double noisefrequency = 2;
    int seed;
    int seed_offset;    

    int resolution;

    public List<BufferedNoiseArray> buffer = new List<BufferedNoiseArray>();
    public int maxbufferlength=10;
    public int currentbufferlength=0;

    MD5 md5hasher = MD5.Create();

    // Use this for initialization
    void Start () {
       	
	}
	
	// Update is called once per frame
	void Update () {
        currentbufferlength = buffer.Count;
        if (currentbufferlength<=maxbufferlength)
        {

        }
	}

    public void SetResolution(int newresolution)
    {
        resolution = newresolution;

        //you would have to clear any existing buffered noisemaps to avoid errors
    }

    public float[,] GetOrGenerate(int newseed, int newseed_offset)
    {
        string hashstring;

        //Debug.Log("Getting/Generating Noise: " + newseed + " " + newseed_offset);

        seed = newseed;
        seed_offset = newseed_offset;

        hashstring = GenerateHash();

        //Debug.Log("Hashstring: " + hashstring);

        //check whether noise has already been generated and buffered

        foreach (BufferedNoiseArray bna in buffer)
        {
            /*if (bna.hashstring==hashstring)
            {
                return bna.GetNoiseArray();
            }*/
        }

        //OK so we didn't find the noise in the buffer - create new one...

        ModuleBase moduleBase;
        
        switch (noisestyle)
        {
            case NoiseType.RidgedMultifractal:
                RidgedMultifractal rmf = new RidgedMultifractal();
                rmf.Seed = seed + seed_offset;
                moduleBase = rmf;                
                break;

            case NoiseType.Voronoi:
                // moduleBase = new Voronoi();                
                moduleBase = new Voronoi(noisefrequency, noisedisplacement, seed + seed_offset, false);                
                break;

            case NoiseType.Mix:
                Perlin perlin = new Perlin();
                perlin.Frequency = noisefrequency;
                perlin.OctaveCount = noiseperlinOctaves;
                perlin.Seed = seed + seed_offset;
                var rigged = new RidgedMultifractal();
                rigged.Seed = seed + seed_offset;
                rigged.Frequency = noisefrequency;
                rigged.OctaveCount = noiseperlinOctaves;
                moduleBase = new Add(perlin, rigged);                
                break;

            case NoiseType.FMT:
                Perlin perlin1 = new Perlin();
                perlin1.Frequency = noisefrequency*2.0f;
                perlin1.OctaveCount = noiseperlinOctaves;
                perlin1.Seed = seed + seed_offset;
                var rigged1 = new RidgedMultifractal();
                rigged1.Seed = seed + seed_offset + 1;
                rigged1.Frequency = noisefrequency*0.5f;
                rigged1.OctaveCount = noiseperlinOctaves;
                moduleBase = new Add(perlin1, rigged1);
                break;

            case NoiseType.Practice:
                var bill = new Billow();
                bill.Seed = seed + seed_offset;
                bill.Frequency = noisefrequency;
                moduleBase = new Turbulence(noiseturbulence / 10, bill);                

                break;

            default:
                var defPerlin = new Perlin();
                defPerlin.Seed = seed + seed_offset;
                defPerlin.OctaveCount = noiseperlinOctaves;
                moduleBase = defPerlin;                
                break;
        }

        Noise2D noisemap;

        noisemap = new Noise2D(resolution,moduleBase);

        //Debug.Log("Generating Noise " + Time.realtimeSinceStartup);
        noisemap.GeneratePlanar(
            noiseoffset + -1 * 1 / noisezoom,
            noiseoffset + noiseoffset + 1 * 1 / noisezoom,
            noiseoffset + -1 * 1 / noisezoom,
            noiseoffset + 1 * 1 / noisezoom, true);        
        //Debug.Log("Finished Generating Noise " + Time.realtimeSinceStartup);

        //Debug.Log("Getting Noise " + Time.realtimeSinceStartup);
        float[,] noise = noisemap.GetData(isNormalized: true);
        //Debug.Log("Finished Getting Noise " + Time.realtimeSinceStartup);

        if (buffer.Count==maxbufferlength)
        {
            buffer.RemoveAt(0); //delete oldest buffered noise
        }

        buffer.Add(new BufferedNoiseArray(hashstring, noise));
        
        return noise;

    }

    string GenerateHash()
    {
        //make byte array capturing settings and seed.

        byte[] bytes;
        bytes = ConcatenateByteArrays(System.BitConverter.GetBytes((int)noisestyle), System.BitConverter.GetBytes(seed_offset));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(seed_offset));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noisezoom));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noiseoffset));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noiseturbulence));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noiseperlinOctaves));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noisedisplacement));
        bytes = ConcatenateByteArrays(bytes, System.BitConverter.GetBytes(noisefrequency));


        //hash        
        byte[] hashbytes = md5hasher.ComputeHash(bytes);
        string hashstring = System.Convert.ToBase64String(hashbytes);
        return hashstring;
    }

    public static byte[] ConcatenateByteArrays(byte[] first, byte[] second)
    {
        byte[] ret = new byte[first.Length + second.Length];
        System.Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        System.Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }
}

[System.Serializable]

public class BufferedNoiseArray
{
    public string hashstring;
    float[,] noisearray;

    public BufferedNoiseArray(string newhashstring,float[,] newnoisearray)
    {
        hashstring = newhashstring;
        noisearray = newnoisearray;
    }

    public float[,] GetNoiseArray()
    {
        return noisearray;
    }
}

