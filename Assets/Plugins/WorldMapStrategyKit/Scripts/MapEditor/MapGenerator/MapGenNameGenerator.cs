using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator;
using WorldMapStrategyKit.MapGenerator.Geom;

namespace WorldMapStrategyKit {

	public partial class WMSK_Editor : MonoBehaviour {

		readonly string[] preffix = new string[] {
			"ab",
			"al",
			"an",
			"am",
			"as",
			"at",
			"ax",
			"ba",
			"bac",
			"bah",
			"bal",
			"ban",
			"be",
			"ber",
			"bi",
			"bo",
			"bor",
			"bo",
			"bra",
			"brob",
			"brun",
			"bru",
			"bu",
			"bul",
			"buen",
			"bur",
			"ca",
			"car",
			"cher",
			"chin",
			"co",
			"cor",
			"cos",
			"da",
			"dat",
			"daw",
			"delt",
			"di",
			"drak",
			"druss",
			"dur",
			"durk",
			"east",
			"eco",
			"ed",
			"elb",
			"equa",
			"er",
			"es",
			"eu",
			"farf",
			"fe'",
			"fe",
			"feld",
			"fla",
			"flo",
			"fook",
			"fr",
			"ge",
			"gé",
			"gi",
			"gl",
			"go",
			"grin",
			"gu",
			"gr",
			"gro",
			"ha",
			"hol",
			"hor",
			"idr",
			"illé",
			"illy",
			"is",
			"ix",
			"ka",
			"ker",
			"khe",
			"khu",
			"ki",
			"klo",
			"kra",
			"kra",
			"kum",
			"ku",
			"ky",
			"lat",
			"lo",
			"lau",
			"leu",
			"lich",
			"loom",
			"luft",
			"lug",
			"low",
			"ma",
			"mal",
			"man",
			"mon",
			"mar",
			"me",
			"men",
			"mol",
			"mor",
			"mun",
			"my",
			"no",
			"nai",
			"na",
			"ner",
			"ne",
			"new ",
			"no",
			"norg",
			"nu",
			"o",
			"or",
			"os",
			"ou",
			"pa",
			"po",
			"pu",
			"qa",
			"que",
			"qu",
			"ra",
			"re",
			"rhe",
			"ro",
			"ru",
			"st. ",
			"sa",
			"saint ",
			"san ",
			"se",
			"sh",
			"sl",
			"so",
			"sp",
			"sq",
			"su",
			"sven",
			"sy",
			"ta",
			"taz",
			"tch",
			"te",
			"ti",
			"trans-",
			"to",
			"tr",
			"tro",
			"tse",
			"tur",
			"tyr",
			"um",
			"un",
			"the ",
			"united ",
			"uq",
			"urk",
			"ur",
			"u",
			"va",
			"val ",
			"ver",
			"vey",
			"ves",
			"vul",
			"wa",
			"west",
			"yellow ",
			"yer",
			"yu",
			"za",
			"ze",
			"zu"
		};
		readonly string[] middle = new string[] {
			"sur",
			"dis",
			"do",
			"vo",
			"maig",
			"tra",
			"pu",
			"go",
			"me",
			"ri",
			"fe",
			"de",
			"ra",
			"ta",
			"chu",
			"gre",
			"ler",
			"lan",
			"pha",
			"bar",
			"te",
			"ha",
			"drin",
			"ga",
			"gis",
			"ra",
			"bi",
			"tu",
			"to",
			"sen",
			"zer",
			"kis",
			"al",
			"fus",
			"lum",
			"du",
			"ros",
			"tyr",
			"gin",
			"zat",
			"gan",
			"ding",
			"zun",
			"dan",
			"chis",
			"na",
			"ven",
			"tu",
			"ran",
			"gli",
			"os",
			"le",
			"do",
			"bom",
			"mo",
			"cra",
			"de",
			"ra",
			"jack",
			"pa",
			"ne",
			"cia",
			"din",
			"te",
			"tu",
			"ber",
			"no",
			"to",
			"ka",
			"dur",
			"kas",
			"do",
			"bo",
			"rew",
			"toc",
			"ras",
			"sen",
			"ri",
			"tan",
			"mis",
			"zirs",
			"za",
			"ten",
			"dri",
			"li",
			"roe",
			"do",
			"ri",
			"ve",
			"sho",
			"crum",
			"be",
			"cea",
			"ter",
			"man",
			"ranis",
			"pi",
			"go",
			"gis",
			"mil",
			"kan",
			"hun",
			"zi",
			"mun",
			"phyr"
		};

		readonly string[] suffix = new string[] {
			"stan",
			"ria",
			"via",
			"nia",
			"maigne",
			"tis",
			"sia",
			"dom",
			"ca",
			"fan",
			"tis",
			"ain",
			"ri",
			"tish",
			"ka",
			"lia",
			"ma",
			"lands",
			"nji",
			"ran",
			"lya",
			"fuscu",
			"bia",
			"gnag",
			"danga",
			"tan",
			"tura",
			"ostro",
			"thia",
			"rus",
			"tion",
			"land",
			"blic",
			"tes",
			"guay",
			"se",
			"nu",
			"lof",
			"gen",
			"ra",
			"pia",
			"han",
			"sia",
			"du",
			"whon",
			"cia",
			"felu",
			"berg",
			"thurm",
			"rin",
			"mer",
			"chia",
			"sha",
			"drib",
			"dal",
			"dour",
			"dia",
			"der",
			"wick",
			"stark",
			"lla",
			"ance",
			"iléa",
			"ogg",
			"daq",
			"hrus",
			"hfar",
			"kuta",
			"'in",
			"med",
			"zhia",
			"rat",
			"gash",
			"poor",
			"etta",
			"ci",
			"orra",
			"pis",
			"lus",
			"ka",
			"sia",
			"russ",
			"ny",
			"ana",
			"nia",
			"aven",
			"bia",
			"au",
			"dor",
			"san",
			"mar",
			"mran",
			"rac",
			"rie",
			"eiro",
			"enzo",
			"cos",
			"iffe",
			"isca",
			"khan",
			"da",
			"kenz",
			"anda",
			"wanda",
			"pire",
			"donia",
			"erba",
			"phyra"
		};
		readonly string[] voxels = new string[] { "a", "e", "i", "o", "u" };


		string GetUniqueRandomName (int minLength, int maxLength, HashSet<string> usedNames) {
			string origName = GetRandomName (minLength, maxLength);
			string name = origName;
			int tries = 1;
			while (usedNames.Contains (name)) {
				name = origName + tries;
				tries++;
			}
			usedNames.Add(name);
			return name;
		}

		string GetRandomName (int minLength, int maxLength) {
	

			if (sb == null) {
				sb = new StringBuilder ();
			} else {
				sb.Length = 0;
			}

			sb.Append (preffix [UnityEngine.Random.Range (0, preffix.Length)]);
			int len = UnityEngine.Random.Range (minLength, maxLength + 1) - 2;
			if (len == 0 || UnityEngine.Random.value > 0.5f) {
				sb.Append (voxels [UnityEngine.Random.Range (0, 5)]);
			}
			for (int k = 0; k < len; k++) {
				sb.Append (middle [UnityEngine.Random.Range (0, middle.Length)]);
				if (UnityEngine.Random.value > 0.5f) {
					sb.Append (voxels [UnityEngine.Random.Range (0, 5)]);
				}
			}
			sb.Append (suffix [UnityEngine.Random.Range (0, suffix.Length)]);
			return sb.ToString ();
		}

	}
}