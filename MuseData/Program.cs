
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

class Program
{
    static void Main()
    {
        
        string[] lines = File.ReadAllLines("muse_data3.csv");

        
        string[] columnNames = lines[0].Split(',');
        int avgIndex = Array.IndexOf(columnNames, "Avg_value");
        

        
        Complex[] avgData = new Complex[lines.Length - 1]; 

        

        for (int i = 1; i < lines.Length; i++) 
        {
            string[] parts = lines[i].Split(',');
            avgData[i - 1] = new Complex(double.Parse(parts[avgIndex]), 0);
          
        }

        
        Fourier.Forward(avgData, FourierOptions.Default);
        
        double sampleRate = 250;
        double[] frequencies = new double[avgData.Length];
        for (int i = 0; i < avgData.Length; i++)
        {
            frequencies[i] = i * (sampleRate / avgData.Length);
            
        }


        // Calculate amplitude spectrum
        double[] amplitudeSpectrum = new double[avgData.Length];
        for (int i = 0; i < avgData.Length; i++)
        {
            amplitudeSpectrum[i] = avgData[i].Magnitude;
        }

        double deltaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 0.5, 4);
        double thetaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 4, 8);
        double alphaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 8, 13);
        double betaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 13, 30);
        double gammaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 30, double.PositiveInfinity);

        Console.WriteLine($"Delta band amplitude: {deltaBandAmplitude}");
        Console.WriteLine($"Theta band amplitude: {thetaBandAmplitude}");
        Console.WriteLine($"Alpha band amplitude: {alphaBandAmplitude}");
        Console.WriteLine($"Beta band amplitude: {betaBandAmplitude}");
        Console.WriteLine($"Gamma band amplitude: {gammaBandAmplitude}");


        

        
    }

    static double CalculateBandAmplitude(double[] amplitudeSpectrum, double[] frequencies, double bandStart, double bandEnd)
    {
        double bandAmplitude = 0;
        for (int i = 0; i < frequencies.Length; i++)
        {
            if (frequencies[i] >= bandStart && frequencies[i] < bandEnd)
            {
                bandAmplitude += amplitudeSpectrum[i];
            }
        }
        return bandAmplitude;
    }
    
    


        
    
}
