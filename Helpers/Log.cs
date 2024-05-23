using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;

namespace ButtonsExtraBooks.Helpers;

public class Log
{
    public static void Debug<T>(T message) => ModEntry.ModMonitor.Log($"{(message is not string ? "[" + message.GetType() + "] " : string.Empty)}{message.ToString() ?? string.Empty}", LogLevel.Debug);
    
    public static void Error<T>(T message) => ModEntry.ModMonitor.Log($"{(message is not string ? "[" + message.GetType() + "] " : string.Empty)}{message.ToString() ?? string.Empty}", LogLevel.Error);
    
    public static void Warn<T>(T message) => ModEntry.ModMonitor.Log($"{(message is not string ? "[" + message.GetType() + "] " : string.Empty)}{message.ToString() ?? string.Empty}", LogLevel.Warn);
    
    public static void Info<T>(T message) => ModEntry.ModMonitor.Log($"{(message is not string ? "[" + message.GetType() + "] " : string.Empty)}{message.ToString() ?? string.Empty}", LogLevel.Info);
    
    public static void Trace<T>(T message) => ModEntry.ModMonitor.Log($"{(message is not string ? "[" + message.GetType() + "] " : string.Empty)}{message.ToString() ?? string.Empty}", LogLevel.Trace);

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