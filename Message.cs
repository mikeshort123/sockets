using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace crosses
{
    class Message
    {

        private static Regex regex = new Regex(@"{type:(?<type>.*),content:(?<content>.*)}<EOF>", RegexOptions.Compiled);

        public MessageType type;
        public string content;

        public Message(MessageType type, string content) {
            this.type = type;
            this.content = content;
        }

        public byte[] Serialise() {
            string json = "{type:" + GetTypeString(type) + ",content:" + content + "}<EOF>";
            return Encoding.ASCII.GetBytes(json);
        }

        private string GetTypeString(MessageType type) {
            switch (type) {
                case MessageType.JOIN: return "JOIN";
                case MessageType.LEAVE: return "LEAVE";
                case MessageType.TEXT: return "TEXT";
                case MessageType.REGISTER: return "REGISTER";
                case MessageType.CLOSE: return "CLOSE";
                case MessageType.INVALID: return "INVALID";
                default: return "INVALID";
            }
        }

        private static MessageType GetTypeFromString(string data) {
            if (data == "JOIN") return MessageType.JOIN;
            if (data == "LEAVE") return MessageType.LEAVE;
            if (data == "TEXT") return MessageType.TEXT;
            if (data == "REGISTER") return MessageType.REGISTER;
            if (data == "CLOSE") return MessageType.CLOSE;
            if (data == "INVALID") return MessageType.INVALID;
            return MessageType.INVALID;
        }

        public static Message Deserialise(string data) {
            MatchCollection matches = regex.Matches(data);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                MessageType type = GetTypeFromString(groups["type"].Value);
                return new Message(type, groups["content"].Value);
            }
            return new Message(MessageType.INVALID, "regex failure");
        }
    }

    enum MessageType
    {
        JOIN,
        LEAVE,
        TEXT,
        REGISTER,
        CLOSE,
        INVALID
    }
}
