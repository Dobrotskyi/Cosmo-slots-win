using System.Collections.Generic;
using System.Linq;

public static class ContainsSequenceExtension
{
    public static bool ContainsSequence<T>(this IEnumerable<T> source, IEnumerable<T> sequence)
    {
        int sequenceLength = sequence.Count();
        if (sequenceLength == 0)
            return true;

        int i = 0;
        while (i + sequenceLength <= source.Count())
        {
            IEnumerable<T> sliced = source.Skip(i).Take(sequenceLength);
            if (sliced.SequenceEqual(sequence))
                return true;
            i++;
        }


        return false;
    }
}
