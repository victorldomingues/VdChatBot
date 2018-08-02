namespace VdChatBot
{
    public static class Reactions
    {
        public static bool ContainsAprovation(this string text)
        {
            text = text.ToLower();
            return text.Contains("sim") || text.Contains("claro") || text.Contains("com certeza") || text.Contains("com toda certeza") || text.Contains("claramente") || text.Contains("obvio");
        }
        public static bool ContainsNegation(this string text)
        {
            text = text.ToLower();
            return text.Contains("não") || text.Contains("nao") || text.Contains("nunca");
        }
    }
}