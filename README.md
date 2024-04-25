# IniFileNet
A .NET Library for reading and writing ini files.

# Usage
Note that because this package is currently not at 1.0 yet, this may change. Below is some example code.
I'll probably add a loader class that can load a file into memory in one chunk, and then allow one to essentially do dictionary lookups on it.

```cs
List<Target> targets = [];
using (IniStreamSectionReader iniIn = new(new IniStreamReader(new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read), Encoding.UTF8), new IniReaderOptions(ignoreComments: true))))
{
	IniValueAcceptorDictionaryBuilder b = new(new Dictionary<string, IIniValueAcceptor>(StringComparer.OrdinalIgnoreCase));
	IniValueAcceptorOnlyLast url = b.OnlyLast("Url");
	IniValueAcceptorOnlyFirst outputTemplate = b.OnlyFirst("OutputTemplate");
	IniValueAcceptorSingle<bool> ask = b.Single("Ask", Parse.Boolean);
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
	IniValueAcceptorMany<int, HashSet<int>> seen = new(Parse.Int32);
	Dictionary<string, IIniValueAcceptor> acceptors = b.Acceptors;
	while (iniIn.TryReadNext(out var section))
	{
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
			url: url.Value ?? throw new IniException(IniErrorCode.ValueMissing, "Url is required for " + section.Name),
			outputTemplate: outputTemplate.Value ?? throw new IniException(IniErrorCode.ValueMissing, "OutputTemplate is required for " + section.Name),
			ask: ask.Value,
			regex: regex.Value,
			latest: latest.Value,
			seen: seen.Values
		));
		Util.ResetAll(acceptors.Values);
	}
	iniIn.Reader.Error.ThrowIfError();
}
```