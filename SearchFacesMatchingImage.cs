using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using S3Object = Amazon.Rekognition.Model.S3Object;

namespace FaceAmazon.Rekognition
{
    internal class SearchFacesMatchingImage
    {
        public string Example(string path)
        {

            StringBuilder stringBuilder = new StringBuilder();

            FileInfo fileInfo = new FileInfo(path);
            string photo = fileInfo.Name;
            string extension = fileInfo.Name.Remove(fileInfo.Name.IndexOf(fileInfo.Extension));
            string accessKey = "AKIA6MRBY5XDXZYGV7MC";
            string secretKey = " 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";
            String bucket = "obushkobucket";

            IAmazonS3 AmazonS3 = new AmazonS3Client(
        accessKey,
        secretKey,
        Amazon.RegionEndpoint.EUNorth1
        );
            if (!fileInfo.Exists)
            {
                Console.WriteLine("Filepath invalid");
                Environment.Exit(0);
            }
            else
            {

                UploadFileAsync(AmazonS3, bucket, photo, path).Wait();
            }

            AmazonRekognitionClient rekognitionClient = new AmazonRekognitionClient
                (
                        accessKey,
                        secretKey,
                        Amazon.RegionEndpoint.EUNorth1
                );

            DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = new Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = photo,
                        Bucket = bucket
                    },
                },
                MaxLabels = 10,
                MinConfidence = 75F
            };
            string result = "";
            try
            {
                DetectLabelsResponse detectLabelsResponse = rekognitionClient.DetectLabelsAsync(detectlabelsRequest).GetAwaiter().GetResult();
                Console.WriteLine("Detected labels for " + photo);
                foreach (Label label in detectLabelsResponse.Labels)
                {
                    Console.WriteLine("{0}: {1}", label.Name, label.Confidence);
                    result += label.Name + " : " + label.Confidence + "\n";
                }

                File.WriteAllText($"{extension}.txt", stringBuilder.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public static async Task<bool> UploadFileAsync(
        IAmazonS3 client,
        string bucketName,
        string objectName,
        string filePath)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                FilePath = filePath,
            };

            var response = await client.PutObjectAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Successfully uploaded {objectName} to {bucketName}.");
                return true;
            }
            else
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}.");
                return false;
            }
        }

    }
}

