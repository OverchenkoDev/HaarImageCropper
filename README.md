# HaarImageCropper
Image cropper used for preparing object samples for further Haar cascade training. Creates a samples of objects from original images.
Cropped samples must contain just objects which are the purpose of Haar cascade recognition. Good - samples with needed objects, bad - samples without it. Markup for good samples will contain sizes of sample so it need to contain just recognized objects.
Program created on .Net Framework 4.7 without any third-party libraries, just with C# 7 and WPF.
You can find compiled program here in the releases.

## Features
1. Images of all sizes can be uploaded. Images are shown in their oiriginal size (scrollable if the resolution is big) so there no scaling and objects distortion.
2. Multiple selection. You can select many regions with the same parameters and save all of them at once. **Selection creates** by the mouse left-clicking (mouse down - start selection, mousemove - expand. mouseup - end selection). **Mouse right-click** clears the last created selection.
3. Automatical creating of markup in the format needed in Haars cascade training.

## Settings
**Source images directory** - choose the directory with original images to crop objects from them. List of images will be shown there. Navigation by clicking on rows.
**Save samples directory** - choose the directory where the results of cropping will be saved. Folders for good and bad samples will be created there automatically.
**Save options** - choose saving options. Set the type of current crops and number of objects on current selection. Also there can be set if the markup file is creating automatically. Creates two files: for good samples write lines in format "{path of sample relative to this file} {number of objects} 0 0 {sample.width} {sample.Height}", for bad samples lines  will present just path to each sample relative to this file.
**All settings are necessary**
