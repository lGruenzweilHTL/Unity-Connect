using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UniConsole : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_InputField inputField;

    private readonly List<string> commandHistory = new();
    private int commandHistoryIndex = 0;

    public UnityEvent<string> OnCommandSubmitAttempted;
    public UnityEvent<TerminalCommand> OnCommandSubmitted;
    public UnityEvent OnTerminalCleared;
    public UnityEvent OnTerminalAwake;

    private void Awake()
    {
        Reflector.UpdateCommandCache();
        Debug.Log($"Number of Commands: " + Reflector.Commands.Length);
        
        inputField.onSubmit.AddListener(OnInputFieldSubmit);
        OnTerminalCleared.AddListener(LogHelpText);

        OnTerminalCleared?.Invoke();
    }

    private void LogHelpText()
    {
        Log("Type 'help' for a list of commands");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && commandHistoryIndex > 0)
        {
            commandHistoryIndex--;
            inputField.text = commandHistory[commandHistoryIndex];
            inputField.caretPosition = inputField.text.Length;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistoryIndex < commandHistory.Count)
        {
            commandHistoryIndex++;
            inputField.text = commandHistoryIndex == commandHistory.Count ? "" : commandHistory[commandHistoryIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HandleAutocomplete(inputField.text);
        }
    }

    private void HandleAutocomplete(string command)
    {
        string[] parts = command.Split(' ');
        command = parts[^1];
        
        var options = TerminalCommand.GetAutocompleteOptions(command)
            .Select(c => c.CommandName)
            //.Concat(TerminalCommand.GetClassAutocompletes(command))
            .ToArray();

        if (options.Length == 0)
            return;

        int diffIdx = GetEarliestDifferenceIndex(options);
        if (diffIdx != command.Length)
        {
            // Calculate final autocomplete
            parts[^1] = options[0][..diffIdx];
            string complete = string.Join(' ', parts);
            
            // Only one option, complete the command
            inputField.text = complete;

            // Move the cursor to the end of the input field
            inputField.caretPosition = complete.Length;
        }
        else
            TerminalLog(command, string.Join(", ", options));
    }

    private static int GetEarliestDifferenceIndex(string[] strings)
    {
        if (strings.Length == 1)
            return strings[0].Length;

        int idx = int.MaxValue;
        for (int i = 0; i < strings.Length - 1; i++)
        {
            int limit = Mathf.Min(strings[i].Length, strings[i + 1].Length);
            for (int j = 0; j < limit; j++)
            {
                if (strings[i][j] != strings[i + 1][j])
                {
                    idx = Mathf.Min(j, idx);
                    break;
                }
            }

            idx = Mathf.Min(limit, idx);
        }

        return idx;
    }

    private void OnInputFieldSubmit(string command)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(command);

            commandHistory.Add(command);
            commandHistoryIndex = commandHistory.Count;

            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void ExecuteCommand(string commandToExecute)
    {
        OnCommandSubmitAttempted?.Invoke(commandToExecute);

        var available = Reflector.Commands;
        string[] commandParts = commandToExecute.Split(' ');
        string commandName = commandParts[0];

        if (commandName.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            logText.text = "";
            OnTerminalCleared?.Invoke();
            return;
        }

        foreach (var command in available)
        {
            var expectedParameters = command.Method.GetParameters();

            if (!command.GetAllPossibleNames().Contains(commandName, StringComparer.OrdinalIgnoreCase))
                continue;
            if (commandParts.Length - 1 != expectedParameters.Length)
                continue;

            // Check for ambiguous commands
            // When a command name is ambiguous, check if the full name has already been specified
            if (command.IsAmbiguous)
            {
                if (commandName.Equals(command.Name,
                        StringComparison.OrdinalIgnoreCase)) // If full name is not specified
                {
                    // Print all possibilities of the command
                    var possibilities =
                        string.Join(", ", available.Where(c => c.Equals(command)).Select(GetHelpString));
                    TerminalLog(commandToExecute, $"Ambiguous command!\nPossible commands: {possibilities}",
                        LogType.Warning);
                    return;
                }
            }

            try
            {
                object[] parameters = ParseParameters(commandParts[1..], command.Method);

                if (expectedParameters.Length == 0)
                    expectedParameters = null;

                // Check if parameters are the same
                if (parameters == null ^ expectedParameters == null)
                    continue;
                if (parameters != null && expectedParameters != null && parameters.Length != expectedParameters.Length)
                    continue;

                object result = command.Method.Invoke(null, parameters);
                if (result != null)
                    TerminalLog(commandToExecute, result);

                OnCommandSubmitted?.Invoke(command);
            }
            catch (Exception)
            {
                TerminalLog(commandToExecute, "Could not Parse parameters", LogType.Error);
            }

            return;
        }

        Log(commandToExecute);
    }

    private object[] ParseParameters(string[] parameters, MethodInfo method)
    {
        if (parameters.Length == 0)
            return null; // Invoke with no parameters

        object[] parsedParameters = new object[parameters.Length];
        Type[] targetTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

        for (var i = 0; i < parsedParameters.Length; i++)
        {
            parsedParameters[i] = targetTypes[i].IsEnum
                ? Enum.Parse(targetTypes[i], parameters[i], true)
                : Convert.ChangeType(parameters[i], targetTypes[i]);
        }

        return parsedParameters;
    }

    private void TerminalLog(object command, object result, LogType logType = LogType.Message)
    {
        string log = $"> {command}\n{result}";

        if (logType == LogType.Message)
            Log(log);
        else if (logType == LogType.Warning)
            LogWarning(log);
        else if (logType == LogType.Error)
            LogError(log);
        else
            throw new ArgumentOutOfRangeException();
    }

    private void Log(object message)
    {
        logText.text += message + "\n";
    }

    private void LogWarning(object message)
    {
        Log("<color=\"yellow\">" + message + "</color>");
    }

    private void LogError(object message)
    {
        Log("<color=\"red\">" + message + "</color>");
    }

    [Command("Clears the console")]
    public static void Clear()
    {
    }

    [Command("Prints a manual for a specific command")]
    public static string Help(string commandName)
        => string.Join('\n', Reflector.Commands.Where(
                c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
            .Select(cmd =>
            {
                string description = cmd.Method.GetCustomAttribute<CommandAttribute>().Description;
                string name = GetHelpString(cmd);
                
                return $"{name}\n\t{description}";
            }));


    [Command("Prints all possible commands")]
    public static string Help()
        => "Available Commands:\n" + string.Join("\n",
            Reflector.Commands
                .Select(GetHelpString));

    private static string GetHelpString(TerminalCommand command)
    {
        string parameters = string.Join(" ", command.Method.GetParameters().Select(p => p.ParameterType.Name));

        return command.IsAmbiguous
            ? $"{command.Class.FullName}.{command.Name} {parameters}"
            : $"{command.Name} {parameters}";
    }
    
    [Command("Exits the application")]
    public static void Exit()
    {
        Application.Quit();
    }
}