using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EuclideanCalculator : MonoBehaviour {

	void Start () {

		Patrones.init();
		Debug.Log (Patrones.listaPatrones[0].name);
		printPattern (Patrones.listaPatrones[0].pulses, Patrones.listaPatrones[0].steps);

	}

	void printPattern(int _pulses, int _steps, int offset=0){
		List<string> cc;
		string ccs = "";

		cc = GetPattern (_pulses, _steps, offset);
		ccs = "";
		foreach (string coco in cc)
			ccs += coco.ToString ();
		Debug.Log (_steps+":"+_pulses+" "+ccs);
	}

	// Update is called once per frame
	List<string>   GetPattern (int pulses, int steps, int offset=0) {

		List<string>  pattern = new List<string>(); 

		if (pulses < 0 || steps < 0 || steps < pulses) {
			return pattern;
		}

		List<string>  first = new List<string>(); 
		List<string>  second = new List<string>(); 

		for (int i=0; i < pulses; i++) {
			first.Add ("1");
		}
		for (int i=0; i < steps - pulses; i++) {
			second.Add ("0");
		}

		int firstLength = first.Count;
		int minLength = Mathf.Min(firstLength, second.Count);

		int loopThreshold = 0;

		while (minLength > loopThreshold) {
			// Allow only loopThreshold to be zero on the first loop
			if (loopThreshold == 0) {
				loopThreshold = 1;
			}

			// For the minimum array loop and concat
			for (var x = 0; x < minLength; x++) {
				first[x]=string.Concat(first[x], second[x]);
				//first[x] = Array.prototype.concat.call(first[x], second[x]);
			}

			// if the second was the bigger array, slice the remaining elements/arrays and update
			if (minLength == firstLength) { 
				second.RemoveRange (0,minLength);
				//second = Array.prototype.slice.call(second, minLength);
			}
			// Otherwise update the second (smallest array) with the remainders of the first
			// and update the first array to include onlt the extended sub-arrays
			else {
				second = new List<string>(first);
				second.RemoveRange (0,minLength);
				//second = Array.prototype.slice.call(first, minLength);
				first.RemoveRange (minLength, first.Count-minLength);
				//first = Array.prototype.slice.call(first, 0, minLength);
			}
			firstLength = first.Count;
			minLength = Mathf.Min(firstLength, second.Count);
		}

		foreach (string ss in first)
			pattern.Add (ss);
		foreach (string ss2 in second)
			pattern.Add (ss2);

		if (offset != 0) {
			string ccs = "";
			foreach (string coco in pattern)
				ccs += coco.ToString ();

			List<string> primerosNuevos= new List<string>();
			List<string> segundosNuevos = new List<string>();
			foreach (char cd2 in ccs) {
				primerosNuevos.Add (cd2.ToString ());
				segundosNuevos.Add (cd2.ToString ());
			}
			primerosNuevos.RemoveRange (0,offset);
			segundosNuevos.RemoveRange (offset, segundosNuevos.Count-offset);

			/*ccs = "";
			foreach (string coco in pattern)
				ccs += coco.ToString ();
			Debug.Log (ccs);

			ccs = "";
			foreach (string coco in primerosNuevos)
				ccs += coco.ToString ();
			Debug.Log (ccs);

			ccs = "";
			foreach (string coco in segundosNuevos)
				ccs += coco.ToString ();
			Debug.Log (ccs);

			pattern = primerosNuevos;
			pattern.AddRange (segundosNuevos);

			ccs = "";
			foreach (string coco in pattern)
				ccs += coco.ToString ();
			Debug.Log (ccs);*/
		}


       return pattern;
     }
}


public class Patrones
{
	public static List<Patron>  listaPatrones;

	public static void init()
	{
		
		listaPatrones = new List<Patron> ();
		listaPatrones.Add (new Patron("West African, Latin American", 2, 3, new int[] {1, 0, 1}));

	}
}


public class Patron{
	public string name;
	public int pulses;
	public int steps;
	public int[] pattern;

	public Patron(string _name,int _pulses, int _steps, int[] _pattern)
	{
		name = _name;
		pulses = _pulses;
		steps = _steps;
		pattern = _pattern;
	}
}