namespace LSlicer
{
    public class Reason
    {
        private bool _result;
        private string _reason = "";

        public Reason()
        {
            _result = true;
        }

        public Reason(string reason)
        {
            _result = false;
            _reason = reason;
        }

        public override string ToString()
        {
            return _reason;
        }

        public static implicit operator bool(Reason result) => result._result;
        public static implicit operator string(Reason result) => result._reason;
    }
}
