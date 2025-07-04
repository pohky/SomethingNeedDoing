﻿using SomethingNeedDoing.Core.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SomethingNeedDoing.NativeMacro.Modifiers;
/// <summary>
/// Modifier for specifying wait duration after a command.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WaitModifier"/> class.
/// </remarks>
[GenericDoc(
    "Specify wait duration after a command",
    ["wait", "until"],
    ["/ac Groundwork <wait.3>", "ac Groundwork <wait.3.5", "/ac Groundwork <wait.1-5>", "/ac Groundwork <wait.1.5-5.5>"]
)]
public class WaitModifier(string text, int waitDuration, int maxWaitDuration = 0) : MacroModifierBase(text)
{
    private static readonly Regex Regex = new(@"(?<modifier><wait\.(?<wait>\d+(?:\.\d+)?)(?:-(?<until>\d+(?:\.\d+)?))?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the wait duration in milliseconds.
    /// </summary>
    public int WaitDuration { get; } = waitDuration;

    /// <summary>
    /// Gets the maximum wait duration for random waits.
    /// </summary>
    public int MaxWaitDuration { get; } = maxWaitDuration;

    /// <inheritdoc/>
    public static bool TryParse(ref string text, out IMacroModifier modifier)
    {
        var match = Regex.Match(text);
        if (!match.Success)
        {
            modifier = new WaitModifier(string.Empty, 0);
            return false;
        }

        var group = match.Groups["modifier"];
        text = text.Remove(group.Index, group.Length);

        var waitGroup = match.Groups["wait"];
        var waitValue = waitGroup.Value;
        var wait = (int)(float.Parse(waitValue, CultureInfo.InvariantCulture) * 1000);

        var untilGroup = match.Groups["until"];
        var untilValue = untilGroup.Success ? untilGroup.Value : "0";
        var until = (int)(float.Parse(untilValue, CultureInfo.InvariantCulture) * 1000);

        if (wait > until && until > 0)
            throw new MacroSyntaxError("Until value cannot be lower than the wait value");

        modifier = new WaitModifier(group.Value, wait, until);
        return true;
    }
}
