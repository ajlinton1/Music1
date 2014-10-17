param([string] $musicFolder)

function Main()
{
    Log "Starting";

    Add-Type -Path "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\2012-10\ref\Microsoft.WindowsAzure.StorageClient.dll";
    $accountName = "voidingthevoid";
    $accessKey = "po5OnNRfg8cNX+kBv5hkjsFyLRaJmf/ifHoPMCeYl1KA4zShZrRJKQoM8JYBDjAO+/Bg2IGyVzqi5w0AlFui2g==";
    $storageCredentialsAccountAndKey = New-Object Microsoft.WindowsAzure.StorageCredentialsAccountAndKey($accountName, $accessKey);
    $cloudStorageAccount = New-Object Microsoft.WindowsAzure.CloudStorageAccount($storageCredentialsAccountAndKey, $true);
    $baseQueueAddress = "http://voidingthevoid.queue.core.windows.net/";
    $cloudQueueClient = New-Object Microsoft.WindowsAzure.StorageClient.CloudQueueClient($baseQueueAddress, $storageCredentialsAccountAndKey);
    [string] $queueName = [System.Environment]::MachineName;
    $queueName = $queueName.ToLower();
    $cloudQueue = $cloudQueueClient.GetQueueReference($queueName);
    $blobContainerName = "public";

    $baseBlobAddress = "http://voidingthevoid.blob.core.windows.net/";
    $cloudBlobClient = New-Object Microsoft.WindowsAzure.StorageClient.CloudBlobClient($baseBlobAddress, $storageCredentialsAccountAndKey);
    $timeout = New-Object System.TimeSpan(0,15,0);
    $cloudBlobClient.Timeout = $timeout;
    $filename = $null;

    do
    {
        $message = $cloudQueue.GetMessage();
        if ($message -ne $null)
        {
            try
            {
                [string]$messageStr = $message.AsString;
                Log "Processing message: $messageStr";
                $loc = $messageStr.IndexOf("---");
                if ($loc -gt -1)
                {
                    $idString = $messageStr.Substring(0,$loc);
                    $location = $messageStr.Substring($loc + 3);
                    $fileInfo = New-Object System.IO.FileInfo($location);
                    $c = New-Object char[] 1;
                    $c[0] = '\';
                    $s1 = $location.Split($c);
                    $name = $fileInfo.Name;
                    if ($s1.Length -gt 3)
                    {
                        $artistFolder = $musicFolder;
                        for ($i = 2; $i -lt ($s1.Length - 1); $i++)
                        {
                            $artistFolder = $artistFolder + '\' + $s1[$i];
                            $directoryInfo = New-Object System.IO.DirectoryInfo($artistFolder);
                            if (!$directoryInfo.Exists)
                            {
                                [System.IO.Directory]::CreateDirectory($artistFolder);
                            }
                        }       
                        $filename = $artistFolder + '\' + $name;
                    }
                    else
                    {
                        $filename = $musicFolder + '\' + $name;
                    }
                    $key = $idString + ".mp3";

                    $cloudBlobContainer = $cloudBlobClient.GetContainerReference($blobContainerName);
                    $blob = $cloudBlobContainer.GetBlobReference($key);
                    Log "Downloading $filename";
                    $blob.DownloadToFile($filename);
                }
                $cloudQueue.DeleteMessage($message);
            }
            catch [Exception]
            {
                Log "Error";
            }
        
        }
    } while ($message -ne $null)
    Log "Done"
}

function Log($message)
{
    Write-Host $message;
    Write-EventLog -LogName SongDownloader -Source SongDownloaderSource -EventId 0 -Message $message;
}

Main;