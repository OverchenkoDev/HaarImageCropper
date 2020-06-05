# HaarImageCropper
# English
Image cropper used for preparing object samples for further Haar cascade training by OpenCV. Creates a samples of objects from original images.
For training needed two types of samples. Good cropped samples must contain just objects which are the purpose of Haar cascade recognition. Bad - samples without it. Markup for good samples will contain sizes of sample so it need to contain just recognized objects.

Program created on .Net Framework 4.7 without any third-party libraries, just with C# 7 and WPF.
You can find compiled program here in the releases.

## Features
1. Images of all sizes can be uploaded. Images are shown in their oiriginal size (scrollable if the resolution is big) so there no scaling and objects distortion.
2. Multiple selection. You can select many regions with the same parameters and save all of them at once. **Selection creates** by the mouse left-clicking (mouse down - start selection, mousemove - expand. mouseup - end selection). **Mouse right-click** clears the last created selection.
3. Automatical creating of markup in the format needed in Haars cascade training.

## Settings
**Source images directory** - choose and manipulate the directory with original images to crop objects from them. List of images will be shown there. Navigation by clicking on rows.

**Save samples directory** - choose the directory where the results of cropping will be saved. Folders for good and bad samples will be created there automatically.

**Save options** - choose saving options. Set the type of current crops and number of objects on current selection. Also there can be set if the markup file is creating automatically. Creates two files: for good samples write lines in format "{path of sample relative to this file} {number of objects} 0 0 {sample.width} {sample.Height}", for bad samples lines  will present just path to each sample relative to this file.

**All settings are necessary**

# Russian
Программа используется для подготовки кадров объектов для последующего обучения каскада Хаара их распознаванию с помощью OpenCV. Сохраняет выделенные участки исходных изображений. Для тренировки каскада используются кадры двух типов - положительные и отрицательные. Положительные должны содержать только распознаваемый объект, отрицательные - участки изображения без объекта.

Программа написана без использования сторонних библиотек, только с использованием .Net Framework 4.7 и WPF. Скомпилированное приложение находится в релизах.

## Особенности
1. Могут быть загружены изображения любых размеров. Отображаются в исходном размере с возможностью скроллинга области изображения. Таким образом, исключены искажения объектов вследствие масштабирования изображения.
2. Может быть выделено любое количество областей изображения. Вы можете выделить сразу несколько областей с объектами одного типа (например, положительные участки, содержащие по одному объекту) и сохранить их за раз. **Выделение областей** производится с помощью левой кнопки мыши, с помощью правой удаляется последнее выделение.
3.Возможность автоматического создания файлов разметки в процессе работы в формате, необходимом для обучения.

## Настройки
**Source images directory** - выбор папки, содержащей исходные изображения с объектами. Поддерживаются различные форматы.

**Save samples directory** - выбор папки сохранения выделенных областей. Каталоги для положительных и отрицательных кадров будут созданы автоматически.

**Save options** - выбор опций сохранения. Выберите тип текущего выделения (положительные или отрицательные кадры), количество объектов на них. Также указывается, создавать ли автоматически файлы разметки. Создаёт два файла: формат строк описания положительных кадров "{путь к кадрам относительно данного файла} {количество объектов} 0 0 {ширина кадра} {высота кадра}", для отрицательных примеров строки описания содержат только пути к кадрам относительно файла. Оба файла создаются в корне директории сохранения.

**Все настройки являются обязательными**
