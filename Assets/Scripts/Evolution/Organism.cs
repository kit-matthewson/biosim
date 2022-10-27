using System;
using UnityEngine;

[Serializable]
public class Organism {
    public static readonly Attribute[] attributes = {
        new("Strength",                0.90,  0.70),
        new("Intelligence",            0.95,  0.05),
        new("Agility",                 0.85,  0.30),
        new("Endurance",               0.90,  0.15),
        new("Disease Susceptibility", -0.95,  0.00),
    };

    private readonly double[] attributeValues;
    public double[] AttributeValues {
        get {
            return attributeValues;
        }
    }

    private static readonly double ageingFactor = 0.2;
    private double age = 1;
    public double Age {
        get {
            return age;
        }
    }

    public Organism() { 
        attributeValues = new double[attributes.Length];

        for (int i = 0; i < attributes.Length; i++) {
            attributeValues[i] = Mathf.Clamp((float)RandomNormal.Random(0.4), -1, 1);
        }
    }

    public Organism(double[] attribute_values) {
        if (attribute_values.Length != attributes.Length) {
            throw new ArgumentException("attributeValues.Length != attributes.Length");
        }

        attributeValues = attribute_values;
    }

    public void DoAgeing() {
        age *= ageingFactor;
    }

    public double Fitness {
        get {
            double average_fitness = 0;
            for (int i = 0; i < attributes.Length; i++) {
                average_fitness += attributes[i].Fitness(attributeValues[i]);
            }
            average_fitness /= attributes.Length;

            return average_fitness * age;
        }
    }

    public double AttributeValue(string name) {
        for (int i = 0; i < attributes.Length; i++) {
            if (attributes[i].name == name) {
                return attributeValues[i];
            }
        }

        throw new ArgumentException("no attribute named {}", name);
    }
}

[Serializable]
public struct Attribute {
    public readonly string name;
    public readonly double benefitWeight;
    public readonly double costWeight;

    public Attribute(string name, double benefit_weight, double cost_weight) {
        this.name = name;
        benefitWeight = benefit_weight;
        costWeight = cost_weight;
    }

    public double Fitness(double value) {
        double benefit = benefitWeight * value;
        double cost = costWeight * value * value;
        return Math.Tanh(benefit - cost);
    }
}
