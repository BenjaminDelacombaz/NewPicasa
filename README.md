# Documentation application NewPicasa

## This project was created by Benjamin Delacombaz and Nicolas Henry
## Class : SI-T1a
## This document was created the 24.10.17

<div align="center">
  <img src="/Documentation/img/logo.png" width="35%"/><br><br>

![forthebadge](http://forthebadge.com/images/badges/makes-people-smile.svg)

![forthebadge](http://forthebadge.com/images/badges/made-with-c-sharp.svg)

</div>

For information to install the app, please check it to this link [:heart: :camera:](https://github.com/BenjaminDelacombaz/NewPicasa/blob/master/Install/install.md)

## Table of Contents
1. [Architecture](#architecture)
  * [Organisation work](#organisation-work)
  * [Git](#git)
2. [Problems encountered](#problems-encountered)
3. [Analysis](#analysis)
4. [Choices made](#choices-made)


## Architecture

### Organisation work

We have a normally Visual Studio architecture, we start with a blank WPF project.

<div align="center">
  <img src="/Documentation/img/imgArchitecture.jpg"/>
</div>

For work by pair, we add github in our Visual Studio.

### Git

For this project, we use Github to work by pair. We have the branch Master where we put always the latest version build. We have one branch for different part of the application:

- Master
- Development

After that, we merge the repository and make available the final version of the application.

Git was perfect for two reasons:

1. We can work on the same project on the same time and don't have any conflict with our version.
2. We always have a backup of our file on Github, so if we have any problem with our hardware, it's easy to take the full project again.

## Problems encountered

### Git
At the beginning, we used Git very little and we didn't really know how it worked. We had to learn and understand how to use it well.
The configuration of Git in Visual Studio 2017 is not very simple and all the command are translate in French...

### Learn C#/WPF
We used a lot of time for learn WPF and particularity of the C#.

## Analysis
When we detect a problem on the development, we recalculate the time it will take and modify the planning to see the time it will take and if we have the time to apply this correction.

If the time allow it, we correct imediately the problem.

If the time don't allow it, we finish the task with the bug and continue the project for finish him on the correct time but we write the task is completed to a percentage of made.

## Choices made
### C#/WPF
We chose the C# because the application had to be made for Windows and we thought it would be very simple to read and write the metadatas.
We chose WPF cause it's more performant and we can customize styles.

### Modification from the contract

At the start of the project, we made 17 task.

Now, the project is ended and we can see if we correctly manage this project.

the tasks were :
  1. Display a list of images.
  2. Open a context menu.
  3. Add one or more images.
  4. Rename photo.
  5. Save my images.
  6. identify people.
  7. Search images.
  8. Rate pictures.
  9. Ordonate pictures.
  10. Locate images.
  11. Put some tags on the pictures.
  12. Store images by event.
  13. Add comments.
  14. Print the pictures.
  15. Share images by email.
  16. Access metadata.
  17. Add date when the photo was taken.

In the end, we deleted some task because we didn't have the time to do all. We encountered some problem and need to fix them.

So, at the start, the planning look like this:
<!--IMG planning initial-->

But when the project was ended we deleted these point because we take a lot of delay.
Different problems where with the language used and other with the plannification.

<!-- IMG planning final -->

We have deleted these point :

* Open a context menu.
* Identify people.
* Locate images.
* Print the pictures.
* Share images by email.
* Add date when the photo was taken. We had discussed with the client and agreed it's not necessary to add or update the date.

### The application

NewPicasa is made with C# and WPF. It's a photo management application using metadata to store various datas like comments, notes, tags.
During the development we used 4 folders:
  * class: This is where all classes are stored.
  * image: This is where all images are stored.
  * other: This is where all various documents like todo lists, informations are stored.
  * view: This is where all windows are stored

#### Class folder
In the class folder, we have 4 classes:
  * ImageMetada: Stores image information such as name, size, and metadata
  * Utilities: In Utilities class we can find a list of static functions tools like convert array to string, files copy, check if file exist, etc...
  * ImageDetails: ImageDetails is used for all image list's in the application. This class allows to manage the binding with WPF
  * UriToBitmapConverter: This class is useful for the image list to use less memory.

#### View folder
In the view folder, we have 4 windows:
  * winAddPhoto: This window provide tools for import images in the application
  * winMain_V2: This is the main window. We have a treeview to navigate in image folders, a list of images and informations of the images.
  * winParameter: Here, we can edit the default images folder.
  * winSaveImage: From this window we can save the images folder to another folder.

#### Navigation
For navigate in the application, we use a simple menu.
<div align="center">
  <img src="/Documentation/img/Arch.png" width="100%"/>
</div>

#### Use of classes
  * ImageMetada: winMain_V2.
  * Utilities: winMain_V2, winAddPhoto, winParameter, winSavePhoto.
  * ImageDetails: winMain_V2.
  * UriToBitmapConverter: winMain_V2.

### Problems encountered
#### Loading
If we have a lot images to load, it can be take several minutes. If we load images list in the main process, the window gonna be freezed and the user can think the application has crash.
That is why we decided to use a thread and a loading gif image.
The time does not allow us to spend a lot of time on the thread. The application can handle only one thread at a time and can't be interrupted until it is finished.
<div align="center">
  <img src="/Documentation/img/winMain_V2_Loading.png" width="100%"/>
</div>

#### Memory
First, to build the list, we used the images directly, which was then resized to the correct size. If the images were too big, the application could use up to 4GB of RAM.
To correct this problem, we found on the Internet a class that allows to resize the images in memory to show only the version with the right size.

#### Optimization
We currently have an optimization problem when we load and convert images, if images are too big the time to load can take a lot of minutes.
When we make a research, the loading take a lot of time, cause we check a few value in the images with the ImageMetadata class's but the ImageMetadata class's is not optimized and is heavy to employ.
We didn't have time to correct that.

#### Images without metadatas
If an image doesn't have the ''place'' to insert metadata, we must create it by recreate the file with this ''place'' and replace it.
We found a solution on Internet but it doesn't work, we have an rights error when we must replace the file.
We didn't have time to debug this part of code.

#### Read and Write metadatas
We used a lot of time for finding the best way to read and write metadatas.
The biggest problem we encountered is the file access.

#### Error handling
Error handling is particularly complicated and we took a lot of time for make a ''good'' error handling.

#### Store user settings
Store user settings is a little problem but very important.
First, we have think it was good to put user settings in a '.ini' files. After a little bit of reading, we saw it's actually no more used.
We decided to store the user settings in Windows Registry cause it was the more simple to use.
