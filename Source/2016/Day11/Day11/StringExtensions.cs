namespace Day11
{
    public static class StringExtensions
    {
        public static string Last(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Substring(input.Length - 1, 1);
        }

        public static string AllButLast(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Substring(0, input.Length - 1);
        }

        public static string[][] Clone(string[][] input)
        {
            string[][] clone = new string[input.Length][];

            for (int y = 0; y < input.Length; y++)
            {
                clone[y] = Clone(input[y]);
            }

            return clone;
        }

        public static string[] Clone(string[] input)
        {
            string[] clone = new string[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                clone[i] = input[i];
            }

            return clone;
        }
    }
}
