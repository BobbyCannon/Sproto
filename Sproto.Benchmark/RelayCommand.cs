#region References

using System;
using System.Diagnostics;
using System.Windows.Input;

#endregion

namespace Sproto.Benchmark
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The
	/// default return value for the CanExecute method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		#region Fields

		private readonly Predicate<object> _canExecute;

		private readonly Action<object> _execute;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute"> The execution logic. </param>
		/// <param name="canExecute"> The execution status logic. </param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		#endregion

		#region Methods

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke(parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		#endregion

		#region Events

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		#endregion
	}
}