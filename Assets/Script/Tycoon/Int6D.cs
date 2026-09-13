using System;

/// <summary>
/// 약 179-bit급 표현 범위를 가지는 10억 진수 6-버킷 고정 크기 빅인티저 데이터 타입
/// </summary>
public struct Int6D : IEquatable<Int6D>, IComparable<Int6D>
{
    private int d0;
    private int d1;
    private int d2;
    private int d3;
    private int d4;
    private int d5;

    private bool isPositive;

    private const int BASE = 1_000_000_000;

    public bool IsZero => d0 == 0 && d1 == 0 && d2 == 0 && d3 == 0 && d4 == 0 && d5 == 0;
    public int Sign => IsZero ? 0 : (isPositive ? 1 : -1);


    public static readonly Int6D Zero = new Int6D(0);
    public static readonly Int6D One = new Int6D(1);
    public static readonly Int6D MinusOne = new Int6D(-1);

    public static readonly Int6D MaxValue = new Int6D { d0 = 999_999_999, d1 = 999_999_999, d2 = 999_999_999, d3 = 999_999_999, d4 = 999_999_999, d5 = 999_999_999, isPositive = true };
    public static readonly Int6D MinValue = new Int6D { d0 = 999_999_999, d1 = 999_999_999, d2 = 999_999_999, d3 = 999_999_999, d4 = 999_999_999, d5 = 999_999_999, isPositive = false };
    private static readonly System.Random _sharedRandom = new System.Random();

    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => d0,
                1 => d1,
                2 => d2,
                3 => d3,
                4 => d4,
                5 => d5,
                _ => throw new IndexOutOfRangeException("Int6D는 0부터 5까지의 인덱스만 지원함.")
            };
        }
        private set
        {
            switch (index)
            {
                case 0: d0 = value; break;
                case 1: d1 = value; break;
                case 2: d2 = value; break;
                case 3: d3 = value; break;
                case 4: d4 = value; break;
                case 5: d5 = value; break;
                default: throw new IndexOutOfRangeException("Int6D는 0부터 5까지의 인덱스만 지원함.");
            }
        }
    }

    public Int6D(long value)
    {
        isPositive = value >= 0;

        ulong absValue;
        if (value == long.MinValue)
        {
            absValue = 9223372036854775808UL; // long.MinValue
        }
        else
        {
            absValue = (value < 0) ? (ulong)(-value) : (ulong)value;
        }

        d0 = (int)(absValue % BASE);
        absValue /= (ulong)BASE;

        d1 = (int)(absValue % BASE);
        absValue /= (ulong)BASE;

        d2 = (int)(absValue % BASE);
        absValue /= (ulong)BASE;

        d3 = 0;
        d4 = 0;
        d5 = 0;
    }

    public Int6D(string value)
    {
        if (TryParse(value, out Int6D result))
        {
            this.d0 = result.d0;
            this.d1 = result.d1;
            this.d2 = result.d2;
            this.d3 = result.d3;
            this.d4 = result.d4;
            this.d5 = result.d5;
            this.isPositive = result.isPositive;
        }
        else
        {
            throw new FormatException($"'{value}'는 유효한 Int6D 형식의 문자열이 아님.");
        }
    }

    public override string ToString()
    {
        if (d0 == 0 && d1 == 0 && d2 == 0 && d3 == 0 && d4 == 0 && d5 == 0) return "0";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (!isPositive) sb.Append("-");

        int startIndex = 5;
        while (startIndex >= 0 && this[startIndex] == 0)
        {
            startIndex--;
        }

        for (int i = startIndex; i >= 0; i--)
        {
            if (i == startIndex)
            {
                sb.Append(this[i]);
            }
            else
            {
                sb.Append(this[i].ToString("D9"));
            }
        }

        return sb.ToString();
    }

    public string ToStringWithCommas()
    {
        if (IsZero) return "0";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (!isPositive) sb.Append("-");

        int startIndex = 5;
        while (startIndex >= 0 && this[startIndex] == 0)
        {
            startIndex--;
        }

        for (int i = startIndex; i >= 0; i--)
        {
            if (i == startIndex)
            {
                sb.Append(this[i].ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                int v = this[i];
                sb.Append($",{v / 1000000:D3},{(v / 1000) % 1000:D3},{v % 1000:D3}");
            }
        }

        return sb.ToString();
    }

    public static Int6D Parse(string value)
    {
        if (TryParse(value, out Int6D result))
        {
            return result;
        }
        throw new FormatException($"'{value}'는 유효한 Int6D 형식의 문자열이 아니거나 표현 가능한 범위를 초과함.");
    }

    public static bool TryParse(string value, out Int6D result)
    {
        result = Zero;
        if (string.IsNullOrWhiteSpace(value)) return false;

        value = value.Trim();
        bool isPos = true;

        if (value.StartsWith("-"))
        {
            isPos = false;
            value = value.Substring(1);
        }
        else if (value.StartsWith("+"))
        {
            value = value.Substring(1);
        }

        value = value.TrimStart('0');
        if (value.Length == 0)
        {
            result = Zero;
            return true;
        }

        if (value.Length > 54) return false;

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] < '0' || value[i] > '9') return false;
        }

        Int6D temp = new Int6D();
        temp.isPositive = isPos;

        int bucketIndex = 0;
        int length = value.Length;

        while (length > 0 && bucketIndex < 6)
        {
            int chunkLength = Math.Min(9, length);
            int startIndex = length - chunkLength;
            string chunk = value.Substring(startIndex, chunkLength);

            temp[bucketIndex] = int.Parse(chunk);

            length -= chunkLength;
            bucketIndex++;
        }

        if (isPos)
        {
            if (temp > MaxValue) return false;
        }
        else
        {
            Int6D absMin = MinValue;
            absMin.isPositive = true;
            if (temp > absMin) return false;
        }

        result = temp;
        return true;
    }

    public static implicit operator Int6D(long value)
    {
        return new Int6D(value);
    }

    public static explicit operator int(Int6D target)
    {
        long longValue = (long)target;

        if (longValue > int.MaxValue) return int.MaxValue;
        if (longValue < int.MinValue) return int.MinValue;

        return (int)longValue;
    }

    public static explicit operator long(Int6D target)
    {
        if (target.d5 != 0 || target.d4 != 0 || target.d3 != 0)
        {
            return target.isPositive ? long.MaxValue : long.MinValue;
        }

        ulong absValue = (ulong)target.d2 * BASE * BASE
                       + (ulong)target.d1 * BASE
                       + (ulong)target.d0;

        if (target.isPositive && absValue > (ulong)long.MaxValue)
        {
            return long.MaxValue;
        }
        if (!target.isPositive && absValue > 9223372036854775808UL)
        {
            return long.MinValue;
        }

        return target.isPositive ? (long)absValue : -(long)absValue;
    }

    public static bool operator ==(Int6D a, Int6D b)
    {
        if (a.isPositive != b.isPositive) return false;

        return
            a.d5 == b.d5 &&
            a.d4 == b.d4 &&
            a.d3 == b.d3 &&
            a.d2 == b.d2 &&
            a.d1 == b.d1 &&
            a.d0 == b.d0;
    }

    public static bool operator !=(Int6D a, Int6D b)
    {
        return !(a == b);
    }

    public static bool operator >(Int6D a, Int6D b)
    {
        if (a.isPositive && !b.isPositive) return true;
        if (!a.isPositive && b.isPositive) return false;

        for (int i = 5; i >= 0; i--)
        {
            if (a[i] != b[i])
            {
                if (a.isPositive)
                {
                    return a[i] > b[i];
                }
                else
                {
                    return a[i] < b[i];
                }
            }
        }

        return false;
    }

    public static bool operator <(Int6D a, Int6D b)
    {
        return b > a;
    }

    public static bool operator >=(Int6D a, Int6D b)
    {
        return a > b || a == b;
    }

    public static bool operator <=(Int6D a, Int6D b)
    {
        return a < b || a == b;
    }

    public override bool Equals(object obj)
    {
        return obj is Int6D other && this == other;
    }

    public bool Equals(Int6D other) // IEquatable<Int6D>
    {
        return this == other;
    }

    public int CompareTo(Int6D other)   // IComparable<Int6D>
    {
        if (this == other) return 0;
        return this > other ? 1 : -1;
    }

    public override int GetHashCode()
    {
        if (IsZero) return 0;
        return HashCode.Combine(d0, d1, d2, d3, d4, d5, isPositive);
    }

    public static Int6D operator +(Int6D a, Int6D b)
    {
        if (a.isPositive != b.isPositive)
        {
            Int6D invertedB = b;
            invertedB.isPositive = !b.isPositive;
            return a - invertedB;
        }

        Int6D result = new Int6D();

        result.isPositive = a.isPositive;

        long carry = 0;

        for (int i = 0; i < 6; i++)
        {
            long sum = (long)a[i] + b[i] + carry;
            result[i] = (int)(sum % BASE);
            carry = sum / BASE;
        }

        if (carry > 0)
        {
            result = MaxValue;
            result.isPositive = a.isPositive;
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("Int6D 덧셈이 MaxValue로 클램프됨.");
#endif
            return result;
        }

        return result;
    }

    public static Int6D operator -(Int6D a, Int6D b)
    {
        if (a.isPositive != b.isPositive)
        {
            Int6D invertedB = b;
            invertedB.isPositive = !b.isPositive;
            return a + invertedB;
        }

        Int6D absA = a; absA.isPositive = true;
        Int6D absB = b; absB.isPositive = true;

        if (absA == absB) return 0;

        Int6D large = (absA > absB) ? a : b;
        Int6D small = (absA > absB) ? b : a;

        Int6D result = new Int6D();

        if (a.isPositive) result.isPositive = absA > absB;
        else result.isPositive = absA < absB;

        long borrow = 0;

        for (int i = 0; i < 6; i++)
        {
            long diff = (long)large[i] - small[i] - borrow;

            if (diff < 0)
            {
                diff += BASE;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }

            result[i] = (int)diff;
        }

        return result;
    }

    public static Int6D operator ++(Int6D a) => a + One;
    public static Int6D operator --(Int6D a) => a - One;

    public static Int6D operator *(Int6D a, Int6D b)
    {
        Int6D result = new Int6D();
        result.isPositive = (a.isPositive == b.isPositive);

        long[] tempDigits = new long[12];

        for (int i = 0; i < 6; i++)
        {
            if (a[i] == 0) continue;

            for (int j = 0; j < 6; j++)
            {
                if (b[j] == 0) continue;
                tempDigits[i + j] += (long)a[i] * b[j];
            }
        }

        long carry = 0;
        for (int i = 0; i < 6; i++)
        {
            long total = tempDigits[i] + carry;
            result[i] = (int)(total % BASE);
            carry = total / BASE;
        }

        if (carry > 0 ||
            tempDigits[6] > 0 ||
            tempDigits[7] > 0 ||
            tempDigits[8] > 0 ||
            tempDigits[9] > 0 ||
            tempDigits[10] > 0 ||
            tempDigits[11] > 0)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning("Int6D 곱셈이 MaxValue로 클램프됨.");
#endif
            result = MaxValue;
            result.isPositive = a.isPositive == b.isPositive;
        }

        return result;
    }

    public static Int6D operator /(Int6D a, Int6D b)
    {
        if (b.d0 == 0 && b.d1 == 0 && b.d2 == 0 && b.d3 == 0 && b.d4 == 0 && b.d5 == 0)
        {
            throw new DivideByZeroException("Int6D 데이터타입은 0으로 나눌 수 없음");
        }

        bool finalPositive = (a.isPositive == b.isPositive);

        Int6D absA = a; absA.isPositive = true;
        Int6D absB = b; absB.isPositive = true;

        if (absB > absA) return 0;
        if (absA == absB) return finalPositive ? 1 : -1;

        Int6D low = 0;
        Int6D high = absA;
        Int6D answer = 0;

        while (low <= high)
        {
            Int6D sum = low + high;
            Int6D mid = DivideByTwo(sum);

            Int6D check = absB * mid;

            if (check == absA)
            {
                answer = mid;
                break;
            }
            else if (check < absA)
            {
                answer = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        answer.isPositive = finalPositive;
        return answer;
    }

    private static Int6D DivideByTwo(Int6D target)
    {
        Int6D result = new Int6D();
        result.isPositive = target.isPositive;
        long remain = 0;

        for (int i = 5; i >= 0; i--)
        {
            long current = remain * BASE + target[i];
            result[i] = (int)(current / 2);
            remain = current % 2;
        }
        return result;
    }

    public static Int6D operator %(Int6D a, Int6D b)
    {
        Int6D quotient = a / b;
        return a - (quotient * b);
    }

    public static Int6D operator +(Int6D a) => a;

    public static Int6D operator -(Int6D a)
    {
        Int6D result = a;

        if (a.d0 != 0 || a.d1 != 0 || a.d2 != 0 || a.d3 != 0 || a.d4 != 0 || a.d5 != 0)
        {
            result.isPositive = !a.isPositive;
        }
        return result;
    }

    #region Int6D와 long 연산자 오버로딩

    public static Int6D operator +(Int6D a, long b) => a + (Int6D)b;
    public static Int6D operator +(long a, Int6D b) => (Int6D)a + b;

    public static Int6D operator -(Int6D a, long b) => a - (Int6D)b;
    public static Int6D operator -(long a, Int6D b) => (Int6D)a - b;

    public static Int6D operator *(Int6D a, long b) => a * (Int6D)b;
    public static Int6D operator *(long a, Int6D b) => (Int6D)a * b;

    public static Int6D operator /(Int6D a, long b) => a / (Int6D)b;
    public static Int6D operator /(long a, Int6D b) => (Int6D)a / b;

    public static Int6D operator %(Int6D a, long b) => a % (Int6D)b;
    public static Int6D operator %(long a, Int6D b) => (Int6D)a % b;

    public static bool operator ==(Int6D a, long b) => a == (Int6D)b;
    public static bool operator ==(long a, Int6D b) => (Int6D)a == b;

    public static bool operator !=(Int6D a, long b) => !(a == b);
    public static bool operator !=(long a, Int6D b) => !(a == b);

    public static bool operator >(Int6D a, long b) => a > (Int6D)b;
    public static bool operator >(long a, Int6D b) => (Int6D)a > b;

    public static bool operator <(Int6D a, long b) => a < (Int6D)b;
    public static bool operator <(long a, Int6D b) => (Int6D)a < b;

    public static bool operator >=(Int6D a, long b) => a >= (Int6D)b;
    public static bool operator >=(long a, Int6D b) => (Int6D)a >= b;

    public static bool operator <=(Int6D a, long b) => a <= (Int6D)b;
    public static bool operator <=(long a, Int6D b) => (Int6D)a <= b;

    #endregion

    #region Int6D와 int 연산자 오버로딩

    public static Int6D operator +(Int6D a, int b) => a + (Int6D)(long)b;
    public static Int6D operator +(int a, Int6D b) => (Int6D)(long)a + b;

    public static Int6D operator -(Int6D a, int b) => a - (Int6D)(long)b;
    public static Int6D operator -(int a, Int6D b) => (Int6D)(long)a - b;

    public static Int6D operator *(Int6D a, int b) => a * (Int6D)(long)b;
    public static Int6D operator *(int a, Int6D b) => (Int6D)(long)a * b;

    public static Int6D operator /(Int6D a, int b) => a / (Int6D)(long)b;
    public static Int6D operator /(int a, Int6D b) => (Int6D)(long)a / b;

    public static Int6D operator %(Int6D a, int b) => a % (Int6D)(long)b;
    public static Int6D operator %(int a, Int6D b) => (Int6D)(long)a % b;

    public static bool operator ==(Int6D a, int b) => a == (Int6D)(long)b;
    public static bool operator ==(int a, Int6D b) => (Int6D)(long)a == b;

    public static bool operator !=(Int6D a, int b) => !(a == b);
    public static bool operator !=(int a, Int6D b) => !(a == b);

    public static bool operator >(Int6D a, int b) => a > (Int6D)(long)b;
    public static bool operator >(int a, Int6D b) => (Int6D)(long)a > b;

    public static bool operator <(Int6D a, int b) => a < (Int6D)(long)b;
    public static bool operator <(int a, Int6D b) => (Int6D)(long)a < b;

    public static bool operator >=(Int6D a, int b) => a >= (Int6D)(long)b;
    public static bool operator >=(int a, Int6D b) => (Int6D)(long)a >= b;

    public static bool operator <=(Int6D a, int b) => a <= (Int6D)(long)b;
    public static bool operator <=(int a, Int6D b) => (Int6D)(long)a <= b;

    #endregion

    public static Int6D Abs(Int6D value)
    {
        if (value.isPositive) return value;
        Int6D result = value;
        result.isPositive = true;
        return result;
    }

    public static Int6D Min(Int6D a, Int6D b) => (a < b) ? a : b;

    public static Int6D Max(Int6D a, Int6D b) => (a > b) ? a : b;

    public static Int6D Clamp(Int6D value, Int6D min, Int6D max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static Int6D Pow(Int6D baseValue, int exponent)
    {
        if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent), "음수 지수는 정수 연산에서 지원하지 않음.");
        if (exponent == 0) return One;
        if (exponent == 1) return baseValue;

        Int6D result = One;
        Int6D current = baseValue;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1) result *= current;
            current *= current;
            exponent >>= 1;
        }

        return result;
    }

    public int GetDigitCount()
    {
        if (IsZero) return 1;

        int startIndex = 5;
        while (startIndex >= 0 && this[startIndex] == 0)
        {
            startIndex--;
        }

        int topBucketValue = this[startIndex];
        int bucketDigits = 0;
        while (topBucketValue > 0)
        {
            bucketDigits++;
            topBucketValue /= 10;
        }

        return (startIndex * 9) + bucketDigits;
    }

    public int Log10() => GetDigitCount() - 1;

    public static Int6D Random()
    {
        Int6D randomValue = new Int6D();

        for (int i = 0; i < 6; i++)
        {
            randomValue[i] = _sharedRandom.Next(0, 1_000_000_000);
        }

        if (!randomValue.IsZero)
        {
            randomValue.isPositive = _sharedRandom.Next(0, 2) == 0;
        }
        else
        {
            randomValue.isPositive = true;
        }

        return randomValue;
    }

    public static Int6D Random(Int6D min, Int6D max)
    {
        if (min >= max) return min;

        Int6D range = max - min;
        Int6D randomValue = new Int6D();

        bool isStrict = true;

        for (int i = 5; i >= 0; i--)
        {
            if (isStrict)
            {
                if (range[i] == 0)
                {
                    randomValue[i] = 0;
                }
                else
                {
                    int r = _sharedRandom.Next(0, range[i] + 1);

                    if (r < range[i])
                    {
                        isStrict = false;
                    }
                    randomValue[i] = r;
                }
            }
            else
            {
                randomValue[i] = _sharedRandom.Next(0, 1_000_000_000);
            }
        }

        return min + randomValue;
    }
}