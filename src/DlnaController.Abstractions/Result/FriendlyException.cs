using System;

namespace DlnaController.Abstractions
{
    public class FriendlyException : Exception
    {
        public int Code { get; private set; }

        public FriendlyException(int code, string message) : base(message)
        {
            if (code >= 0) throw new ArgumentException($"{nameof(code)}必小于0");
            Code = code;
        }
    }
}