# ExifDateTimeCorrection

This is a simple application to correct the DateTime Exif tags of a photo.  

Set a source Folder and a destination folder. The application will copy the files from the source folder to the destination folder and correct the DateTime Exif tags of the photo if needed.
To get the correct DateTime, the application will analyze it's name and the DateTimeOriginal.

It will extract the *correct* DataTime from the file name, like :
- IMG_20190101_120000.jpg will be `2019-01-01 12:00:00`
- IMG-20190101-120000.jpg will be `2019-01-01 12:00:00`
- WP_20190101_12_00_00_Pro.jpg will be `2019-01-01 12:00:00`
- Screenshot_20190101-120000.jpg will be `2019-01-01 12:00:00`
- 20190101_120000.jpg will be `2019-01-01 12:00:00`
- xxxxx_BURST20190101_120000.jpg will be `2019-01-01 12:00:00`
- 20190101_12_00_00.jpg will be `2019-01-01 12:00:00`
- 20190101_12.00.00.jpg will be `2019-01-01 12:00:00`
- 20190101120000.jpg will be `2019-01-01 12:00:00`

It will change the Exif Tags:
- DateTimeOriginal
- DateTimeDigitized
- DateTime

It will also change the file's :
- Creation DateTime
- Last Write DateTime
- Last Access DateTime

At each run, the application will create a log file in the destination folder.