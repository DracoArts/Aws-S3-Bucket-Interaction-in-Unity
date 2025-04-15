using UnityEngine;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
public enum ImageFormat
{
    PNG,
    JPG
}
public class AWSManager : MonoBehaviour
{
    private AmazonS3Client _s3Client;
    private const string BucketName = "dracoarts-logo"; // Change you Bucket Name
    private const string Region = "eu-north-1"; // Change to your region

    private string accessKey = "Replace access key";
    private string secretKey = "Replace seceretkey ";

    [Header("UI Element")]
    public RawImage displayImage;
    public RawImage imageToUpload; // Assign in inspector
    public GameObject SuccesslPanel;
    public Text Succcess_Text;
    public Text Status_Text;
    public Button LoadButton;
    public Button UploadButton;
    public
    void Awake()
    {


        // Initialize the S3 client (use proper credential management)
        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(Region));
#if UNITY_EDITOR
        TestConnection().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.LogError("Connection test failed");
        });
#endif
    }


    private void Start()
    {

    
        UploadButton.onClick.AddListener(OnUploadButtonClick);
        LoadButton.onClick.AddListener(OnLoadButtonClick);

        

    }
    public async Task TestConnection()
    {
        try
        {
            Debug.Log("Attempting to connect to S3...");
            Status_Text.text = "Attempting to connect to S3...";
            var response = await _s3Client.ListBucketsAsync();

            Debug.Log($"Successfully connected to AWS S3! Found {response.Buckets.Count} buckets");

            Status_Text.text = "$Successfully connected to AWS S3! Found {response.Buckets.Count} buckets";
            // Optional: Test specific bucket access
            try
            {
                var locationRequest = new GetBucketLocationRequest()
                {
                    BucketName = BucketName
                };
                var locationResponse = await _s3Client.GetBucketLocationAsync(locationRequest);
                Debug.Log($"Bucket location: {locationResponse.Location}");

                Status_Text.text = $"Bucket location: {locationResponse.Location}";
                UploadButton.interactable=true;
                 LoadButton.interactable=true;

            }
            catch (Exception ex)
            {


                Status_Text.text = $"Couldn't access bucket: {ex.Message}";
            }
        }
        catch (AmazonS3Exception s3Ex)
        {
            Status_Text.text = $"S3 Connection Error: {s3Ex.Message}";
            Debug.LogError($"S3 Connection Error: {s3Ex.Message}");
            Debug.LogError($"Error Code: {s3Ex.ErrorCode}");
            Debug.LogError($"HTTP Status: {s3Ex.StatusCode}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"General Connection Error: {ex.Message}");
            Status_Text.text = $"General Connection Error: {ex.Message}";
        }
    }

    public void OnLoadButtonClick()
    {
        LoadButton.interactable=false;
        // Example call - replace with your actual S3 key
        //S3 key mean file or image name
        DownloadAndDisplayImage("DracoArts.png", displayImage).ContinueWith(task =>
        {
            if (task.IsFaulted){
                Debug.LogError("Image download failed");
            Status_Text.text = "Image download failed";}
        });

    }
    // Example usage
    public void OnUploadButtonClick()
    {                UploadButton.interactable=false;

        // Upload with a unique filename
        string fileName = $"user_images/{System.DateTime.Now:yyyyMMddHHmmss}.png";
        UploadUIImage(imageToUpload, fileName).ContinueWith(task =>

        { 
            
            
            if (task.IsFaulted){
                Debug.LogError("Upload failed");
                Status_Text.text = "Upload failed";}
        });
    }
    public async Task DownloadAndDisplayImage(string s3Key, RawImage targetImage)
    {
        try
        {
            // Create a temporary path
            string tempPath = Path.Combine(Application.temporaryCachePath, Path.GetFileName(s3Key));

            // Download the file first
            var request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = s3Key
            };

            using (var response = await _s3Client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            using (var fileStream = File.Create(tempPath))
            {
                await responseStream.CopyToAsync(fileStream);
            }

            // Load the downloaded image into a texture
            byte[] fileData = File.ReadAllBytes(tempPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // This auto-resizes the texture
                    LoadButton.interactable=true;
                SuccesslPanel.SetActive(true);

            // Apply to UI
            if (targetImage != null)
            {
                targetImage.texture = texture;
                Debug.Log($"Image set on UI: {s3Key}");
                Status_Text.text =$"Image set on UI: {s3Key}";

            
            }

            // Clean up
            File.Delete(tempPath);
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"S3 error: {e.Message}");
                            Status_Text.text =e.Message;

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading image: {e.Message}");
            Status_Text.text=e.Message;
        }
    }
    public async Task UploadUIImage(RawImage uiImage, string s3Key)
    {
        if (uiImage == null || uiImage.texture == null)
        {
            Debug.LogError("No image assigned or texture missing");
            Status_Text.text="No image assigned or texture missing";
            return;
        }

        try
        {
            // Convert the UI image texture to bytes
            Texture2D texture = (Texture2D)uiImage.texture;
            byte[] imageBytes = texture.EncodeToPNG(); // or EncodeToJPG()

            // Create upload request
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = s3Key,
                InputStream = new MemoryStream(imageBytes),
                ContentType = "image/png", // Change to "image/jpeg" if using JPG
                //CannedACL = S3CannedACL.PublicRead // Adjust permissions as needed
            };

            // Upload the file
            await _s3Client.PutObjectAsync(putRequest);
            Status_Text.text=$"UI image uploaded successfully to s3://{BucketName}/{s3Key}";
            Debug.Log($"UI image uploaded successfully to s3://{BucketName}/{s3Key}");
            UploadButton.interactable=true;
            SuccesslPanel.SetActive(true);
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"S3 error: {e.Message}");
            Status_Text.text=e.Message;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error uploading UI image: {e.Message}");
            Status_Text.text=e.Message;
        }
    }
    public async Task UploadTexture(Texture2D texture, string s3Key, ImageFormat format = ImageFormat.PNG)
    {
        try
        {
            byte[] imageBytes;
            string contentType;

            if (format == ImageFormat.PNG)
            {
                imageBytes = texture.EncodeToPNG();
                contentType = "image/png";
            }
            else // JPG
            {
                imageBytes = texture.EncodeToJPG();
                contentType = "image/jpeg";
            }

            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = s3Key,
                InputStream = new MemoryStream(imageBytes),
                ContentType = contentType,
                //  CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(putRequest);
            Debug.Log($"Texture uploaded to s3://{BucketName}/{s3Key}");
            Status_Text.text=$"Texture uploaded to s3://{BucketName}/{s3Key}";
        }
        catch (Exception e)
        {
            Debug.LogError($"Error uploading texture: {e.Message}");
        }
    }






 











    // Example: Upload a file
    public async Task UploadFile(string filePath, string s3Key)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = s3Key,
                FilePath = filePath
            };

            await _s3Client.PutObjectAsync(putRequest);
            Debug.Log("File uploaded successfully");
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"S3 error: {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }




}

