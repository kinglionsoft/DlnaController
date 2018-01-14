namespace DlnaController.OS
{
    public class ProcessResult
    {
        public int ExitCode { get; }

        public string Output { get; }

        public string Error { get; }

        public ProcessResult(int exitCode, string output, string error)
        {
            this.ExitCode = exitCode;
            this.Output = output.TrimEnd('\n');
            this.Error = error;
        }

        public override string ToString()
        {
            return $"{{\"ExitCode\":{ExitCode}, \"Output\":\"{Output}\", \"Error\":\"{Error}\"}}";
        }
    }
}
