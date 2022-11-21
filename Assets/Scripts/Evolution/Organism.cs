using System;
using UnityEngine;

/// <summary>
/// Represents an Organism for mathematical functions.
/// </summary>
public class Organism {
    public static readonly Attribute[] Attributes = {
        new("Strength", 0.90, 0.70),
        new("Intelligence", 0.95, 0.05),
        new("Agility", 0.85, 0.30),
        new("Endurance", 0.90, 0.15),
        new("Disease Susceptibility", -0.95, 0.00)
    };

    public double[] AttributeValues { get; private set; }

    private const double _ageingFactor = 0.2;
    public double Age { get; private set; } = 1;

    /// <summary>
    /// Constructs an <c>Organism</c> with random (std = 0.4) attribute values.
    /// </summary>
    public Organism() {
        AttributeValues = new double[Attributes.Length];

        for (int i = 0; i < Attributes.Length; i++) {
            AttributeValues[i] = Mathf.Clamp((float)RandomNormal.Random(0.4), -1, 1);
        }
    }

    /// <summary>
    /// Constructs an <c>Organism</c> with set attribute values.
    /// </summary>
    /// <param name="attributeValues">The attribute values to use.</param>
    /// <exception cref="ArgumentException"></exception>
    public Organism(double[] attributeValues) {
        if (attributeValues.Length != Attributes.Length) {
            throw new ArgumentException("attributeValues.Length != attributes.Length");
        }

        AttributeValues = attributeValues;
    }

    /// <summary>
    /// Changes the organism's age by it's ageing factor.
    /// </summary>
    public void DoAgeing() {
        Age *= _ageingFactor;
    }

    /// <summary>
    /// Calculates the Fitness of the organism.
    /// </summary>
    public double Fitness {
        get {
            double averageFitness = 0;

            for (int i = 0; i < Attributes.Length; i++) {
                averageFitness += Attributes[i].Fitness(AttributeValues[i]);
            }

            averageFitness /= Attributes.Length;

            return averageFitness * Age;
        }
    }

    /// <summary>
    /// Get an Attribute's value from its name.
    /// </summary>
    /// <param name="name">The name of the Attribute to return.</param>
    /// <returns>An Attribute value in the range [-1:1].</returns>
    /// <exception cref="ArgumentException"></exception>
    public double AttributeValue(string name) {
        for (int i = 0; i < Attributes.Length; i++) {
            if (Attributes[i].Name == name) {
                return AttributeValues[i];
            }
        }

        throw new ArgumentException("no attribute named {}", name);
    }
}

/// <summary>
/// Represents one of an <c>Organism's</c> Attributes.
/// </summary>
[Serializable]
public struct Attribute {
    public readonly string Name;
    public readonly double BenefitWeight;
    public readonly double CostWeight;

    public Attribute(string name, double benefitWeight, double costWeight) {
        Name = name;
        BenefitWeight = benefitWeight;
        CostWeight = costWeight;
    }

    /// <summary>
    /// Calculate this Attribute's fitness contribution based on some value.
    /// </summary>
    /// <param name="value">The value to use in the range [-1:1].</param>
    /// <returns>This Attribute's fitness given <c>value</c> in the range [-1:1].</returns>
    public double Fitness(double value) {
        double benefit = BenefitWeight * value;
        double cost = CostWeight * value * value;
        return Math.Tanh(benefit - cost);
    }
}