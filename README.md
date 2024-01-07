# IniFileNet
A .NET Library for reading and writing ini files.

# Usage
Note that because this package is currently not at 1.0 yet, this may change. Below is some example code.
I'll probably add a loader class that can load a file into memory in one chunk, and then allow one to essentially do dictionary lookups on it.

```cs
List<Target> targets = [];
using (IniStreamSectionReader iniIn = new(new IniStreamReader(new StreamReader(new FileStream(args[0], FileMode.Open, FileAccess.Read), Encoding.UTF8), new IniReaderOptions(ignoreComments: true))))
{
	IniValueAcceptorOnlyLast url = new();
	IniValueAcceptorOnlyLast outputTemplate = new();
	IniValueAcceptorOnlyLast<bool> ask = new(Parse.Boolean);
	IniValueAcceptorOnlyLast<Regex?> regex = new((string value) =>
	{
		try
		{
			return new(new Regex(value), default);
		}
		catch (Exception ex)
		{
			return new(null, new IniError(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as Regex: ", ex.Message)));
		}
	});
	IniValueAcceptorOnlyLast<DateTimeOffset> latest = new((string value) => DateTimeOffset.TryParse(value, out var r)
		? new(r, default)
		: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as datetime"))));
	IniValueAcceptorMany<int, HashSet<int>> seen = new(Parse.Int32);
	IReadOnlyDictionary<string, IIniValueAcceptor> acceptors = new Dictionary<string, IIniValueAcceptor>()
	{
		["Url"] = url,
		["OutputTemplate"] = outputTemplate,
		["Ask"] = ask,
		["Regex"] = regex,
		["Latest"] = latest,
		["Seen"] = seen,
	};
	while (iniIn.TryReadNext(out var section))
	{
		IniError err = section.AcceptAll(acceptors);
		if (err.Code != default)
		{
			Console.WriteLine("Error reading ini file: " + err.Msg);
			throw err.ToException();
		}

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
	if (iniIn.Reader.Error.Code != default)
	{
		Console.WriteLine("Error reading ini file: " + iniIn.Reader.Error.Msg);
		throw iniIn.Reader.Error.ToException();
	}
}
```