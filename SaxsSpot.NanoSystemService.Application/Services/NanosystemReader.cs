using SaxsSpot.NanoSystemGeneration.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class NanosystemReader
{
    public static List<Particle> Read(Stream ms)
	{
		string firstLine;
		using var streamReader = new StreamReader(ms);
		firstLine = streamReader.ReadLine();
		
		if(string.IsNullOrEmpty(firstLine)) throw new InvalidOperationException("Invalid input");
			
		if(firstLine.Split(',', ' ').Length == 8)
		{
			return ReadParallelepipedSystem(streamReader, firstLine).ToList();
		}
		if(firstLine.Split(',', ' ').Length == 4)
		{
			return ReadSphereSystem(streamReader, firstLine).ToList();
		}
			
		throw new InvalidOperationException("Invalid input");
	}
	
	public static IEnumerable<Particle> ReadParallelepipedSystem(StreamReader streamReader, string firstLine)
	{

		var firstParms = firstLine.Split(',', ' ');
		var firstPar = new Parallelepiped(float.Parse(firstParms[0])
		, float.Parse(firstParms[1]), float.Parse(firstParms[2]), float.Parse(firstParms[3]),
		float.Parse(firstParms[4]), float.Parse(firstParms[5]), float.Parse(firstParms[6]), float.Parse(firstParms[7]));

		yield return firstPar;
		
		while(!streamReader.EndOfStream)
		{
			var line = streamReader.ReadLine();
			
			var parms = line.Split(',', ' ');
			
			if (parms.Length < 8) continue;
			
			var par = new Parallelepiped(float.Parse(parms[0])
				, float.Parse(parms[1]), float.Parse(parms[2]), float.Parse(parms[3]),
				float.Parse(parms[4]), float.Parse(parms[5]), float.Parse(parms[6]), float.Parse(parms[7]));

			yield return par;
		}
		
		
	}
	
	public static IEnumerable<Particle> ReadSphereSystem(StreamReader streamReader, string firstLine)
	{
		var spheres = new List<Particle>();
		
		var firstParms = firstLine.Split(',');
		spheres.Add(new Sphere(float.Parse(@firstParms[0]), float.Parse(@firstParms[1]), float.Parse(@firstParms[2]), float.Parse(@firstParms[3])));
		
		string[] lines = streamReader.ReadToEnd().Split('\n');
		foreach(var l in lines)
		{
			var @params = l.Split(',');
			if (@params.Length < 4) throw new InvalidOperationException("Invalid sphere data");
			
			spheres.Add(new Sphere(float.Parse(@params[0]), float.Parse(@params[1]), float.Parse(@params[2]), float.Parse(@params[3])));
		}
		
		
		return spheres;
	}    
}
