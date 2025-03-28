namespace MyStem;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

/// <summary>
/// A class for interacting with the MyStem executable in a multithreaded environment.
/// </summary>
public sealed class MultiThreadedMyStem : IDisposable
{
	/// <summary>
	/// The default timeout for reading data from the MyStem process.
	/// </summary>
	public static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(50);

	/// <summary>
	/// The maximum allowed length of the processed text before restarting the MyStem process.
	/// </summary>
	public static int MaxLength = 4096;

	/// <summary>
	/// Factor used to estimate the initial total buffer size for reading the output.
	/// </summary>
	public static float TotalBufferFactorSize = 3f;

	/// <summary>
	/// Factor used to estimate the initial step buffer size for reading the output.
	/// </summary>
	public static float StepBufferFactorSize = 2.2f;

	/// <summary>
	/// The string that marks the end of the input for MyStem.
	/// </summary>
	public static string EndString = "\nъъ";

	/// <summary>
	/// The string that is replaced with an empty string in the output.
	/// </summary>
	public static string EndReplaceString = "ъъ??\r\n";

	/// <summary>
	/// An object used for locking to ensure thread-safe access to the MyStem process.
	/// </summary>
	private static readonly object processLock = new();

	/// <summary>
	/// The current total length of the text processed by the current MyStem process instance.
	/// </summary>
	private int currentTextLength = 0;

	/// <summary>
	/// The process instance for the MyStem executable.
	/// </summary>
	private Process? mystemProcess;

	/// <summary>
	/// Indicates whether the object has been disposed.
	/// </summary>
	private bool disposed = false;

	/// <summary>
	/// The options to configure the MyStem process.
	/// </summary>
	public MyStemOptions Options { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="MultiThreadedMyStem"/> class with the specified options. <b>Requires the <c>LineByLine</c> option to be set to true.</b>
	/// </summary>
	/// <param name="options">The MyStem options to use.</param>
	public MultiThreadedMyStem(MyStemOptions? options = null)
	{
		Options = options ?? new MyStemOptions();
		Options.LineByLine = true; // Required for stream reading
	}

	/// <summary>
	/// Initializes the MyStem process if it's not already running or if the processed text length exceeds the maximum limit.
	/// This method is thread-safe.
	/// </summary>
	private void InitializeProcess()
	{
		if (mystemProcess == null || mystemProcess.HasExited || currentTextLength > MaxLength)
		{
			currentTextLength = 0;

			mystemProcess?.Dispose();
			mystemProcess = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = MyStemOptions.PathToMyStem,
					Arguments = Options.GetArguments(),
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					StandardOutputEncoding = Encoding.UTF8
				}
			};
			mystemProcess.Start();
		}
	}

	/// <summary>
	/// Analyzes the given text using the MyStem executable in a multithreaded manner.
	/// </summary>
	/// <param name="text">The text to analyze.</param>
	/// <returns>The analysis result from MyStem.</returns>
	/// <exception cref="FileNotFoundException">If the MyStem executable is not found at the specified path.</exception>
	/// <exception cref="FormatException">If an error occurs during the MyStem analysis.</exception>
	public string MultiAnalysis(string text)
	{
		if (!File.Exists(MyStemOptions.PathToMyStem))
		{
			throw new FileNotFoundException("Path to MyStem.exe is not valid!");
		}

		try
		{
			lock (processLock)
			{
				InitializeProcess();
				return GetResultsAsync(text);
			}
		}
		catch (Exception ex)
		{
			throw new FormatException($"Error during MyStem analysis. See logs for details. Text: '{text}'", ex);
		}
	}

	/// <summary>
	/// Reads the analysis results from the MyStem process asynchronously.
	/// </summary>
	/// <param name="inputText">The input text that was sent to MyStem.</param>
	/// <returns>The analysis result string.</returns>
	private string GetResultsAsync(string inputText)
	{
		inputText += EndString;
		mystemProcess!.StandardInput.WriteLine(inputText);
		mystemProcess.StandardInput.Flush();

		StringBuilder outputBuffer = new((int)MathF.Round(inputText.Length * TotalBufferFactorSize));
		char[] stepBuffer = new char[(int)MathF.Round(inputText.Length * StepBufferFactorSize)];
		int totalBytesRead = 0;
		bool timeoutOccurred = false;

		while (true)
		{
			int bytesRead = 0;
			if (timeoutOccurred)
			{
				var readTask = mystemProcess.StandardOutput.ReadAsync(stepBuffer).AsTask();
				if (readTask.Wait(Timeout))
				{
					bytesRead = readTask.Result;
				} else {
					Console.WriteLine("Timeout occurred while reading from MyStem process.");
				}
			}
			else
			{
				bytesRead = mystemProcess.StandardOutput.Read(stepBuffer, 0, stepBuffer.Length);
			}

			if (bytesRead == 0)
			{
				goto exit_loop;
			}

			outputBuffer.Append(stepBuffer, 0, bytesRead);
			totalBytesRead += bytesRead;

			if (timeoutOccurred || totalBytesRead >= inputText.Length || outputBuffer.Length < inputText.Length)
			{
				timeoutOccurred = true;
				// Check for the end string in the received buffer
				for (int i = (bytesRead == 0 ? stepBuffer.Length : bytesRead) - 3; i >= 0; i--)
				{
					if (stepBuffer[i] == 'ъ' && i - 1 >= 0 && stepBuffer[i - 1] == 'ъ')
					{
						goto exit_loop;
					}
				}
			}
		}

	exit_loop:
		string result = outputBuffer.Replace(EndReplaceString, string.Empty).ToString();
		currentTextLength += result.Length + 1; // +1 Against empty result
		return result;
	}

	/// <summary>
	/// Disposes of the resources used by the <see cref="MultiThreadedMyStem"/> object.
	/// </summary>
	public void Dispose()
	{
		if (!disposed)
		{
			if (mystemProcess != null)
			{
				if (!mystemProcess.HasExited)
				{
					try
					{
						mystemProcess.Kill();
					}
					catch { /* Ignore errors during termination */ }
				}
				mystemProcess.Dispose();
			}
			disposed = true;
		}
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Finalizes the <see cref="MultiThreadedMyStem"/> object before it is reclaimed by garbage collection.
	/// </summary>
	~MultiThreadedMyStem() => Dispose();
}