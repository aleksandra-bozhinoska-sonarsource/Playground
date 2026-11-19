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
            List<int> unused = [with(capacity: 10), 1, 2, 3]; // Noncompliant S1481 unused local

            Console.WriteLine($"{names.Count} {set.Count} {dict.Count}");
        }

        public void Concatenate(string[] values)
        {
            List<string> names = [with(capacity: values.Length), .. values];
            string result = "";
            foreach (var name in names)
            {
                result += name; // Noncompliant S1643 string concatenation in loop
            }

            // Noncompliant S125 commented-out code below
            // List<string> other = [with(capacity: 5), .. names];
            // other.Add(result);
            Console.WriteLine(result);
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

        public string Speak(Pet pet) => GetSound(pet, true);

        private static string GetSound(Pet pet, bool loud) // Noncompliant S1172 unused parameter 'loud'
        {
            string sound;
            if (pet is Dog)
            {
                sound = "Woof";
                Console.WriteLine(sound);
            }
            else if (pet is Cat)
            {
                sound = "Woof"; // Noncompliant S1871 identical branch
                Console.WriteLine(sound);
            }
            else
            {
                sound = "Tweet";
            }

            // TODO: handle birds // Noncompliant S1135
            return sound;
        }

        public void Test()
        {
            Pet pet = new Dog("Rex");
            Console.WriteLine(GetName(pet));
        }

        private void FeedPet(Pet pet) // Noncompliant S1144 unused private method
        {
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

        public string DescribeVerbose(GateState state)
        {
            bool isOpen = state is Open;
            if (isOpen == true) // Noncompliant S1125 redundant boolean literal
            {
                var percent = ((Open)state).Percent;
                return percent > 50 ? percent > 90 ? "wide open" : "mostly open" : "barely open"; // Noncompliant S3358 nested ternary
            }

            return Describe(state);
        }
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
            int first = numbers[0]; // Noncompliant S1854 dead store
            first = numbers[1];
            return numbers[2] + first;
        }

        public int Sum(IEnumerable<int> numbers, int count)
        {
            int total = 0;
            for (int i = 0; i < count; i++)
            {
                if (numbers[i] == numbers[i]) // Noncompliant S1764 identical operands
                {
                    total += numbers[i];
                }
            }
            return total;
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

        public int CountUntilWall(char[,] grid)
        {
            int count = 0;
            outer: for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int column = 0; column < grid.GetLength(1); column++)
                {
                    if (grid[row, column] == '#')
                    {
                        break outer;
                    }

                    if (grid[row, column] != '.')
                    {
                        if (grid[row, column] != ' ') // Noncompliant S1066 mergeable if
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public void FindWithGoto(int[] items) // Noncompliant S907 goto
        {
            foreach (var item in items)
            {
                if (item == 42)
                {
                    goto found;
                }
            }
            return;
        found:
            Console.WriteLine("found");
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

        public double Total(double a, double b, double c, double d, double e, double f, double g, double h) // Noncompliant S107 too many parameters
            => IMeasurement.ToMeters(a + b + c + d + e + f + g + h);

        public void Convert() // Noncompliant S1186 empty method
        {
        }

        public double SafeConvert(double value)
        {
            try
            {
                return IMeasurement.ToMeters(value);
            }
            catch (Exception) // Noncompliant S2737 catch clause only rethrows
            {
                throw;
            }
        }
    }
}
#endif
