using System.Text;

namespace ResponseCrafter;

internal static class NamingConventionExtensions
{
    internal static string Default(this string message)
    {
        return message;
    }
    
    internal static string ToSnakeCaseLowerNamingConvention(this string message)
    {
        var words = message.Split(' ');

        var newMessage = new StringBuilder();

        for (var i = 0; i < words.Length; i++)
        {
            newMessage.Append(words[i].ToLower());
            if (i < words.Length - 1)
            {
                newMessage.Append('_');
            }
        }

        return newMessage.ToString();
    }
    
    internal static string ToSnakeCaseUpperNamingConvention(this string message)
    {
        var words = message.Split(' ');

        var newMessage = new StringBuilder();

        for (var i = 0; i < words.Length; i++)
        {
            newMessage.Append(words[i].ToUpper());
            if (i < words.Length - 1)
            {
                newMessage.Append('_');
            }
        }

        return newMessage.ToString();
    }
}