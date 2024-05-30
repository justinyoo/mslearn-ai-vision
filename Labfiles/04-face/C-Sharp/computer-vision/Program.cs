using System;
using System.IO;

using Azure;
using Azure.AI.Vision.ImageAnalysis;

using Microsoft.Extensions.Configuration;

// Import namespaces

namespace detect_people
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServiceKey"];

                // Get image
                string imageFile = "images/people.jpg";
                if (args.Length > 0)
                {
                    imageFile = args[0];
                }

                // Authenticate Azure AI Vision client
                var cvClient = new ImageAnalysisClient(
                    new Uri(aiSvcEndpoint),
                    new AzureKeyCredential(aiSvcKey));

                // Analyze image
                AnalyzeImage(imageFile, cvClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AnalyzeImage(string imageFile, ImageAnalysisClient client)
        {
            Console.WriteLine($"\nAnalyzing {imageFile} \n");

            // Use a file stream to pass the image data to the analyze call
            using FileStream stream = new FileStream(imageFile, FileMode.Open);

            // Detect people in the image.
            ImageAnalysisResult result = client.Analyze(
                BinaryData.FromStream(stream),
                VisualFeatures.People);

            // Print people detection results to the console
            Console.WriteLine($"Image analysis results:");
            Console.WriteLine($" Metadata: Model: {result.ModelVersion} Image dimensions: {result.Metadata.Width} x {result.Metadata.Height}");
            Console.WriteLine($" People:");
            foreach (DetectedPerson person in result.People.Values)
            {
                Console.WriteLine($"   Person: Bounding box {person.BoundingBox.ToString()}, Confidence {person.Confidence:F4}");
            }
        }
    }
}
