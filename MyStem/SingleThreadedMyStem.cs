namespace MyStem;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

/// <summary>
/// A class for interacting with the MyStem executable in a single-threaded environment.
/// </summary>
/// <remarks>
/// This class is not recommended for use in multithreaded scenarios.
/// </remarks>
public sealed class SingleThreadedMyStem : IDisposable
{
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
	/// Initializes a new instance of the <see cref="SingleThreadedMyStem"/> class with the specified options.
	/// </summary>
	/// <param name="options">The MyStem options to use.</param>
	public SingleThreadedMyStem(MyStemOptions? options = null)
	{
		Options = options ?? new MyStemOptions();
	}

	/// <summary>
	/// Initializes the MyStem process if it's not already running.
	/// </summary>
	public void Initialize()
	{
		if (mystemProcess == null || mystemProcess.HasExited)
		{
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
	/// Analyzes the given text using the MyStem executable in a single-threaded manner.
	/// </summary>
	/// <param name="text">The text to analyze.</param>
	/// <returns>The analysis result from MyStem.</returns>
	/// <exception cref="FileNotFoundException">If the MyStem executable is not found at the specified path.</exception>
	/// <exception cref="FormatException">If an error occurs during the MyStem analysis.</exception>
	public string Analysis(string text)
	{
		if (!File.Exists(MyStemOptions.PathToMyStem))
		{
			throw new FileNotFoundException("Path to MyStem.exe is not valid!");
		}

		try
		{
			Initialize();

			// Write the input text to the MyStem process
			byte[] inputBytes = Encoding.UTF8.GetBytes(text);
			mystemProcess!.StandardInput.BaseStream.Write(inputBytes, 0, inputBytes.Length);
			mystemProcess.StandardInput.BaseStream.Flush();
			mystemProcess.StandardInput.BaseStream.Close();

			// Read the entire output from the MyStem process
			return mystemProcess.StandardOutput.ReadToEnd();
		}
		catch (Exception ex)
		{
			throw new FormatException($"Error during MyStem analysis. See logs for details. Text: '{text}'", ex);
		}
	}

	/// <summary>
	/// Disposes of the resources used by the <see cref="SingleThreadedMyStem"/> object.
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
	/// Finalizes the <see cref="SingleThreadedMyStem"/> object before it is reclaimed by garbage collection.
	/// </summary>
	~SingleThreadedMyStem() => Dispose();
}