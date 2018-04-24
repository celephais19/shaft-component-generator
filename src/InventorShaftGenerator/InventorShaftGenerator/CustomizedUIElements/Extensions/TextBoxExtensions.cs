namespace InventorShaftGenerator.CustomizedUIElements.Extensions
{
    public static class TextBoxExtensions
    {
        public static bool HasOnlyOne(this string text, char character)
        {
            int count = 0;
            foreach (var ch in text)
            {
                if (ch == character)
                {
                    if (count == 1)
                    {
                        return false;
                    }

                    count++;
                }
            }

            return count == 1;
        }
    }
}
