#if NET11_0_OR_GREATER
namespace CSharp15
{
    // Collection expression arguments
    public class CollectionExpressionArguments
    {
        public void Test()
        {
            string[] values = ["one", "two", "three"];

            List<string> names = [with(capacity: values.Length * 2), .. values];
            HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
            Dictionary<string, int> dict = [with(StringComparer.OrdinalIgnoreCase)];

            Console.WriteLine($"{names.Count} {set.Count} {dict.Count}");
        }
    }

    // Union types
    public record class Cat(string Name);
    public record class Dog(string Name);
    public record class Bird(string Name);

    public union Pet(Cat, Dog, Bird);

    public class UnionTypes
    {
        public string GetName(Pet pet) => pet switch
        {
            Dog d => d.Name,
            Cat c => c.Name,
            Bird b => b.Name,
        };

        public void Test()
        {
            Pet pet = new Dog("Rex");
            Console.WriteLine(GetName(pet));
        }
    }

    // Closed hierarchies
    public closed record class GateState;
    public record class Closed : GateState;
    public record class Open(float Percent) : GateState;

    public class ClosedHierarchies
    {
        public string Describe(GateState state) => state switch
        {
            Closed => "closed",
            Open(var percent) => $"{percent}% open",
        };
    }

    // Extension indexers
    public static class SequenceIndexer
    {
        extension(IEnumerable<int> sequence)
        {
            public int this[int index] => sequence.ElementAt(index);
        }
    }

    public class ExtensionIndexers
    {
        public int Test()
        {
            IEnumerable<int> numbers = Enumerable.Range(1, 10);
            return numbers[2];
        }
    }

    // Labeled break and continue
    public class LabeledBreakContinue
    {
        public (int Row, int Column)? FindGoal(char[,] grid)
        {
            (int Row, int Column)? goal = null;

            outer: for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int column = 0; column < grid.GetLength(1); column++)
                {
                    if (grid[row, column] == '#')
                    {
                        continue outer;
                    }

                    if (grid[row, column] == 'G')
                    {
                        goal = (row, column);
                        break outer;
                    }
                }
            }

            return goal;
        }
    }

    // Non-virtual static interface members
    public interface IMeasurement
    {
        public static double ToMeters(double centimeters) => centimeters / 100;
    }

    public class StaticInterfaceMembers
    {
        public double Test() => IMeasurement.ToMeters(250);
    }
}
#endif
