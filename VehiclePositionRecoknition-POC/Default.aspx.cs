using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace VehiclePositionRecoknition_POC
{
	public partial class _Default : Page
	{
		private Bitmap bitmap;

		private class CategoryResult
		{
			public string Position { get; set; }
			public string Percentage { get; set; }
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ImageURL.ImageUrl = "~/content/frontright.jpg";

				try
				{
					RunModel("frontright"); //no need for extension
				}
				catch
				{
					GridView1.EmptyDataText = "Something went wrong, try again!";
					GridView1.DataBind();
				}
			}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{

			try
			{
				var filename = SaveImageLocally();
				RunModel(filename);
			}
			catch
			{
				GridView1.EmptyDataText = "Something went wrong, try again!";
				GridView1.DataBind();
			}
		}

		private void RunModel(string filename)
		{
			string filenameWithExtention = filename + ".jpg";

			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = @"C:\windows\system32\cmd.exe";
			start.UseShellExecute = false;
			start.RedirectStandardInput = true;
			start.RedirectStandardOutput = true;
			start.CreateNoWindow = true;
			using (Process process = Process.Start(start))
			{
				using (StreamWriter writer = process.StandardInput)
				{
					writer.WriteLine(@"cd c:\temp\POC");
					writer.WriteLine(@"activate  C:\Users\fcarrillo\AppData\Local\anaconda3\envs\tensorflow");
					writer.WriteLine($"python label_image.py {filenameWithExtention}");
				}

				using (StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
					var results = CleanResult(result);

					GridView1.DataSource = results;
					GridView1.DataBind();
				}
			}
		}

		private static List<CategoryResult> CleanResult(string result)
		{
			List<CategoryResult> results = new List<CategoryResult>();

			if (result != "")
			{
				var lineArr = result.Trim().Split('\n').Select(n => n).Where(n => !string.IsNullOrEmpty(n)).ToArray();

				int x = 8; //start at 8 end at 12
				while (x < 14)
				{
					string[] parts = lineArr[x].Split('=');
					CategoryResult newCategoryResult = new CategoryResult();

					//clean  the percentage
					parts[0] = parts[0].Replace(" (score ", "");
					parts[1] = parts[1].Trim(')', '\r');
					double percentage = Convert.ToDouble(parts[1]) * 100;

					if (parts[0] == "left front")
						parts[0] = "Left Front";

					else if (parts[0] == "rightfront")
						parts[0] = "Right Front";

					else if (parts[0] == "rear")
						parts[0] = "Rear";

					else if (parts[0] == "front")
						parts[0] = "Front";

					else if (parts[0] == "lfinterior")
						parts[0] = "Left Interior";

					else if (parts[0] == "cargo")
						parts[0] = "Cargo";

					newCategoryResult.Position = parts[0];
					newCategoryResult.Percentage = percentage.ToString("##.##") + " %";

					results.Add(newCategoryResult);

					x++;
				}
			}

			return results;

		}

		private string SaveImageLocally()
		{
			var url = TextBox1.Text;
			var guid = Guid.NewGuid();


			WebClient client = new WebClient();
			Stream stream = client.OpenRead(url);
			bitmap = new Bitmap(stream);

			bitmap.Save(@"c:\temp\POC\" + guid+ ".jpg", ImageFormat.Jpeg);

			//copy file to content file for now
			string fileName = guid + ".jpg";
			string sourcePath = @"C:\temp\POC";
			string targetPath = @"C:\Users\fcarrillo\Documents\Visual Studio 2017\Projects\VehiclePositionRecoknition-POC\VehiclePositionRecoknition-POC\Content";


			string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
			string destFile = System.IO.Path.Combine(targetPath, fileName);

			System.IO.File.Copy(sourceFile, destFile, true);


			//display on page
			ImageURL.ImageUrl= @"~/content/" + guid + ".jpg";

			stream.Flush();
			stream.Close();


			return guid.ToString();
		}

		public Bitmap GetImage()
		{
			return bitmap;
		}
	}
}