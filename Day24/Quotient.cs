using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day24;

public class Quotient : IEquatable<Quotient>, IComparable<Quotient>
{
    public BigInteger Numerator { get; private set; }
    public BigInteger Denominator { get; private set; }

    public Quotient(BigInteger numerator, BigInteger denominator)
    {
        var factor = denominator >= 0 ? 1 : -1;
        var gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);

        if (gcd == 0)
        {
            // prevent division by zero (happens when numerator and denominator are both zero)
            return;
        }

        Numerator = factor * numerator / gcd;
        Denominator = factor * denominator / gcd;
    }

    public Quotient(BigInteger value) : this(value, 1) { }

    public static implicit operator Quotient(long value) => new Quotient(value);
    public static implicit operator Quotient(BigInteger value) => new Quotient(value);

    public static implicit operator BigInteger(Quotient value)
    {
        if (!value.IsInteger)
        {
            throw new Exception("Quotient is not an integer");
        }

        return value.Numerator;
    }

    public bool IsZero => Numerator == 0 && Denominator != 0;

    public bool IsInteger => Denominator == 1;
    public bool IsFinite => Denominator != 0;
    public bool IsNegative => Numerator < 0;

    public Quotient Add(Quotient other)
    {
        var numerator = Numerator * other.Denominator + other.Numerator * Denominator;
        var denominator = Denominator * other.Denominator;

        return new Quotient(numerator, denominator);
    }

    public Quotient Subtract(Quotient other)
    {
        return Add(new Quotient(-other.Numerator, other.Denominator));
    }

    public Quotient Multiply(Quotient other)
    {
        var numerator = Numerator * other.Numerator;
        var denominator = Denominator * other.Denominator;

        return new Quotient(numerator, denominator);
    }

    public Quotient Divide(Quotient other)
    {
        return Multiply(new Quotient(other.Denominator, other.Numerator));
    }

    public static Quotient operator +(Quotient value) => value;
    public static Quotient operator -(Quotient value) => new Quotient(0).Subtract(value);
    public static Quotient operator +(Quotient lhs, Quotient rhs) => lhs.Add(rhs);
    public static Quotient operator -(Quotient lhs, Quotient rhs) => lhs.Subtract(rhs);
    public static Quotient operator *(Quotient lhs, Quotient rhs) => lhs.Multiply(rhs);
    public static Quotient operator /(Quotient lhs, Quotient rhs) => lhs.Divide(rhs);

    public static bool operator ==(Quotient lhs, Quotient rhs) => lhs.Equals(rhs);
    public static bool operator !=(Quotient lhs, Quotient rhs) => !lhs.Equals(rhs);

    public static bool operator <(Quotient lhs, Quotient rhs) => lhs.CompareTo(rhs) < 0;
    public static bool operator >(Quotient lhs, Quotient rhs) => lhs.CompareTo(rhs) > 0;
    public static bool operator <=(Quotient lhs, Quotient rhs) => lhs.CompareTo(rhs) <= 0;
    public static bool operator >=(Quotient lhs, Quotient rhs) => lhs.CompareTo(rhs) >= 0;

    public override string ToString()
    {
        if (Denominator == 0)
        {
            if (Numerator > 0)
            {
                return "Inf";
            }

            if (Numerator < 0)
            {
                return "-Inf";
            }

            return "NaN";
        }

        if (Denominator == 1)
        {
            return Numerator.ToString();
        }

        return $"{Numerator}/{Denominator}";
    }

    public bool Equals(Quotient? other)
    {
        if (other is null)
        {
            return false;
        }

        return Numerator == other.Numerator && Denominator == other.Denominator;
    }

    public override bool Equals(object? obj)
    {
        return obj is Quotient other && this == other;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Numerator);
        hash.Add(Denominator);

        return hash.ToHashCode();
    }

    public int CompareTo(Quotient? other)
    {
        if (other is null)
        {
            return 1;
        }

        var difference = this - other;

        if (difference.IsZero)
        {
            return 0;
        }
        
        if (difference.IsNegative)
        {
            return -1;
        }

        return 1;
    }

    public static Quotient[,] ZeroMatrix(int rows, int columns)
    {
        var result = new Quotient[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                result[i, j] = 0;
            }
        }

        return result;
    }
}
