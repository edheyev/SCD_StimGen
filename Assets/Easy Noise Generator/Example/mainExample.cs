using UnityEngine;
using System.Collections.Generic;
using System;
public class mainExample : MonoBehaviour
{
	public Material material;
	public static Noise terrainNoise;
	
	public Material material2;
	public static Noise colourNoise;
	
	public Material material3;
	public static Noise marbleNoise;
	
	public Material material4;
	public static Noise woodNoise;
	
	public Material material5;
	public static Noise perlinNoise;
	
	public Material material6;
	public static Noise turbulenceNoise;
	
	int width,height;
	void Start ()
	{
		//Single-channel Turbulence3d heightmap (4x Fractal)
		terrainNoise = new Noise();	
		terrainNoise.addChannel(new Channel("Height",Algorithm.Turbulence3d,150.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth).setFractal(4,2.0f,0.5f));
				
		//Three Simplex channels for Red, Green and Blueingle-channel Turbulence3d heightmap (4x Fractal)
		colourNoise = new Noise();
		colourNoise.addChannel(new Channel("Red",Algorithm.Simplex2d,250.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		colourNoise.addChannel(new Channel("Green",Algorithm.Simplex2d,250.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		colourNoise.addChannel(new Channel("Blue",Algorithm.Simplex2d,250.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		
		//Single-channel Marble noise
		marbleNoise = new Noise();	
		marbleNoise.addChannel(new Channel("Color",Algorithm.Marble3d,50.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		
		//Single-channel Wood noise
		woodNoise = new Noise();	
		woodNoise.addChannel(new Channel("Color",Algorithm.Wood3d,250.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		
		//Single-channel Perlin noise
		perlinNoise = new Noise();	
		perlinNoise.addChannel(new Channel("Color",Algorithm.Perlin3d,75.0f,NoiseStyle.Second,0.0f,1.0f,Edge.Smooth).setFractal(4,2.0f,0.5f));
		
		//Single-channel Turbulence3d noise
		turbulenceNoise = new Noise();	
		turbulenceNoise.addChannel(new Channel("Color",Algorithm.Turbulence3d,75.0f,NoiseStyle.Linear,0.0f,1.0f,Edge.Smooth));
		
		
		width = 700;
		height = 700;
		Texture2D texture1 = new Texture2D(width,height, TextureFormat.ARGB32, false);	
		Texture2D texture2 = new Texture2D(width,height, TextureFormat.ARGB32, false);	
		Texture2D texture3 = new Texture2D(width,height, TextureFormat.ARGB32, false);	
		Texture2D texture4 = new Texture2D(width,height, TextureFormat.ARGB32, false);		
		Texture2D texture5 = new Texture2D(width,height, TextureFormat.ARGB32, false);		
		Texture2D texture6 = new Texture2D(width,height, TextureFormat.ARGB32, false);	
		
		for(int x=0;x<width;x++)
		{for(int y=0;y<height;y++)
		{		
				Vector3 vector = new Vector3((float)x,0.0f,(float)y);
				
				//Single-channel Turbulence3d heightmap (4x Fractal)
				float sample = terrainNoise.getNoise(vector,"Height");
				texture1.SetPixel(x,y,new Color(sample,sample,sample,1.0f));
				
				//Three Simplex channels for Red, Green and Blueingle-channel Turbulence3d heightmap (4x Fractal) 
				Color color2 = new Color();
				color2.r = colourNoise.getNoise(vector,"Red");
				color2.g = colourNoise.getNoise(vector,"Green");
				color2.b = colourNoise.getNoise(vector,"Blue");
				texture2.SetPixel(x,y,color2);
		
				//Single-channel Marble noise
				Color color3 = new Color();
				sample = marbleNoise.getNoise(vector,"Color");
				color3.r = 0.96f*sample;
				color3.g = 0.87f*sample;
				color3.b = 0.32f*sample;
				texture3.SetPixel(x,y,color3);
		
				//Single-channel Wood noise
				sample = woodNoise.getNoise(vector,"Color");
				texture4.SetPixel(x,y,new Color(sample,sample,sample,1.0f));
		
				//Single-channel Perlin noise
				sample = perlinNoise.getNoise(vector,"Color");
				texture5.SetPixel(x,y,new Color(sample,sample,sample,1.0f));
		
				//Single-channel Turbulence3d noise
				sample = turbulenceNoise.getNoise(vector,"Color");
				texture6.SetPixel(x,y,new Color(sample,sample,sample,1.0f));
				
		}}
		texture1.Apply();
		material.mainTexture = texture1;	
		texture2.Apply();
		material2.mainTexture = texture2;
		texture3.Apply();
		material3.mainTexture = texture3;
		texture4.Apply();
		material4.mainTexture = texture4;
		texture5.Apply();
		material5.mainTexture = texture5;
		texture6.Apply();
		material6.mainTexture = texture6;
	}
}
