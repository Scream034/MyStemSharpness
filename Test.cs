#if TEST

using System.Diagnostics;
using MyStem;

public static class Test
{
	public static void Main()
	{
		MultiThreadedMyStem stem = new(new() { PrintOnlyLemmasAndGrammemes = true });

		List<string> inputs = new List<string>() {"Двигатель башни колонки", "!!!!", "Тестовъ три", "Тестовых восемь тысяч", "Где деньги Либовский?"};
		for (int i = 0; i < 100000; i++)
		{
			inputs.Add(i.ToString());
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		foreach (var input in inputs)
		{
			var result = stem.MultiAnalysis(input);
			Console.WriteLine($"{input} -> {result}");
		}

		stopwatch.Stop();
		Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
		Console.WriteLine($"Total memory: {Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024} MB");
	}
}

#endif