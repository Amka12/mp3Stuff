using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mp3Stuff.ViewModels;

public class Commands : ICommand
{
    private readonly Func<object, bool> _CanExecute;
    private readonly Action<object> _Execute;

    public Commands(Action<object> Execute, Func<object, bool> CanExecute = null)
    {
        _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
        _CanExecute = CanExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return _CanExecute?.Invoke(parameter) ?? true;
    }

    public void Execute(object parameter)
    {
        _Execute(parameter);
    }
}

public class AsyncCommands : ICommand
{
    private readonly Func<object, Task> _command;
    private readonly Func<object, bool> _CanExecute;

    public AsyncCommands(Func<object, Task> command, Func<object,bool> canExecute)
    {
        _command= command;
        _CanExecute= canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _CanExecute?.Invoke(parameter) ?? true;
    }

    public async void Execute(object parameter)
    {
        await ExecuteAsync(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public Task ExecuteAsync(object p)
    {
        return _command(p);
    }

    protected void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}