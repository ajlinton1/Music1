using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;

namespace GDrive
{
    public class GDriveService
    {
        DriveService service;

        public GDriveService()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "78558406012-rgpilvdmsn0borc3bjc9rvq6khe8bn37.apps.googleusercontent.com",
                ClientSecret = "51vAa6qU4CjgsH26GJXoZ7g-",
            },
            new[] { DriveService.Scope.Drive }, "user", CancellationToken.None).Result;

            // Create the service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });
        }

        public DriveService GetService()
        {
            return service;
        }

        public List<File> GetFiles(string search)
        {
            List<File> result = new List<File>();
            FilesResource.ListRequest request = service.Files.List();
            request.Q = search;

            do
            {
                try
                {
                    FileList files = request.Execute();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            return result;
        }

        public void ProcessFiles(string search, Action<File> processFile, int? maxFiles)
        {
            FilesResource.ListRequest request = service.Files.List();
            request.Q = search;
            request.MaxResults = 100;
			int filesProcessed = 0;

            do
            {
                try
                {
                    FileList files = request.Execute();
                    foreach (var file in files.Items)
                    {
                        processFile(file);
						filesProcessed++;
						if ((maxFiles!=null) && (filesProcessed>maxFiles))
						{
							return;
						}
                    }

                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
        }


/*        public static void UploadFile(string filename)
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "78558406012-rgpilvdmsn0borc3bjc9rvq6khe8bn37.apps.googleusercontent.com",
                ClientSecret = "51vAa6qU4CjgsH26GJXoZ7g-",
            },
            new[] { DriveService.Scope.Drive }, "user",  CancellationToken.None).Result;

            // Create the service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });


            File body = new File();
            body.Title = "test.mp3";
            body.Description = "test";
            body.MimeType = "audio/mp3";

            byte[] byteArray = System.IO.File.ReadAllBytes(filename);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, "audio/mp3");
            request.Upload();

            File file = request.ResponseBody;
            Console.WriteLine("File id: " + file.Id);
            Console.WriteLine("Press Enter to end this process.");
            Console.ReadLine();
        } */

		public static List<File> retrieveAllFiles(DriveService service)
		{
			List<File> result = new List<File>();
			FilesResource.ListRequest request = service.Files.List();

			do
			{
				try
				{
					FileList files = request.Execute();

					result.AddRange(files.Items);
					request.PageToken = files.NextPageToken;
				}
				catch (Exception e)
				{
					Console.WriteLine("An error occurred: " + e.Message);
					request.PageToken = null;
				}
			} while (!String.IsNullOrEmpty(request.PageToken));
			return result;
		}

	}
}
