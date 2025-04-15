
# Welcome to DracoArts

![Logo](https://dracoarts-logo.s3.eu-north-1.amazonaws.com/DracoArts.png)




#  AWS S3 Bucket Interaction in Unity

Amazon Simple Storage Service (S3) is a scalable cloud storage solution that can be effectively integrated into Unity applications. This interaction allows game developers to store and retrieve various types of data including game assets, player profiles, configuration files, and saved games in the cloud. The AWS SDK for .NET provides the necessary tools to implement this functionality within Unity projects.


##  1. AWS SDK for .NET in Unity
- The AWS SDK for .NET is the foundation for S3 interaction, providing:

- Client libraries for various AWS services

- Authentication and request handling

- Response processing utilities

- Asynchronous operation support compatible with Unity's execution model

## 2. S3 Service Concepts
- Key S3 concepts relevant to Unity integration:

- Buckets: Container for objects (similar to folders)

- Objects: Individual files with data and metadata

- Keys: Unique identifiers for objects within a bucket

- Regions: Geographic locations where buckets are hosted

# Functional Capabilities
## 1. Data Storage and Retrieval
### Upload Operations:

- Store game assets (textures, models, audio) in the cloud

-  Save player progress and statistics

- Backup configuration files

- Support for various file formats including binary, JSON, and XML

### Download Operations:

- Retrieve game assets on demand

- Load player profiles across devices

- Fetch configuration updates

- Support for partial downloads and resumable transfers

## 2. Advanced Features
### Object Management:

- Listing available objects with filtering

- Copying and moving objects between locations

- Deleting obsolete files

- Setting object metadata and tags

### Access Control:

- Generate temporary access URLs (pre-signed URLs)

- Set fine-grained permissions

- Configure Cross-Origin Resource Sharing (CORS) for web builds

## Performance Optimization:

- Multipart uploads for large files

- Transfer acceleration for global distribution

- Caching strategies for frequently accessed content

# Prerequisites
- AWS account with S3 access

 - Install AWS .NET Package  Through Nuget 

- Proper IAM permissions configured
# Implementation Architecture

## 1. Client Initialization
#### The S3 client requires:

- AWS credentials (access key and secret key)

- Service region configuration

- Optional settings for retries, timeouts, and endpoints

## 2. Request Processing
#### Typical workflow:

- Create request object with parameters

- Execute request asynchronously

- Handle response or errors

- Process returned data

## 3. Unity Integration Points
 #### Key integration aspects:

- Coroutine-based async operations

- Main thread consideration for UI updates

- Platform-specific path handling

- Memory management for large objects

##  Performance Considerations
## 1. Network Efficiency
#### Strategies to optimize:

- Compression for large assets

- Delta updates for changed files

- Batch operations for multiple objects

- Intelligent prefetching of likely-needed assets

## 2. Local Caching
### Implementation patterns:

- Persistent local storage of downloaded files

- Cache validation using ETags or timestamps

- Automatic cleanup of stale cache entries

- Size-based cache limits

## 3. Concurrency Handling

### Approaches for smooth operation:

- Queue-based request management

- Priority-based download system

- Background loading where appropriate

- Connection quality adaptation
## Usage/Examples
    // Initialize the S3 client (use proper credential management)
        var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);

        _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(Region);

       //DownloadAndDisplayImage

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

    // Upload Image 

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

## Images

#### Download  Image


![](https://github.com/AzharKhemta/Gif-File-images/blob/main/AWS%20S3%20part%20_1.gif?raw=true)


 #### Upload  Image
 
![](https://github.com/AzharKhemta/Gif-File-images/blob/main/AWS%20S3%20part_2.gif?raw=true)

## Authors

- [@MirHamzaHasan](https://github.com/MirHamzaHasan)
- [@WebSite](https://mirhamzahasan.com)


## 🔗 Links

[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/company/mir-hamza-hasan/posts/?feedView=all/)
## Documentation

[Aws .Net Package](https://aws.amazon.com/sdk-for-net/)




## Tech Stack
**Client:** Unity,C#

**Plugin:** AWS SDK .NET



