using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;

namespace SoftMarineTest.MVVM
{
    class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                try
                {
                    throw new ArgumentNullException(nameof(execute));
                }
                catch (Exception e)
                {
                    StreamWriter er = new StreamWriter("log.txt");
                    er.WriteLine(e.Message);
                    er.Close();

                    throw;
                }
            }

            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
            => this._canExecute == null || this._canExecute();

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
                this._execute();
        }
    }
}
