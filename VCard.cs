using System;
using System.IO;

namespace tCard
{
	/// <summary>
	/// Description of VCardUtils.
	/// </summary>
	public class VCard
	{
		public static bool Export(TContact contact, string targetdir) {
			// verify there is a valid contact here at all
			if(String.IsNullOrWhiteSpace(contact.FirstName) && String.IsNullOrWhiteSpace(contact.LastName) && String.IsNullOrWhiteSpace(contact.FullName)) {
				return false;
			}
			
			// begin according to specs
			string vcardstring = "BEGIN:VCARD\nVERSION:3.0\n";
			
			// if we have first and last names (even just empty strings): write them into the file
			// format: Last;First;Others;Prefix;Suffix
			if(contact.FirstName != null && contact.LastName != null) {
				vcardstring += "N:" + contact.LastName + ";" + contact.FirstName + ";;;\n";
			}
			
			// if formatted/full name exists, add that
			if(!String.IsNullOrWhiteSpace(contact.FullName)) {
				vcardstring += "FN:" + contact.FullName + "\n";
			}
			
			// write kind of contact. currently only supported: user (Telegram export specs unknown)
			if(contact.Kind != TContact.KindOfContact.NULL) {
				vcardstring += "KIND:" + contact.Kind.ToString().ToLower() + "\n";
			}
			
			if(!String.IsNullOrWhiteSpace(contact.PhoneNumber)) {
				vcardstring += "TEL;type=CELL:" + contact.PhoneNumber + "\n";
			}
			
			// finish according to specs
			vcardstring += "END:VCARD";
			
			// file name: use full name if given, merge last and first otherwise
			string filename = String.IsNullOrWhiteSpace(contact.FullName) ? contact.LastName + contact.FirstName : contact.FullName;
			// since Telegram exports some users twice (with and without phone number, type 1 and 2 contact)
			// add suffix _withphone to those with phone number to avoid overwriting
			filename += String.IsNullOrWhiteSpace(contact.PhoneNumber) ? "" : "_withphone";
			
			// add directory to filename
			filename = targetdir + @"\" + filename;
			
			// overwrite protection: if file exists, increment name until no collisions anymore
			if(new FileInfo(filename + ".vcf").Exists) {
				int fileadd = 0;
				while(new FileInfo(filename + "_" + fileadd + ".vcf").Exists) {
					fileadd++;
				}
				filename += "_" + fileadd;
			}
			
			File.WriteAllText(filename + ".vcf", vcardstring);
			Console.WriteLine(vcardstring);
			Console.Write(Environment.NewLine);
			
			return true;
		}
	}
}
