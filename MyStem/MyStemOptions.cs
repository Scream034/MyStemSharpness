namespace MyStem;

/// <summary>
/// Represents the command-line options for the MyStem executable.
/// </summary>
public class MyStemOptions
{
	/// <summary>
	/// The path to the MyStem executable.
	/// </summary>
	public static string PathToMyStem { get; set; } = "mystem.exe";

	/// <summary>
	/// Enables line-by-line mode; each word is printed on a new line.
	/// </summary>
	public bool LineByLine { get; set; }

	/// <summary>
	/// Copies the entire input to the output, including words and inter-word spaces.
	/// Necessary for returning to the full representation of the text.
	/// In case of line-by-line output (when the -n option is set), inter-word spaces are pulled into one line,
	/// and newline characters are replaced with \r and/or \n.
	/// Space is replaced with an underscore for better visibility.
	/// The \ symbol is replaced with \\, and the underscore with \_.
	/// This allows for unambiguous reconstruction of the original text.
	/// </summary>
	public bool CopyInputToOutput { get; set; }

	/// <summary>
	/// Prints only dictionary words.
	/// </summary>
	public bool PrintOnlyDictionaryWords { get; set; }

	/// <summary>
	/// Does not print the original word forms, only lemmas and grammemes.
	/// </summary>
	public bool PrintOnlyLemmasAndGrammemes { get; set; }

	/// <summary>
	/// Prints grammatical information (decoding provided below).
	/// </summary>
	public bool PrintGrammaticalInformation { get; set; }

	/// <summary>
	/// Glues the information of word forms with the same lemma (only when the -i option is enabled).
	/// </summary>
	public bool GlueWordFormInformation { get; set; }

	/// <summary>
	/// Prints the end-of-sentence marker (only when the -c option is enabled).
	/// </summary>
	public bool PrintEndOfSentenceMarker { get; set; }

	/// <summary>
	/// Input/output encoding. Possible values: cp866, cp1251, koi8-r, utf-8 (default).
	/// </summary>
	public string Encoding { get; set; } = "utf-8";

	/// <summary>
	/// Applies contextual homonymy disambiguation.
	/// </summary>
	public bool ApplyContextualDisambiguation { get; set; }

	/// <summary>
	/// Prints English designations of grammemes.
	/// </summary>
	public bool UseEnglishGrammemeNames { get; set; }

	/// <summary>
	/// Builds analyses only with the specified grammemes.
	/// </summary>
	public List<string> FilterGrammemes { get; set; } = new List<string>();

	/// <summary>
	/// Uses a file with a custom dictionary.
	/// </summary>
	public string FixListFile { get; set; } = string.Empty;

	/// <summary>
	/// Output format. Possible values: text, xml, json. Default value is text.
	/// </summary>
	public string Format { get; set; } = "text";

	/// <summary>
	/// Generates all possible hypotheses for non-dictionary words.
	/// </summary>
	public bool GenerateAllHypotheses { get; set; }

	/// <summary>
	/// Prints the context-free probability of the lemma.
	/// </summary>
	public bool PrintLemmaWeight { get; set; }

	/// <summary>
	/// Gets the command-line arguments string based on the current options.
	/// </summary>
	/// <returns>The command-line arguments string.</returns>
	public string GetArguments()
	{
		var arguments = new List<string>();

		if (LineByLine)
		{
			arguments.Add("-n");
		}

		if (CopyInputToOutput)
		{
			arguments.Add("-c");
		}

		if (PrintOnlyDictionaryWords)
		{
			arguments.Add("-w");
		}

		if (PrintOnlyLemmasAndGrammemes)
		{
			arguments.Add("-l");
		}

		if (PrintGrammaticalInformation)
		{
			arguments.Add("-i");
		}

		if (GlueWordFormInformation)
		{
			arguments.Add("-g");
		}

		if (PrintEndOfSentenceMarker)
		{
			arguments.Add("-s");
		}

		if (!string.IsNullOrEmpty(Encoding) && Encoding.ToLowerInvariant() != "utf-8")
		{
			arguments.Add($"-e {Encoding}");
		}

		if (ApplyContextualDisambiguation)
		{
			arguments.Add("-d");
		}

		if (UseEnglishGrammemeNames)
		{
			arguments.Add("--eng-gr");
		}

		if (FilterGrammemes != null && FilterGrammemes.Any())
		{
			arguments.Add($"--filter-gram {string.Join(",", FilterGrammemes)}");
		}

		if (!string.IsNullOrEmpty(FixListFile))
		{
			arguments.Add($"--fixlist {FixListFile}");
		}

		if (!string.IsNullOrEmpty(Format) && Format.ToLowerInvariant() != "text")
		{
			arguments.Add($"--format {Format}");
		}

		if (GenerateAllHypotheses)
		{
			arguments.Add("--generate-all");
		}

		if (PrintLemmaWeight)
		{
			arguments.Add("--weight");
		}

		return string.Join(" ", arguments);
	}
}