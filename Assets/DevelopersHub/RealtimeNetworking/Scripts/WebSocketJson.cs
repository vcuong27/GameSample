namespace DevelopersHub.RealtimeNetworking.Client
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class WebSocketJson
    {
        public static string CreateMessage(int messageID, string jsonValue)
        {
            return CreateMessage(messageID.ToString(), jsonValue, false);
        }

        public static string CreateMessage(string messageID, string jsonValue)
        {
            int parsedMessageID;
            bool isNumeric = int.TryParse(messageID, out parsedMessageID);
            return CreateMessage(messageID, jsonValue, !isNumeric);
        }

        private static string CreateMessage(string messageID, string jsonValue, bool quoteMessageID)
        {
            string idPart = quoteMessageID ? Quote(messageID) : messageID;
            string dataPart = FormatJsonPayload(jsonValue);
            return "{\"messageID\":" + idPart + ",\"data\":" + dataPart + "}";
        }

        public static bool TryParseInitialization(string json, out int clientID, out string token)
        {
            clientID = 0;
            token = string.Empty;

            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            string packetID;
            if (!TryGetString(json, "packetID", out packetID))
            {
                return false;
            }

            if (!string.Equals(packetID, "INITIALIZATION", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return TryGetInt(json, "clientID", out clientID) && TryGetString(json, "token", out token);
        }

        public static bool TryParseServerMessage(string json, out int messageID, out string messageName, out string jsonValue)
        {
            messageID = -1;
            messageName = string.Empty;
            jsonValue = string.Empty;

            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            TryGetInt(json, "messageID", out messageID);
            TryGetString(json, "messageName", out messageName);
            TryGetString(json, "jsonValue", out jsonValue);

            return messageID >= 0 || !string.IsNullOrEmpty(messageName) || !string.IsNullOrEmpty(jsonValue);
        }

        private static string FormatJsonPayload(string jsonValue)
        {
            if (string.IsNullOrWhiteSpace(jsonValue))
            {
                return "{}";
            }

            string trimmed = jsonValue.Trim();
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) || (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                return trimmed;
            }

            return Quote(jsonValue);
        }

        private static string Quote(string value)
        {
            return "\"" + Escape(value ?? string.Empty) + "\"";
        }

        private static string Escape(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length + 8);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '\\': builder.Append("\\\\"); break;
                    case '\"': builder.Append("\\\""); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        if (c < 32)
                        {
                            builder.Append("\\u");
                            builder.Append(((int)c).ToString("x4"));
                        }
                        else
                        {
                            builder.Append(c);
                        }
                        break;
                }
            }
            return builder.ToString();
        }

        private static bool TryGetInt(string json, string fieldName, out int value)
        {
            value = 0;
            Match match = Regex.Match(json, "\\\"" + Regex.Escape(fieldName) + "\\\"\\s*:\\s*(-?\\d+)", RegexOptions.IgnoreCase);
            return match.Success && int.TryParse(match.Groups[1].Value, out value);
        }

        private static bool TryGetString(string json, string fieldName, out string value)
        {
            value = string.Empty;
            Match match = Regex.Match(json, "\\\"" + Regex.Escape(fieldName) + "\\\"\\s*:\\s*\\\"((?:\\\\.|[^\\\"])*)\\\"", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return false;
            }

            value = Unescape(match.Groups[1].Value);
            return true;
        }

        private static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return Regex.Unescape(value);
        }
    }
}
