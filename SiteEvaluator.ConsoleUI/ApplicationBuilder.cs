namespace SiteEvaluator.ConsoleUI
{
    public class ApplicationBuilder
    {
        private readonly string[] _args;
        private IConsoleView _consoleView;

        internal ApplicationBuilder(params string[] args)
        {
            _args = args;
        }

        public void SetBootstrap(IConsoleView consoleView)
        {
            _consoleView = consoleView;
        }
        
        public Application Build()
        {
            return new Application(_consoleView);
        }
    }
}