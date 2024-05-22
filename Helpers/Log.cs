using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;

namespace ButtonsExtraBooks.Helpers;

public class Log
{
    public static void Debug(string message) => ModEntry.ModMonitor.Log(message, LogLevel.Debug);
    
    // log function that takes any type of message
    public static void Debug<T>(T message) => ModEntry.ModMonitor.Log($"[{message.GetType()}] {message.ToString() ?? string.Empty}", LogLevel.Debug);
    public static void Error<T>(T message) => ModEntry.ModMonitor.Log($"[{message.GetType()}] {message.ToString() ?? string.Empty}", LogLevel.Error);

    public static void ILCode(IEnumerable<CodeInstruction> code)
    {
        foreach (var instruction in code)
        {
            Debug($"{instruction.opcode} {instruction.operand}");
        }
    }

    public static void ILCode(CodeInstruction code)
    {
        Debug($"{code.opcode} {code.operand}");
    }
}