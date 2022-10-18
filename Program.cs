using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace tCard
{
	class Program
	{
		static void DebugLog(string message, bool newline = true) {
			ConsoleColor initColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			if(newline) { Console.WriteLine(message); }
			else { Console.Write(message); }
			Console.ForegroundColor = initColor;
		}
		
		public static void Main(string[] args)
		{
			// checking if file even exists etc etc
			if(args.Length == 0) {
				DebugLog("No arguments provided! Shutting down...");
				return;
			}
			
			FileInfo checkFile = new FileInfo(args[0]);
			if(!checkFile.Exists) {
				DebugLog("Invalid parameter: ", false);
				
				try {
					FileAttributes checkDir = File.GetAttributes(args[0]);
					if(checkDir.HasFlag(FileAttributes.Directory)) {
						DebugLog("filename is directory! Shutting down...");
						return;
					}
				} catch (FileNotFoundException e) {
					DebugLog("file does not exist! Shutting down...");
					return;
				}
			}
			
			// actual file reading starts here
			String tExport = File.ReadAllText(args[0]);
			
			// Telegram exports two kinds of contacts
			
			// "regular" ones with first name, last name, and phone number
			Regex rx_contacts1 = new Regex("{[\\n\\s]*\"first_name\": \"(?'firstname'[^\"]*)\",[\\n\\s]*\"last_name\": \"(?'lastname'[^\"]*)\",[\\n\\s]*\"phone_number\": \"(?'phonenumber'[+\\d-]*)\",[\\n\\s]*\"date\": \"(?'date'[\\d\\-T:.]*)\",[\\n\\s]*\"date_unixtime\": \"(?'dateunix'\\d*)\"[\\n\\s]*}");
			MatchCollection contacts1 = rx_contacts1.Matches(tExport);
			
			// and "frequent" contacts with a full name, ID, type, and (useless) frequency of how often you chat
			Regex rx_contacts2 = new Regex("{[\\n\\s]*\"id\": (?'id'[^\"]*),[\\n\\s]*\"category\": \"(?'category'[^\"]*)\",[\\n\\s]*\"type\": \"(?'type'[^\"]*)\",[\\n\\s]*\"name\": \"(?'name'[^\"]*)\",[\\n\\s]*\"rating\": (?'rating'[\\d.]*)[\\n\\s]*}");
			MatchCollection contacts2 = rx_contacts2.Matches(tExport);
			
			// these regular expressions catch both, file each into its own match collection, and then join them in an array
			TContact[] contacts = new TContact[contacts1.Count + contacts2.Count];
			
			// going through first kind of contacts and creating TContact objects of them
			for(int i = 0; i < contacts1.Count; i++) {
				Match currentMatch = contacts1[i];
				TContact currentContact = new TContact();
				currentContact.FirstName = currentMatch.Groups["firstname"].Value;
				currentContact.LastName = currentMatch.Groups["lastname"].Value;
				currentContact.PhoneNumber = currentMatch.Groups["phonenumber"].Value;
				currentContact.DateCreated= new DateTime(Int64.Parse(currentMatch.Groups["dateunix"].Value)); // we can do parse here because the regex capture group CAN only contain digits, so there HAS to be a proper number in there
				contacts[i] = currentContact;
			}
			
			// going through second kind of contacts and creating TContact objects of them
			for(int i = 0; i < contacts2.Count; i++) {
				Match currentMatch = contacts2[i];
				TContact currentContact = new TContact();
				currentContact.FullName = currentMatch.Groups["name"].Value;
				if(currentMatch.Groups["type"].Value == "user") { currentContact.Kind = TContact.KindOfContact.Individual; }
				currentContact.UUID= Int64.Parse(currentMatch.Groups["id"].Value); // we can do parse here because the regex capture group CAN only contain digits, so there HAS to be a proper number in there
				contacts[i + contacts1.Count] = currentContact;
			}
			
			// if there are any, export them all
			if(contacts.Length > 0) {
				// directory name is directory of contacts file plus subfolder "tCard-YYYY-MM-DD"
				string sourcedir = new FileInfo(args[0]).Directory.FullName;
				string dirname = sourcedir + @"\tCard-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
				
				// if target directory exists already, keep incrementing directory name until we find a free one  
				if(Directory.Exists(dirname)) {
					int diradd = 1;
					while(Directory.Exists(dirname + "_" + diradd)) {
						diradd++;
					}
					dirname += "_" + diradd;
				}
				
				Directory.CreateDirectory(dirname);
				
				foreach(TContact contact in contacts) {
					VCard.Export(contact, dirname);
				}
			} else {
				// if no contacts found, report to user and shut down
				DebugLog("No contacts found in file! Shutting down...");
			}
		}
	}
}