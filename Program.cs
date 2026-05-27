using EdjCase.ICP.Agent;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;


static void C(string text, ConsoleColor color, bool newLine = true)
{
    Console.ForegroundColor = color;
    if (newLine) Console.WriteLine(text);
    else Console.Write(text);
    Console.ResetColor();
}

static void PrintLogo()
{
    Console.WriteLine();
    C("  ██████╗  █████╗ ██████╗ ██████╗  ██████╗ ██╗   ██╗███████╗██████╗ ██╗   ██╗", ConsoleColor.Cyan);
    C("  ██╔══██╗██╔══██╗██╔══██╗██╔══██╗██╔═══██╗██║   ██║██╔════╝██╔══██╗╚██╗ ██╔╝", ConsoleColor.Cyan);
    C("  ██║  ██║███████║██████╔╝██████╔╝██║   ██║██║   ██║█████╗  ██████╔╝ ╚████╔╝ ", ConsoleColor.Cyan);
    C("  ██║  ██║██╔══██║██╔═══╝ ██╔═══╝ ██║▄▄ ██║██║   ██║██╔══╝  ██╔══██╗  ╚██╔╝  ", ConsoleColor.DarkCyan);
    C("  ██████╔╝██║  ██║██║     ██║     ╚██████╔╝╚██████╔╝███████╗██║  ██║   ██║   ", ConsoleColor.DarkCyan);
    C("  ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝      ╚══▀▀═╝  ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝   ", ConsoleColor.DarkCyan);
    Console.WriteLine();
    C("  ┌─────────────────────────────────────────────────────────────────────────┐", ConsoleColor.DarkGray);
    C("  │  ", ConsoleColor.DarkGray, newLine: false);
    C("ICP Canister Query Tool", ConsoleColor.White, newLine: false);
    C("  ·  ", ConsoleColor.DarkGray, newLine: false);
    C("v1.2", ConsoleColor.Yellow, newLine: false);
    C("  ·  ", ConsoleColor.DarkGray, newLine: false);
    C("EdjCase.ICP.WebSockets 6.0.0", ConsoleColor.DarkGray, newLine: false);
    C("              │", ConsoleColor.DarkGray);
    C("  └─────────────────────────────────────────────────────────────────────────┘", ConsoleColor.DarkGray);
    Console.WriteLine();
}

static void PrintSeparator(char ch = '─', int width = 77, ConsoleColor color = ConsoleColor.DarkGray)
{
    C("  " + new string(ch, width), color);
}

static void PrintSection(string title)
{
    Console.WriteLine();
    C("  ┌── ", ConsoleColor.DarkCyan, newLine: false);
    C(title, ConsoleColor.Cyan, newLine: false);
    C(" " + new string('─', Math.Max(0, 70 - title.Length)) + "┐", ConsoleColor.DarkCyan);
}

static void PrintHelp()
{
    PrintLogo();

    C("  USAGE", ConsoleColor.Yellow);
    PrintSeparator();
    C("    DappQuery ", ConsoleColor.White, newLine: false);
    C("-network", ConsoleColor.Green, newLine: false);
    C(" <url> ", ConsoleColor.DarkGray, newLine: false);
    C("-a", ConsoleColor.Green, newLine: false);
    C(" <canister_id> ", ConsoleColor.DarkGray, newLine: false);
    C("-method", ConsoleColor.Green, newLine: false);
    C(" <method_name> ", ConsoleColor.DarkGray, newLine: false);
    C("[-argument <arg1> ...] ", ConsoleColor.DarkYellow, newLine: false);
    C("[-simple]", ConsoleColor.DarkYellow);
    Console.WriteLine();

    C("  OPTIONS", ConsoleColor.Yellow);
    PrintSeparator();

    void Opt(string flag, string desc)
    {
        C("    ", ConsoleColor.White, newLine: false);
        C($"{flag,-14}", ConsoleColor.Green, newLine: false);
        C(desc, ConsoleColor.Gray);
    }

    Opt("-network", "ICP replica or production URL");
    Opt("-a", "Canister ID (last segment of the Candid UI URL)");
    Opt("-method", "Method name to call (from Candid UI)");
    Opt("-argument", "First argument (repeat as -argument2, -argument3 …)");
    Opt("-simple", "Print only the raw result — no logo, no decorations (pipe-friendly)");
    Opt("-help", "Show this help screen");
    Console.WriteLine();

    C("  EXAMPLES", ConsoleColor.Yellow);
    PrintSeparator();

    void Ex(string label, string cmd)
    {
        C($"  ◆ {label}", ConsoleColor.DarkCyan);
        C($"    {cmd}", ConsoleColor.Gray);
        Console.WriteLine();
    }

    Ex("No-argument query (local replica)",
       "DappQuery -network http://localhost:8000 -a t63gs-up777-77776-aaaba-cai -method checkVersion");

    Ex("Query with two arguments",
       "DappQuery -network http://localhost:8000 -a xxx -method textToHash_password -argument \"text\" -argument2 \"password\"");

    Ex("ICP mainnet query",
       "DappQuery -network https://a4gq6-oaaaa-aaaab-qaa4q-cai.icp0.io -a 5nevn-xqaaa-aaaab-aaeja-cai -method showData");

    Ex("Raw output only (pipe-friendly)",
       "DappQuery -network https://a4gq6-oaaaa-aaaab-qaa4q-cai.icp0.io -a 5nevn-xqaaa-aaaab-aaeja-cai -method showData -simple");

    C("  NOTE  ", ConsoleColor.DarkGray, newLine: false);
    C("For mainnet, the canister ID is the ", ConsoleColor.Gray, newLine: false);
 
    Opt("-query", "Force QUERY call (read-only operation). Overrides auto-detection. DEFAULT mode.");
    Opt("-update", "Force UPDATE call (state-changing operation). Overrides auto-detection.");

    Ex(" The using query example",
       "DappQuery -network https://a4gq6-oaaaa-aaaab-qaa4q-cai.icp0.io -a 5nevn-xqaaa-aaaab-aaeja-cai -method showData -query -simple");
    Console.WriteLine();
}

static void PrintError(string message)
{
    Console.WriteLine();
    C("  ╔══ ERROR " + new string('═', 65) + "╗", ConsoleColor.Red);
    C("  ║  ", ConsoleColor.Red, newLine: false);
    C($"{message,-73}", ConsoleColor.White, newLine: false);
    C("  ║", ConsoleColor.Red);
    C("  ╚" + new string('═', 75) + "╝", ConsoleColor.Red);
    Console.WriteLine();
}

static void PrintSuccess(string label, string value)
{
    C("  ✔ ", ConsoleColor.Green, newLine: false);
    C($"{label}: ", ConsoleColor.DarkGray, newLine: false);
    C(value, ConsoleColor.White);
}


var argsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
var listArgs = new List<string>();

for (int i = 0; i < args.Length; i++)
{
    string key = args[i];

    if (key.StartsWith("-argument", StringComparison.OrdinalIgnoreCase))
    {
        if (i + 1 < args.Length)
        {
            listArgs.Add(args[i + 1]);
            i++;
        }
        continue;
    }

    if (i + 1 < args.Length)
    {
        argsDict[key] = args[i + 1];
        i++;
    }
}


if (args.Contains("-help", StringComparer.OrdinalIgnoreCase) || args.Length == 0)
{
    PrintHelp();
    return;
}

if (!argsDict.ContainsKey("-network") ||
    !argsDict.ContainsKey("-a") ||
    !argsDict.ContainsKey("-method"))
{
    PrintLogo();
    PrintError("Missing required parameters: -network, -a and -method are all required.");
    PrintHelp();
    return;
}

bool explicitQuery = args.Contains("-query", StringComparer.OrdinalIgnoreCase);
bool explicitUpdate = args.Contains("-update", StringComparer.OrdinalIgnoreCase);
string replicaUrl = argsDict["-network"];
string canisterId = argsDict["-a"];
string method     = argsDict["-method"];
bool simpleMode   = args.Contains("-simple", StringComparer.OrdinalIgnoreCase);



if (!simpleMode)
{
    PrintLogo();
    C("  SESSION", ConsoleColor.Yellow);
    PrintSeparator();
    PrintSuccess("Network ", replicaUrl);
    PrintSuccess("Canister", canisterId);
    PrintSuccess("Method  ", method);


    if (listArgs.Count > 0)
    {
        for (int i = 0; i < listArgs.Count; i++)
            PrintSuccess($"Arg[{i + 1}]  ", listArgs[i]);
    }

    Console.WriteLine();
}


var agent     = new HttpAgent(new Uri(replicaUrl));
var principal = Principal.FromText(canisterId);


CandidArg candidArg;

if (listArgs.Count == 0)
{
    candidArg = CandidArg.FromCandid();
}
else
{
    var candidValues = listArgs
        .Select(x => CandidTypedValue.Text(x))
        .ToArray();

    candidArg = CandidArg.FromCandid(candidValues);
}

/*    Wywolanie ( Call )        */

bool isQuery = method.Contains("query", StringComparison.OrdinalIgnoreCase);

if (explicitUpdate)
{
    isQuery = false;
}
else if (explicitQuery)
{
    isQuery = true;
}

if (!simpleMode)
{
    C("  CALLING", ConsoleColor.Yellow);
    PrintSeparator();
    C("  ► ", ConsoleColor.DarkCyan, newLine: false);
    C($"{method}", ConsoleColor.White, newLine: false);
    C(" @ ", ConsoleColor.DarkGray, newLine: false);
    C(canisterId, ConsoleColor.Cyan, newLine: false);
    C($"  [{(isQuery ? "QUERY" : "UPDATE")}]", isQuery ? ConsoleColor.Green : ConsoleColor.Yellow);
    Console.WriteLine();
}

try
{
    var response = isQuery
        ? await agent.QueryAsync(principal, method, candidArg)
        : await agent.CallAsync(principal, method, candidArg);

    if (simpleMode)
    {
        Console.WriteLine(response);
    }
    else
    {
        C("  RESULT", ConsoleColor.Yellow);
        PrintSeparator('═', 77, ConsoleColor.DarkGreen);
        Console.WriteLine();
        C("  " + response?.ToString(), ConsoleColor.White);
        Console.WriteLine();
        PrintSeparator('═', 77, ConsoleColor.DarkGreen);
        Console.WriteLine();
        C("  ✔ Done.", ConsoleColor.Green);
    }
}
catch (Exception ex)
{
    if (simpleMode)
    {
        Console.Error.WriteLine(ex.Message);
    }
    else
    {
        Console.WriteLine();
        PrintError(ex.Message);

        if (ex.InnerException is not null)
        {
            C("  Inner: ", ConsoleColor.DarkGray, newLine: false);
            C(ex.InnerException.Message, ConsoleColor.Red);
        }
    }
}

Console.WriteLine();
