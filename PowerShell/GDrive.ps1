function parse($root, $artist, $album, $path, $lastRun)
{
    $start = $root.IndexOf("?id=");
    $rootId = $root.Substring($start+4);
    $rootId = $rootId.Substring(0, $rootId.IndexOf("&"));
    
    $result = Invoke-WebRequest $root;

    # Get artist or album folder name
    $value = $null;
    $divs = $result.ParsedHtml.getElementsByTagName("div");
    foreach ($div in $divs)
    {
        $class = $div.attributes | Where-Object nodeName -like "class";
        if (($class.specified) -and ($class.value -eq "flip-folder-title"))
        {
            $spans = $div.getElementsByTagName("span");
            foreach ($span in $spans)
            {
                $value = $span.innerText;
            }
        }
    }
    if (($artist -eq $null) -or ($artist -eq "Music"))
    {
        $artist = $value;
    } 
    elseif ($album -eq $null)
    {
        $album = $value;
    }
    $path = $path + $value + "\";

    $divs = $result.ParsedHtml.getElementsByTagName("div");
    foreach ($div in $divs)
    {
        $class = $div.attributes | Where-Object nodeName -like "class";

        if (($class.specified) -and ($class.value -eq "flip-entry"))
        {
            $id = $div.attributes | Where-Object nodeName -like "id";
            $id1 = $id = $id.value.Substring(6);
            $url = "https://drive.google.com/folderview?id=" + $id1 + "&usp=sharing&tid=" + $rootId + "#list";
            if ($div.innerHTML.indexOf("drive-sprite-folder-list-shared-icon") -gt -1)
            {
                parse $url $artist $album $path $lastRun;
            }
            if ($div.innerHTML.indexOf(".mp3") -gt -1)
            {
                #Parse song
                $f = $div.attributes | Where-Object nodeName -like "aria-label";
                $filename = $f.nodeValue;
                $filename = $filename.Substring(6, $filename.IndexOf(", press Enter to open.") -6);
                $artist1 = $artist;
                if ($album -ne $null)
                {
                    $artist1 = $artist + "-" + $album + "-";
                }
                $url = "https://drive.google.com/uc?id=" + $id1 + "&export=download";
                updateSong $artist $album $filename $url $path;
            }
        }
    }
}


function updateSong($artist,$album,$filename,$gdrive,$path)
{
    $id = $null;
    $con = New-Object System.Data.SqlClient.SqlConnection
    $con.ConnectionString = "";
    $con.StatisticsEnabled = $True
    $con.Open()

    if ($album -eq $null)
    {
        $album = $artist;
        $artist = "%";
    }
    $filename = "%" + $path + $filename;
    $query = "select top 1 id,artist,title,location,gdrive from song where location like @filename and gdrive is null";
    $parameters = @{filename=$filename};
    $songs = Invoke-SqlQuery -Connection $con -Query $query -Parameters $parameters;
    $updated = $false;
    foreach ($song in $songs)
    {
        $id = $song.id;

        $update = "update song set gdrive = @gdrive where id=@id";
        $parameters = @{gdrive=$gdrive;id=$id};
        Invoke-SqlQuery -Connection $con -Query $update -Parameters $parameters ;
        $updated = $true;
        $filename;
    }
    $con.Close();
    return $id;
}

function getAzureTable()
{
    $tableName = "gdrive";


    $blobEndpoint = New-Object System.Uri("http://$accountName.blob.core.windows.net/");
    $queueEndpoint = New-Object System.Uri("http://$accountName.queue.core.windows.net/");
    $tableEndpoint = New-Object System.Uri("http://$accountName.table.core.windows.net/");
    $credentials = New-Object Microsoft.WindowsAzure.Storage.Auth.StorageCredentials($accountName, $accountKey);
    $storageAccountInfo = New-Object Microsoft.WindowsAzure.Storage.CloudStorageAccount($credentials, $blobEndpoint, $queueEndpoint, $tableEndpoint);
    $cloudTableClient = $storageAccountInfo.CreateCloudTableClient();
    $cloudTable = $cloudTableClient.GetTableReference($tableName);
    return $cloudTable;
}

function updateLastRun()
{
    $cloudTable = getAzureTable;

    $dynamicTableEntity = New-Object Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity;
    $dynamicTableEntity.PartitionKey = "lastIndexed";

    $date = Get-Date;
    $dateString = $date.ToString();
    $dateString = "today";

    $dynamicTableEntity.RowKey = $dateString;
    $to = [Microsoft.WindowsAzure.Storage.Table.TableOperation]::InsertOrReplace($dynamicTableEntity);
    $tr = $cloudTable.Execute($to);
}

function getLastRun()
{
    $cloudTable = getAzureTable;

    $partitionKey = "lastIndexed";
    $rowKey = "today";
    $fetch = [Microsoft.WindowsAzure.Storage.Table.TableOperation]::Retrieve($partitionKey, $rowKey);
    $result = $cloudTable.Execute($fetch);

    $start = $result.Etag.IndexOf("'")
    $lastRunString = $result.Etag.Substring($start + 1);
    $start1 = $lastRunString.IndexOf("T");
    $lastRunString = $lastRunString.Substring(0,$start1);
    $lastRun = [System.DateTime]::Parse($lastRunString)
    return $lastRun;
}

cls;
Import-Module InvokeSqlQuery;
#Add-Type -Path "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\v2.2\ref\Microsoft.WindowsAzure.StorageClient.dll";
Add-Type -Path "C:\Program Files\Microsoft SDKs\Windows Azure\.NET SDK\v2.3\ref\Microsoft.WindowsAzure.Storage.dll";

$lastRun = getLastRun;
updateLastRun;

$lastRunString = "2014-03-31";
$lastRun = [System.DateTime]::Parse($lastRunString);

#$root = "https://drive.google.com/folderview?id=0B4dXRBkWJRuDQl9hX2dsVkZ4ME0&usp=sharing&tid=0B4dXRBkWJRuDVG9kS3ExTGN1cE0#list"
#$root = "https://drive.google.com/folderview?id=0B4dXRBkWJRuDX3p6TGY0SDRtREk&usp=sharing&tid=0B4dXRBkWJRuDVG9kS3ExTGN1cE0#list";
#$root = "https://drive.google.com/folderview?id=0B4dXRBkWJRuDc2tfTHlzTTZHbGM&usp=sharing&tid=0B4dXRBkWJRuDVG9kS3ExTGN1cE0#list";
#$root = "https://drive.google.com/folderview?id=0B4dXRBkWJRuDWGZyR2NlV0Ruc0k&usp=sharing&tid=0B4dXRBkWJRuDVG9kS3ExTGN1cE0#list";
$root = "https://drive.google.com/folderview?id=0B4dXRBkWJRuDVG9kS3ExTGN1cE0&usp=sharing";
$path = "Music\";
$path = "";
parse $root "" "" $path $lastRun;

