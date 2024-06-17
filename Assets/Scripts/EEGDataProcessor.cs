using System;
using System.Numerics;
using System.Collections.Concurrent;
using MathNet.Numerics.IntegralTransforms;
using UnityEngine;

public class EEGDataProcessor
{
    public double SampleRate { get; set; } = 250; // Default sample rate

    public EEGDataProcessor(double sampleRate = 250)
    {
        SampleRate = sampleRate;
    }

    // Modify the method to accept a ConcurrentQueue<float> as a parameter
    public double ProcessData(ConcurrentQueue<float> dataQueue)
    {
        // Convert ConcurrentQueue data to Complex array
        Complex[] avgData = new Complex[dataQueue.Count];
        int index = 0;
        while (!dataQueue.IsEmpty)
        {
            // Try to dequeue data from the queue
            if (dataQueue.TryDequeue(out float value))
            {
                avgData[index++] = new Complex(value, 0);
            }
        }

        // Perform Fourier transform
        Fourier.Forward(avgData, FourierOptions.Default);

        // Calculate frequencies and amplitude spectrum
        double[] frequencies = new double[avgData.Length];
        double[] amplitudeSpectrum = new double[avgData.Length];
        for (int i = 0; i < avgData.Length; i++)
        {
            frequencies[i] = i * (SampleRate / avgData.Length);
            amplitudeSpectrum[i] = avgData[i].Magnitude;
        }

        // Calculate amplitude for different frequency bands
        double betaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 13, 30);
        double gammaBandAmplitude = CalculateBandAmplitude(amplitudeSpectrum, frequencies, 30, double.PositiveInfinity);

        return betaBandAmplitude;
    }

    // Method to calculate amplitude for a specified frequency band
    private double CalculateBandAmplitude(double[] amplitudeSpectrum, double[] frequencies, double bandStart, double bandEnd)
    {
        double bandAmplitude = 0;
        int count = 0;
        for (int i = 0; i < frequencies.Length; i++)
        {
            if (frequencies[i] >= bandStart && frequencies[i] < bandEnd)
            {
                bandAmplitude += amplitudeSpectrum[i];
                count++;
            }
        }
        return count > 0 ? bandAmplitude / count : 0;
    }
}
