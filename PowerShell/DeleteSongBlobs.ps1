function main()
{
    cls;
    Import-Module InvokeSqlQuery;
    Add-Type -Path "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\v2.2\ref\Microsoft.WindowsAzure.StorageClient.dll";

    $con = New-Object System.Data.SqlClient.SqlConnection
    $con.ConnectionString = "";
    $con.StatisticsEnabled = $True
    $con.Open()

    $query = "select id,artist,title from song where uploaded = 1";

    $songsToDelete = New-Object System.Collections.ArrayList;
    $songs = Invoke-SqlQuery -Connection $con -Query $query -Parameters $parameters 
    foreach ($song in $songs)
    {
        $songsToDelete.Add($song.id);
    }

    $query = "update song set uploaded = 0 where uploaded = 1";
    Invoke-SqlQuery -Connection $con -Query $query;
    $con.Close();

    deleteBlobs($songsToDelete);
}

function deleteBlobs($songsToDelete)
{
    $containerUri = "http://voidingthevoid.blob.core.windows.net/public";
    $accountName = "voidingthevoid";
    $accountKey = "po5OnNRfg8cNX+kBv5hkjsFyLRaJmf/ifHoPMCeYl1KA4zShZrRJKQoM8JYBDjAO+/Bg2IGyVzqi5w0AlFui2g==";
    $blobEndpoint = New-Object System.Uri("http://voidingthevoid.blob.core.windows.net/", $accountName);
    $queueEndpoint = New-Object System.Uri("http://voidingthevoid.queue.core.windows.net/", $accountName);
    $tableEndpoint = New-Object System.Uri("http://voidingthevoid.table.core.windows.net/", $accountName);
    $credentials = New-Object Microsoft.WindowsAzure.StorageCredentialsAccountAndKey($accountName, $accountKey);

    $storageAccountInfo = New-Object Microsoft.WindowsAzure.CloudStorageAccount($credentials, $blobEndpoint, $queueEndpoint, $tableEndpoint);

    $cloudBlobClient = [Microsoft.WindowsAzure.StorageClient.CloudStorageAccountStorageClientExtensions]::CreateCloudBlobClient($storageAccountInfo);
    $cloudBlobClient.Timeout = New-Object System.TimeSpan(0, 30, 0);

    $containers = $cloudBlobClient.ListContainers();
    foreach ($container in $containers)
    {
        if ($container.Uri -like $containerUri)
        {
            foreach ($songToDelete in $songsToDelete)
            {
                $targetUri = "http://voidingthevoid.blob.core.windows.net/public/" + $songToDelete + ".mp3";
                $blob = $cloudBlobClient.GetBlobReference($targetUri);
                $blob.Delete();
            }


        }
    }
}

main;