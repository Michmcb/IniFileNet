# IniFileNet
A .NET Library for reading and writing ini files.

# Usage
Note that because this package is currently not at 1.0 yet, this may change. Below is some example code.

The `IniStreamReader` class lets you read individual sections, keys and values, and comments. The `IniStreamSectionReader` wraps that class and groups keys and values up into sections for you.

You can use instances of `IIniValueAcceptor` to accept values with specific keys from a section, which helps parsing destination types and validating that everything loads correctly.


```cs
List<Target> targets = [];
using (IniStreamSectionReader iniIn = new(new IniStreamReader(new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read), Encoding.UTF8), DefaultIniTextEscaper.Default, new IniReaderOptions(ignoreComments: true))))
{
	IniValueAcceptorDictionaryBuilder b = new(new Dictionary<string, IIniValueAcceptor>(StringComparer.OrdinalIgnoreCase));
	IniValueAcceptorOnlyLast url = b.OnlyLast("Url");
	IniValueAcceptorOnlyFirst outputTemplate = b.OnlyFirst("OutputTemplate");
	IniValueAcceptorSingle<bool> ask = b.Single("Ask", IniParse.Boolean);
	IniValueAcceptorOnlyLast<Regex?> regex = b.OnlyLast("Regex", (string value) =>
	{
		try
		{
			return new IniResult<Regex?>(new Regex(value), default);
		}
		catch (Exception ex)
		{
			return new IniResult<Regex?>(null, new IniError(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as Regex: ", ex.Message)));
		}
	});
	IniValueAcceptorOnlyLast<DateTimeOffset> latest = b.OnlyLast("Seen", (string value) => DateTimeOffset.TryParse(value, out var r)
		? new IniResult<DateTimeOffset>(r, default)
		: new IniResult<DateTimeOffset>(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as DateTimeOffset"))));
	IniValueAcceptorMany<int, HashSet<int>> seen = new(IniParse.Int32);
	Dictionary<string, IIniValueAcceptor> acceptors = b.Acceptors;
	while (iniIn.NextSection())
	{
		ReadOnlyIniSection section = iniIn.Section;
		IniError err = section.AcceptAll(acceptors);
		// Can explicitly do it this way
		if (err.Code != default)
		{
			Console.WriteLine("Error reading ini file: " + err.Msg);
			throw err.ToException();
		}
		// Or just call this
		err.ThrowIfError();

		targets.Add(new Target
		(
			name: section.Name,
			url: url.ValueOrException(),
			outputTemplate: outputTemplate.ValueOrException(),
			ask: ask.Value,
			regex: regex.Value,
			latest: latest.Value,
			seen: seen.Value
		));
		IniUtil.ResetAll(acceptors.Values);
	}
	iniIn.Error.ThrowIfError();
}
```